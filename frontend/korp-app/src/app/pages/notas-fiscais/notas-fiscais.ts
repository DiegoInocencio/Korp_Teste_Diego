import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, catchError, EMPTY, switchMap, map } from 'rxjs';
import { Estoque, Produto } from '../../services/estoque';
import { Faturamento } from '../../services/faturamento';

type StatusNota = 0 | 1; // 0 = Aberta, 1 = Fechada

interface NotaFiscal {
  id: string;
  numero: number;
  status: StatusNota;
}

@Component({
  selector: 'app-notas-fiscais',
  imports: [ReactiveFormsModule],
  templateUrl: './notas-fiscais.html',
  styleUrl: './notas-fiscais.scss',
})
export class NotasFiscais implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly estoqueService = inject(Estoque);
  private readonly faturamentoService = inject(Faturamento);

  readonly produtos = signal<Produto[]>([]);
  readonly notaFiscal = signal<NotaFiscal | null>(null);
  loading = false;

  readonly form = this.fb.group({
    produtoId: [null as string | null, [Validators.required]],
    quantidade: [1, [Validators.required, Validators.min(1)]],
  });

  // ngOnInit é usado para carregar os produtos assim que o componente é inicializado.
  ngOnInit(): void {
    this.estoqueService.buscarProdutos().subscribe({
      next: (resposta) => {
        this.produtos.set(resposta.itens);
      },
      error: () => {
        alert('Nao foi possivel carregar os produtos.');
      },
    });
  }

  emitirEImprimirNota(): void {
    if (this.form.invalid || this.loading || !this.podeEmitirImprimir()) {
      this.form.markAllAsTouched();
      return;
    }

    const { produtoId, quantidade } = this.form.getRawValue();
    if (produtoId === null) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;

    this.faturamentoService
      .criarNota({ produtoId, quantidade: Number(quantidade) })
      .pipe(
        // RxJS: switchMap encadeia a impressão após criar a nota.
        switchMap((notaApi) => {
          const notaCriada: NotaFiscal = {
            id: notaApi.id,
            numero: notaApi.numero,
            status: this.mapearStatus(notaApi.status),
          };

          this.notaFiscal.set(notaCriada);

          if (notaCriada.status !== 0) {
            return EMPTY;
          }

          return this.faturamentoService.imprimirNota(notaCriada.id).pipe(map(() => notaCriada.id));
        }),
        map((notaId) => {
          if (notaId) {
            this.notaFiscal.update((nota) => (nota ? { ...nota, status: 1 } : nota));
          }
          return notaId;
        }),
        // RxJS: catchError captura falhas de comunicação entre microsserviços.
        catchError(() => {
          alert('Falha na comunicação entre microsserviços. Tentando recuperar...');
          return EMPTY;
        }),
        finalize(() => {
          this.loading = false;
        }),
      )
      .subscribe({
        next: () => {
          alert('Nota impressa com sucesso!');
          this.form.patchValue({ quantidade: 1 });
        },
      });
  }

  podeEmitirImprimir(): boolean {
    const nota = this.notaFiscal();
    return nota === null || nota.status === 0;
  }

  obterStatusNota(): string {
    return this.notaFiscal()?.status === 1 ? 'Fechada' : 'Aberta';
  }

  private mapearStatus(statusApi: number): StatusNota {
    return statusApi === 2 ? 1 : 0;
  }
}
