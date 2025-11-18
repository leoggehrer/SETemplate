//@CodeCopy
import { Directive, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { IdType, IdDefault, IKeyModel } from '@app-models/i-key-model';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

/**
 * A generic edit component for adding or editing items of type T.
 * The component uses Bootstrap modals for displaying the form.
 * 
 * @template T - A type that extends the IKey interface.
 */
@Directive()
export abstract class GenericEditComponent<T extends IKeyModel> implements OnInit {
  private _dataItem!: T;
  private _saveData: boolean = false;

  /**
   * The data item being edited or added.
   */
  @Input() set dataItem(value: T) {
    this._dataItem = value;
    console.log('dataItem wurde gesetzt:', value);
    this.dataItemChanged(value);
  }

  /**
   * Gets the current data item being edited or added.
   * @returns The data item of type T.
   */
  public get dataItem(): T {
    return this._dataItem;
  }

  /**
   * Event emitted when the form is saved.
   */
  @Output() save = new EventEmitter<T>();

  /**
   * Event emitted when the form is canceled.
   */
  @Output() cancel = new EventEmitter<void>();

  // Injizierte Services
  public activeModal = inject(NgbActiveModal);

  /**
   * Constructor for the GenericEditComponent.
   * 
   * @param activeModal - The active modal instance from NgbActiveModal.
   */
  constructor() { }

  /**
   * Initializes the component.
   */
  ngOnInit(): void {
  }

  /**
   * Gets the unique key of an item.
   * Subclasses should override this method to provide the logic for retrieving the item's key.
   * 
   * @param item - The item for which to get the key.
   * @returns The unique identifier of the item.
   */
  public getItemKey(item: T): IdType {
    return IdDefault;
  }

  /**
   * Gets the title of the modal based on the data item's ID.
   * If the ID is 0, it indicates adding a new item; otherwise, editing an existing item.
   */
  public get title(): string {
    return this.editMode ? 'Bearbeiten' : 'Hinzufügen';
  }

  /**
   * Gets the value indicating whether the form data should be saved.
   * @returns A boolean value indicating whether to save the data.
   */
  public get saveData(): boolean {
    return this._saveData;
  }

  /**
   * Sets the saveData property, which can be used to indicate whether the form data should be saved.
   * @param value A boolean value indicating whether to save the data.
   */
  public set saveData(value: boolean) {
    this._saveData = value;
  }

  /**
  * Returns true if the component is in edit mode (i.e., editing an existing item).
  * Returns false if adding a new item (id === 0).
  */
  public get editMode(): boolean {
    return this.getItemKey(this.dataItem) !== IdDefault;
  }

  /**
   * Sets the edit mode of the component.
   * This setter is provided for completeness but does not change the internal state,
   * as edit mode is determined by the data item's ID.
   * @param value A boolean value indicating whether to set edit mode.
   */
  public set editMode(value: boolean) {
  }

  /**
   * Called whenever the dataItem input is set or changed.
   * Can be overridden by derived classes to react to data changes.
   * @param dataItem The new data item of type T.
   */
  protected dataItemChanged(dataItem: T) {
    console.log('dataItemChanged aufgerufen mit:', dataItem);
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
   * This method is called before the form is submitted.
   * It can be overridden by derived classes to perform actions such as validation or data transformation.
   */
  protected beforeSubmit() {
    // This method can be overridden by derived classes to perform actions before submitting the form.
    // For example, validation or data transformation can be done here.
    console.log('beforeSubmit aufgerufen');
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

  /**
   * Gets the value of a date property from the data item as a string in 'YYYY-MM-DD' format.
   * Returns null if the property is not set.
   * @param prop The property key of the date field in the data item.
   * @returns The date as a string in 'YYYY-MM-DD' format, or null if not set.
   */
  public getDateString(prop: keyof T): string | null {
    const value = this.dataItem[prop] as Date;

    if (!value) return null;
    // Lokale Zeit für YYYY-MM-DD
    const year = value.getFullYear();
    const month = (value.getMonth() + 1).toString().padStart(2, '0');
    const day = value.getDate().toString().padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  /**
   * Sets the value of a date property on the data item from a string in 'YYYY-MM-DD' format.
   * If the value is null, the property is set to null. Otherwise, it is converted to a Date object.
   * @param prop The property key of the date field in the data item.
   * @param value The date as a string in 'YYYY-MM-DD' format, or null.
   */
  public setDateString(prop: keyof T, value: string | null) {
    (this.dataItem as any)[prop] = value ? new Date(value) : null;
  }

  /**
   * Gets the value of a date-time property from the data item as a string in 'YYYY-MM-DDTHH:mm' format.
   * Returns an empty string if the property is not set.
   * @param field The property key of the date-time field in the data item.
   * @returns The date-time as a string in 'YYYY-MM-DDTHH:mm' format, or an empty string if not set.
   */
  public getDateTimeString<K extends keyof T>(field: K): string {
    const dateValue = this.dataItem && this.dataItem[field]
      ? new Date(this.dataItem[field] as any)
      : null;
    if (!dateValue) return '';
    const offset = dateValue.getTimezoneOffset();
    const localDate = new Date(dateValue.getTime() - offset * 60000);
    return localDate.toISOString().slice(0, 16);
  }
  
  /**
   * Sets the value of a date-time property on the data item from a string in 'YYYY-MM-DDTHH:mm' format.
   * If the value is empty, the property is set to null. Otherwise, it is converted to a Date object.
   * @param field The property key of the date-time field in the data item.
   * @param value The date-time as a string in 'YYYY-MM-DDTHH:mm' format, or an empty string.
   */
  public setDateTimeString<K extends keyof T>(field: K, value: string): void {
    if (!value) {
      this.dataItem[field] = null as any;  // Falls das Feld null akzeptieren soll
      return;
    }
    const date = new Date(value);
    this.dataItem[field] = date.toISOString() as any; // Typanpassung
  }

}
