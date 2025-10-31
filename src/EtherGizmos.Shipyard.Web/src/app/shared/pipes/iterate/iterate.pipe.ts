import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'iterate',
  standalone: true
})
export class IteratePipe implements PipeTransform {

  transform(value: number): number[] {
    if (value && value > 0) {
      return Array.from({ length: value }, (_, index) => index);
    } else {
      return [];
    }
  }
}
