import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CadastrarProdutoPayload, Estoque, Produto } from '../../services/estoque';

@Component({
  selector: 'app-produtos',
  imports: [ReactiveFormsModule],
  templateUrl: './produtos.html',
  styleUrl: './produtos.scss',
})
export class Produtos implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly estoqueService = inject(Estoque);

  readonly produtos = signal<Produto[]>([]);
  mostrarFormularioCadastro = false;
  salvandoCadastro = false;
  editandoProdutoId = signal<string | null>(null);
  salvandoEdicao = false;
  deletandoProdutoId = signal<string | null>(null);

  readonly formProduto: FormGroup = this.fb.group({
    codigo: ['', [Validators.required]],
    descricao: ['', [Validators.required]],
    saldo: [0, [Validators.required, Validators.min(0)]],
  });

  readonly formEdicao: FormGroup = this.fb.group({
    descricao: ['', [Validators.required]],
  });

  ngOnInit(): void {
    this.carregarProdutos();
  }

  toggleFormularioCadastro(): void {
    this.mostrarFormularioCadastro = !this.mostrarFormularioCadastro;
    if (!this.mostrarFormularioCadastro) {
      this.formProduto.reset({ codigo: '', descricao: '', saldo: 0 });
    }
  }

  cadastrar(): void {
    if (this.formProduto.invalid || this.salvandoCadastro) {
      this.formProduto.markAllAsTouched();
      return;
    }

    const { codigo, descricao, saldo } = this.formProduto.getRawValue();
    const produto: CadastrarProdutoPayload = {
      codigo: String(codigo).trim(),
      descricao: String(descricao).trim(),
      saldoInicial: Number(saldo),
    };

    this.salvandoCadastro = true;
    this.estoqueService.cadastrarProduto(produto).subscribe({
      next: () => {
        alert('Produto cadastrado com sucesso!');
        this.formProduto.reset({ codigo: '', descricao: '', saldo: 0 });
        this.mostrarFormularioCadastro = false;
        this.carregarProdutos();
        this.salvandoCadastro = false;
      },
      error: () => {
        alert('Nao foi possivel cadastrar o produto.');
        this.salvandoCadastro = false;
      },
    });
  }

  iniciarEdicao(produto: Produto): void {
    this.editandoProdutoId.set(produto.id);
    this.formEdicao.reset({ descricao: produto.descricao });
  }

  cancelarEdicao(): void {
    this.editandoProdutoId.set(null);
    this.formEdicao.reset({ descricao: '' });
  }

  salvarEdicao(produtoId: string): void {
    if (this.formEdicao.invalid || this.salvandoEdicao) {
      this.formEdicao.markAllAsTouched();
      return;
    }

    const descricao = String(this.formEdicao.getRawValue().descricao).trim();
    this.salvandoEdicao = true;
    this.estoqueService.atualizarProduto(produtoId, { descricao }).subscribe({
      next: () => {
        alert('Produto atualizado com sucesso!');
        this.cancelarEdicao();
        this.carregarProdutos();
        this.salvandoEdicao = false;
      },
      error: () => {
        alert('Nao foi possivel atualizar o produto.');
        this.salvandoEdicao = false;
      },
    });
  }

  excluir(produto: Produto): void {
    if (this.deletandoProdutoId() !== null) {
      return;
    }

    const confirmar = confirm(`Deseja excluir o produto ${produto.codigo}?`);
    if (!confirmar) {
      return;
    }

    this.deletandoProdutoId.set(produto.id);
    this.estoqueService.deletarProduto(produto.id).subscribe({
      next: () => {
        alert('Produto excluido com sucesso!');
        if (this.editandoProdutoId() === produto.id) {
          this.cancelarEdicao();
        }
        this.carregarProdutos();
        this.deletandoProdutoId.set(null);
      },
      error: () => {
        alert('Nao foi possivel excluir o produto.');
        this.deletandoProdutoId.set(null);
      },
    });
  }

  private carregarProdutos(): void {
    this.estoqueService.buscarProdutos().subscribe({
      next: (resposta) => {
        this.produtos.set(resposta.itens);
      },
      error: () => {
        alert('Nao foi possivel carregar os produtos.');
      },
    });
  }
}
