import { TestBed } from '@angular/core/testing';

import { OrderFulfillmentService } from './order-fulfillment.service';

describe('OrderFulfillmentService', () => {
  let service: OrderFulfillmentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrderFulfillmentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
