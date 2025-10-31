import { Component, Input } from '@angular/core';

@Component({
  selector: 'detail-box',
  standalone: true,
  imports: [],
  templateUrl: './detail-box.component.html',
  styleUrl: './detail-box.component.scss'
})
export class DetailBoxComponent {

  @Input({ required: true }) icon!: string;
  @Input({ required: true }) text!: string;
  @Input() buttons: DetailBoxButton[] = [];

  get iconClass(): string[] | undefined {
    if (!this.icon) {
      return undefined;
    }

    if (this.icon.startsWith("bi")) {
      return ["bi", this.icon];
    }

    throw new Error("Invalid icon name");
  }
}

export interface DetailBoxButton {
  color: string;
  text: string;
  callback: () => void;
}
