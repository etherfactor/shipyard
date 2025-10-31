import { Injectable } from '@angular/core';
import { SortDirection } from '@ethergizmos/odata-fluent-client';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root'
})
export class SortTableService {

  private sortings: { [id: Guid]: Sorting } = {};

  constructor() { }

  setSortDirection(id: Guid, sorting: Sorting | undefined) {
    if (sorting) {
      this.sortings[id] = { ...sorting };
    } else {
      delete this.sortings[id];
    }
  }

  getSortDirection(id: Guid, column: string) {
    const maybeSorting = this.sortings[id];
    if (maybeSorting && maybeSorting.column === column) {
      return maybeSorting.direction;
    } else {
      return undefined;
    }
  }
}

interface Sorting {
  column: string;
  direction: SortDirection;
}
