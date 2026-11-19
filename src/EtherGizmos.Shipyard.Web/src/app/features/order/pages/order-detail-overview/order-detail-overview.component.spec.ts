import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderDetailOverviewComponent } from './order-detail-overview.component';

describe('OrderDetailOverviewComponent', () => {
  let component: OrderDetailOverviewComponent;
  let fixture: ComponentFixture<OrderDetailOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderDetailOverviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderDetailOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
