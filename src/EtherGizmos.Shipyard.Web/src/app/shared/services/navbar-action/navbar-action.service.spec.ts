import { TestBed } from '@angular/core/testing';

import { NavbarActionService } from './navbar-action.service';

describe('NavbarActionService', () => {
  let service: NavbarActionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NavbarActionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
