import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationSubscriptionDetailComponent } from './notification-subscription-detail.component';

describe('NotificationSubscriptionDetailComponent', () => {
  let component: NotificationSubscriptionDetailComponent;
  let fixture: ComponentFixture<NotificationSubscriptionDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationSubscriptionDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationSubscriptionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
