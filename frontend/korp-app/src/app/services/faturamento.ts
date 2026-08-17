import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface CriarNotaPayload {
  produtoId: string;
  quantidade: number;
}

export interface AdicionarItemPayload {
  produtoId: string;
  quantidade: number;
}

export interface CriarNotaFiscalResponse {
  id: string;
  numero: number;
  status: number;
}

@Injectable({
  providedIn: 'root',
})
export class Faturamento {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7099/api';

  criarNota(payload: CriarNotaPayload): Observable<CriarNotaFiscalResponse> {
    return this.http.post<CriarNotaFiscalResponse>(`${this.baseUrl}/NotasFiscais`, payload);
  }

  adicionarItem(notaId: string, payload: AdicionarItemPayload): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/NotasFiscais/${notaId}/itens`, payload);
  }

  imprimirNota(notaId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/NotasFiscais/${notaId}/imprimir`, {});
  }
}
