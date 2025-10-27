import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InitializeModal } from './initialize-modal.component';

describe('InitializeModalComponent', () => {
  let component: InitializeModal;
  let fixture: ComponentFixture<InitializeModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InitializeModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InitializeModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
