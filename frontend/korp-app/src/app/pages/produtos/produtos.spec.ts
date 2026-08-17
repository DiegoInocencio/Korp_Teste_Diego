import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { Produtos } from './produtos';
import { Estoque } from '../../services/estoque';

describe('Produtos', () => {
  let component: Produtos;
  let fixture: ComponentFixture<Produtos>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Produtos],
      providers: [
        {
          provide: Estoque,
          useValue: {
            buscarProdutos: () => of([]),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Produtos);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
