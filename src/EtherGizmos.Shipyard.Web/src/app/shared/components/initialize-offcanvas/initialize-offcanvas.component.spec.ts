import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InitializeOffcanvas } from './initialize-offcanvas.component';

describe('InitializeOffcanvasComponent', () => {
  let component: InitializeOffcanvas;
  let fixture: ComponentFixture<InitializeOffcanvas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InitializeOffcanvas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InitializeOffcanvas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
