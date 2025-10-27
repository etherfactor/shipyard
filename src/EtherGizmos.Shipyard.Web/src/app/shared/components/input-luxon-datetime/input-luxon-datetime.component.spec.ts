import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputLuxonDatetimeComponent } from './input-luxon-datetime.component';

describe('InputLuxonDatetimeComponent', () => {
  let component: InputLuxonDatetimeComponent;
  let fixture: ComponentFixture<InputLuxonDatetimeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InputLuxonDatetimeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InputLuxonDatetimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
