import { TestBed } from '@angular/core/testing';

import { NotificationSubscriptionService } from './notification-subscription.service';

describe('NotificationSubscriptionService', () => {
  let service: NotificationSubscriptionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificationSubscriptionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
