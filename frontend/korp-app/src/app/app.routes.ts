import { Routes } from '@angular/router';
import { NotasFiscais } from './pages/notas-fiscais/notas-fiscais';
import { Produtos } from './pages/produtos/produtos';

export const routes: Routes = [
  { path: 'produtos', component: Produtos },
  { path: 'notasfiscais', component: NotasFiscais },
  { path: 'notas-fiscais', redirectTo: 'notasfiscais', pathMatch: 'full' },
  { path: '', pathMatch: 'full', redirectTo: 'produtos' }
];
