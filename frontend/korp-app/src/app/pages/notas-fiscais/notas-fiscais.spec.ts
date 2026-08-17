import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { NotasFiscais } from './notas-fiscais';
import { Estoque } from '../../services/estoque';
import { Faturamento } from '../../services/faturamento';

describe('NotasFiscais', () => {
  let component: NotasFiscais;
  let fixture: ComponentFixture<NotasFiscais>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotasFiscais],
      providers: [
        {
          provide: Estoque,
          useValue: {
            buscarProdutos: () => of([]),
          },
        },
        {
          provide: Faturamento,
          useValue: {
            criarNota: () => of({ id: 1, numero: 'NF-1' }),
            adicionarItem: () => of(void 0),
            imprimirNota: () => of(void 0),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(NotasFiscais);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
