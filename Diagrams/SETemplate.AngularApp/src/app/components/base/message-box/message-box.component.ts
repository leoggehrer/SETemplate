//@CodeCopy
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-message-box',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './message-box.component.html',
})
export class MessageBoxComponent {
  @Input() title = 'Message';
  @Input() message = '';
  @Input() details?: string; // Optionale Details
  @Input() okText = 'OK';
  @Input() cancelText?: string;

  public showDetails = false; // Steuerung der Aufklapp-Funktion

  constructor(public activeModal: NgbActiveModal) { }

  public get hasDetails(): boolean {
    return !!(this.details && this.details.trim().length > 0);
  }

  public toggleDetails(): void {
    this.showDetails = !this.showDetails;
  }

  public confirm() {
    this.activeModal.close(true);
  }

  public dismiss() {
    this.activeModal.dismiss(false);
  }
}
