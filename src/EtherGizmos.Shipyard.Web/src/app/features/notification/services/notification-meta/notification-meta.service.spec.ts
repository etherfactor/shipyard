import { TestBed } from '@angular/core/testing';

import { NotificationMetaService } from './notification-meta.service';

describe('NotificationMetaService', () => {
  let service: NotificationMetaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificationMetaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
