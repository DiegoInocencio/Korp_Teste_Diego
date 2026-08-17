import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

export interface Produto {
  id: string;
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface CadastrarProdutoPayload {
  codigo: string;
  descricao: string;
  saldoInicial: number;
}

export interface AtualizarProdutoPayload {
  descricao: string;
}

export interface PaginacaoDto<T> {
  itens: T[];
  pageNumber: number;
  pageSize: number;
  totalItens: number;
}

interface PaginacaoApiDto<T> extends Partial<PaginacaoDto<T>> {
  Itens?: T[];
}

@Injectable({
  providedIn: 'root',
})
export class Estoque {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7208/api';

  cadastrarProduto(produto: CadastrarProdutoPayload): Observable<Produto> {
    return this.http.post<Produto>(`${this.baseUrl}/produtos`, produto);
  }

  atualizarProduto(id: string, payload: AtualizarProdutoPayload): Observable<Produto> {
    return this.http.put<Produto>(`${this.baseUrl}/produtos/${id}`, payload);
  }

  deletarProduto(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/produtos/${id}`);
  }

  buscarProdutos(pageNumber = 1, pageSize = 10): Observable<PaginacaoDto<Produto>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PaginacaoApiDto<Produto>>(`${this.baseUrl}/produtos`, { params }).pipe(
      map((resposta) => ({
        itens: resposta.itens ?? resposta.Itens ?? [],
        pageNumber: resposta.pageNumber ?? pageNumber,
        pageSize: resposta.pageSize ?? pageSize,
        totalItens: resposta.totalItens ?? 0,
      })),
    );
  }
}
