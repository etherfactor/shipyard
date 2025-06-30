import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputLuxonTimeComponent } from './input-luxon-time.component';

describe('InputLuxonTimeComponent', () => {
  let component: InputLuxonTimeComponent;
  let fixture: ComponentFixture<InputLuxonTimeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InputLuxonTimeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InputLuxonTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
