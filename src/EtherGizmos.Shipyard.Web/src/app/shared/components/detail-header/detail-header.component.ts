import { Component, Input } from '@angular/core';

@Component({
  selector: 'detail-header',
  standalone: true,
  imports: [],
  templateUrl: './detail-header.component.html',
  styleUrl: './detail-header.component.scss'
})
export class DetailHeaderComponent {

  @Input({ required: true }) text!: string;
  @Input() subtext?: string;
}
