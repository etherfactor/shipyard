import { TestBed } from '@angular/core/testing';

import { CarrierExecutionService } from './carrier-execution.service';

describe('CarrierExecutionService', () => {
  let service: CarrierExecutionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CarrierExecutionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
