import { Component, Input } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DateTime } from 'luxon';

@Component({
  selector: 'input-luxon-datetime',
  standalone: true,
  imports: [
    FormsModule,
  ],
  templateUrl: './input-luxon-datetime.component.html',
  styleUrl: './input-luxon-datetime.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: InputLuxonDatetimeComponent,
      multi: true
    }
  ]
})
export class InputLuxonDatetimeComponent implements ControlValueAccessor {

  private _value: string | null = null;

  @Input() id?: string;
  @Input() placeholder?: string;
  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;

  onChange: (value: DateTime | null) => void = () => { };
  onTouched = () => { };

  get value() {
    return this._value;
  }

  set value(val: string | null) {
    if (this._value !== val) {
      console.log('set', val);
      this._value = val;

      const emitValue = val ? DateTime.fromISO(val) : null;
      this.onChange(emitValue);
    }
  }

  writeValue(obj: DateTime | null): void {
    let newValue: string | null;
    console.log('write', obj);
    if (obj && DateTime.isDateTime(obj)) {
      newValue = obj
        .startOf('minute')
        .toISO({
          suppressSeconds: true,
          suppressMilliseconds: true,
          includeOffset: false,
        });
    } else {
      newValue = null;
    }

    this.value = newValue;
  }

  registerOnChange(fn: (value: DateTime | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
