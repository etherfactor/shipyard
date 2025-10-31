import { TestBed } from '@angular/core/testing';

import { OidcLoggerService } from './oidc-logger.service';

describe('OidcLoggerService', () => {
  let service: OidcLoggerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OidcLoggerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
