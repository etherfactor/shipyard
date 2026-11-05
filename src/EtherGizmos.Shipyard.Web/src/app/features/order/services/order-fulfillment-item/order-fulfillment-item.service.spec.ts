import { TestBed } from '@angular/core/testing';

import { OrderFulfillmentItemService } from './order-fulfillment-item.service';

describe('OrderFulfillmentItemService', () => {
  let service: OrderFulfillmentItemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrderFulfillmentItemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
