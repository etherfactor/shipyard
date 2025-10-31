import { TestBed } from '@angular/core/testing';

import { TrackingUpdateService } from './tracking-update.service';

describe('TrackingUpdateService', () => {
  let service: TrackingUpdateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TrackingUpdateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
