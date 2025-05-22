//@CodeCopy
import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageBoxComponent } from '@app-components/base/message-box/message-box.component';

@Injectable({
  providedIn: 'root',
})
export class MessageBoxService {
  constructor(private modal: NgbModal) {

  }

  public show(message: string, title = 'Hinweis', okText = 'OK'): Promise<boolean> {
    const ref = this.modal.open(MessageBoxComponent, { centered: true });

    ref.componentInstance.title = title;
    ref.componentInstance.message = message;
    ref.componentInstance.okText = okText;
    return ref.result;
  }

  public confirm(
    message: string,
    title = 'Bitte bestätigen',
    okText = 'Ja',
    cancelText = 'Nein'): Promise<boolean> {
    const ref = this.modal.open(MessageBoxComponent, { centered: true });

    ref.componentInstance.title = title;
    ref.componentInstance.message = message;
    ref.componentInstance.okText = okText;
    ref.componentInstance.cancelText = cancelText;
    return ref.result;
  }
}
