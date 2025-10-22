import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrierExecutionListComponent } from './carrier-execution-list.component';

describe('CarrierExecutionListComponent', () => {
  let component: CarrierExecutionListComponent;
  let fixture: ComponentFixture<CarrierExecutionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrierExecutionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrierExecutionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
