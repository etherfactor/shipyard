import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationInboxComponent } from './notification-inbox.component';

describe('NotificationInboxComponent', () => {
  let component: NotificationInboxComponent;
  let fixture: ComponentFixture<NotificationInboxComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationInboxComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationInboxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
