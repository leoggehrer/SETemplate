//@CodeCopy
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IKey } from '@app/models/i-key';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

/**
 * A generic edit component for adding or editing items of type T.
 * The component uses Bootstrap modals for displaying the form.
 * 
 * @template T - A type that extends the IKey interface.
 */
@Component({
  selector: 'app-generic-edit',
  templateUrl: './generic-edit.component.html',
  styleUrl: './generic-edit.component.css'
})
export class GenericEditComponent<T extends IKey> {
  /**
   * The data item being edited or added.
   */
  @Input() dataItem!: T;

  /**
   * Event emitted when the form is saved.
   */
  @Output() save = new EventEmitter<T>();

  /**
   * Event emitted when the form is canceled.
   */
  @Output() cancel = new EventEmitter<void>();

  /**
   * Constructor for the GenericEditComponent.
   * 
   * @param activeModal - The active modal instance from NgbActiveModal.
   */
  constructor(public activeModal: NgbActiveModal) {}

  /**
   * Gets the title of the modal based on the data item's ID.
   * If the ID is 0, it indicates adding a new item; otherwise, editing an existing item.
   */
  public get title(): string {
    return this.dataItem.id === 0 ? 'Hinzufügen' : 'Bearbeiten';
  }

  /**
   * Closes the modal without returning any data.
   */
  public close() {
    this.activeModal.close();
  }

  /**
   * Dismisses the modal without returning any data.
   */
  public dismiss() {
    this.activeModal.dismiss();
  }

  /**
   * Submits the form. If there are observers for the `save` event, it emits the `save` event.
   * Otherwise, it closes the modal and returns the data item.
   */
  public submitForm() {
    if (this.save.observed) {
      this.save.emit(this.dataItem);
    } else {
      this.activeModal.close(this.dataItem);
    }
  }

  /**
   * Cancels the form. If there are observers for the `cancel` event, it emits the `cancel` event.
   * Otherwise, it dismisses the modal.
   */
  public cancelForm() {
    if (this.cancel.observed) {
      this.cancel.emit();
    } else {
      this.activeModal.dismiss();
    }
  }
}
