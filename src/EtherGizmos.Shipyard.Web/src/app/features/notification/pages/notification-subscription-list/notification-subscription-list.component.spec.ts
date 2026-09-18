import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationSubscriptionListComponent } from './notification-subscription-list.component';

describe('NotificationSubscriptionListComponent', () => {
  let component: NotificationSubscriptionListComponent;
  let fixture: ComponentFixture<NotificationSubscriptionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationSubscriptionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationSubscriptionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
