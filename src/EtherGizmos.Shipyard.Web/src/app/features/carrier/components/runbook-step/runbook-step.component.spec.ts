import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RunbookStepComponent } from './runbook-step.component';

describe('RunbookStepComponent', () => {
  let component: RunbookStepComponent;
  let fixture: ComponentFixture<RunbookStepComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RunbookStepComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RunbookStepComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
