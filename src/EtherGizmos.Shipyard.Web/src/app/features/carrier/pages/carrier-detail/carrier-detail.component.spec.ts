import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrierDetailComponent } from './carrier-detail.component';

describe('CarrierDetailComponent', () => {
  let component: CarrierDetailComponent;
  let fixture: ComponentFixture<CarrierDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrierDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrierDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
