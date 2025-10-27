import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputLuxonDateComponent } from './input-luxon-date.component';

describe('InputLuxonDateComponent', () => {
  let component: InputLuxonDateComponent;
  let fixture: ComponentFixture<InputLuxonDateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InputLuxonDateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InputLuxonDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
