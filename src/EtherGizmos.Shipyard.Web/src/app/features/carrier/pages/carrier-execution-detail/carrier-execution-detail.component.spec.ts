import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrierExecutionDetailComponent } from './carrier-execution-detail.component';

describe('CarrierExecutionDetailComponent', () => {
  let component: CarrierExecutionDetailComponent;
  let fixture: ComponentFixture<CarrierExecutionDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrierExecutionDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrierExecutionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
