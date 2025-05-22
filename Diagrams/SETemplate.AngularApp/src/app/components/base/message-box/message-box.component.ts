//@CodeCopy
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-message-box',
  imports: [CommonModule ],
  templateUrl: './message-box.component.html',})
export class MessageBoxComponent {
  @Input() title = 'Nachricht';
  @Input() message = '';
  @Input() okText = 'OK';
  @Input() cancelText?: string;

  constructor(public activeModal: NgbActiveModal) {}

  public confirm() {
    this.activeModal.close(true);
  }

  public dismiss() {
    this.activeModal.dismiss(false);
  }
}
