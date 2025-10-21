//@CodeCopy
import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { MessageBoxComponent } from '@app-components/base/message-box/message-box.component';

@Injectable({
  providedIn: 'root',
})
export class MessageBoxService {
  constructor(
    private modal: NgbModal,
    private translateService: TranslateService
  ) {

  }

  public show(message: string, title?: string, okText?: string, details?: string): Promise<boolean> {
    const ref = this.modal.open(MessageBoxComponent, { centered: true });

    ref.componentInstance.title = title || this.translateService.instant('MESSAGE_BOX.TITLE');
    ref.componentInstance.message = message;
    ref.componentInstance.okText = okText || this.translateService.instant('MESSAGE_BOX.OK');
    ref.componentInstance.details = details || '';
    return ref.result;
  }

  public confirm(
    message: string,
    title?: string,
    okText?: string,
    cancelText?: string): Promise<boolean> {
    const ref = this.modal.open(MessageBoxComponent, { centered: true });

    ref.componentInstance.title = title || this.translateService.instant('MESSAGE_BOX.CONFIRM_TITLE');
    ref.componentInstance.message = message;
    ref.componentInstance.okText = okText || this.translateService.instant('MESSAGE_BOX.YES');
    ref.componentInstance.cancelText = cancelText || this.translateService.instant('MESSAGE_BOX.NO');
    return ref.result;
  }
}
