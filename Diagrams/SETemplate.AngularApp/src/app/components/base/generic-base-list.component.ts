//@CodeCopy
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Directive } from "@angular/core";
import { IdType } from "../../models/i-key-model";
import { IModel } from "@app/models/i-model";
import { IQueryParams } from "@app/models/base/i-query-params";
import { IApiQueryBaseService } from "@app/services/i-api-query-base.service";
import { MessageBoxService } from "@app/services/message-box-service.service";
import { Observable } from "rxjs";

/**
 * A generic list component for managing a collection of items of type T.
 * Provides functionality for searching, adding, editing, and deleting items.
 * 
 * @template T - A type that extends the IKey interface.
 */
@Directive()
export abstract class GenericBaseListComponent<T extends IModel> {
  /**
   * The list of data items displayed in the component.
   */
  public dataItems: T[] = [];

  /**
   * The current search term used for filtering the data items.
   */
  protected _searchTerm: string = '';

  /**
   * The query parameters used for filtering and querying data items.
   */
  protected _queryParams: IQueryParams = {
    filter: '',
    values: []
  };

  /**
   * Constructor for the GenericListComponent.
   * 
   * @param modal - The modal service for opening modals.
   * @param messageBoxService - The service for displaying message boxes.
   */
  constructor(
    protected modal: NgbModal,
    protected viewService: IApiQueryBaseService<T>,
    protected messageBoxService: MessageBoxService) { }

  /**
      * Gets the current search term.
  */
  public get pageTitle(): string {
    return 'Items';
  }

  /**
   * Gets the current search term.
   */
  public get searchTerm(): string {
    return this._searchTerm;
  }

  /**
   * Sets the search term and reloads the data based on the new term.
   * 
   * @param value - The new search term.
   */
  public set searchTerm(value: string) {
    this._searchTerm = value;
    this._queryParams.values = value ? [value.toLocaleLowerCase()] : [];
    this.reloadData();
  }

  /**
   * Indicates whether the search functionality is enabled for the list component.
   * Subclasses can override this property to enable or disable search.
   * 
   * @returns True if search is enabled; otherwise, false.
   */
  public get canSearch(): boolean {
    return true;
  }

  /**
   * Indicates whether the refresh functionality is enabled for the list component.
   * Subclasses can override this property to enable or disable refresh.
   * 
   * @returns True if refresh is enabled; otherwise, false.
   */
  public get canRefresh(): boolean {
    return true;
  }

  /**
   * Indicates whether the add functionality is enabled for the list component.
   * Subclasses can override this property to enable or disable add.
   * 
   * @returns True if add is enabled; otherwise, false.
   */
  public get canAdd(): boolean {
    return false;
  }

  /**
   * Indicates whether the edit functionality is enabled for the list component.
   * Subclasses can override this property to enable or disable edit.
   * 
   * @returns True if edit is enabled; otherwise, false.
   */
  public get canEdit(): boolean {
    return false;
  }

  /**
   * Indicates whether the delete functionality is enabled for the list component.
   * Subclasses can override this property to enable or disable delete.
   * 
   * @returns True if delete is enabled; otherwise, false.
   */
  public get canDelete(): boolean {
    return false;
  }

  /**
   * Gets the unique key of an item.
   * Subclasses should override this method to provide the logic for retrieving the item's key.
   * 
   * @param item - The item for which to get the key.
   * @returns The unique identifier of the item.
   */
  protected getItemKey(item: T): IdType {
    return this.getDefaultValue<IdType>();
  }

  /**
   * Returns the default value for a given type T.
   * - For number: returns 0
   * - For boolean: returns false
   * - For string: returns an empty string
   * - For all other types: returns null
   * 
   * @template T - The type for which to get the default value.
   * @returns The default value for the specified type.
   */
  protected getDefaultValue<T>(): T {
    if (typeof (null as any as T) === "number") return 0 as T;
    if (typeof (null as any as T) === "boolean") return false as T;
    if (typeof (null as any as T) === "string") return "" as T;
    return null as unknown as T; // fallback
  }

  /**
   * Reloads the data items based on the current query parameters.
   */
  protected reloadData() {
    if (this._queryParams.values.length === 0) {
      this.viewService.getAll()
        .subscribe(data => {
          this.dataItems = this.sortData(data);
        });
    } else {
      this.viewService.query(this._queryParams)
        .subscribe(data => {
          this.dataItems = this.sortData(data);
        });
    }
  }

  /**
   * Sorts the data items. Can be overridden by subclasses to provide custom sorting logic.
   * 
   * @param items - The data items to sort.
   * @returns The sorted data items.
   */
  protected sortData(items: T[]): T[] {
    return items;
  }

  /**
  * Gets the title of an item.
  * This default implementation returns a static string.
  * Subclasses should override this method to return a meaningful title for the given item.
  * 
  * @param item - The item for which to get the title.
  * @returns The title of the item as a string.
  */
  public getItemTitel(item: T): string {
    const keys = Object.keys(item) as Array<keyof T>;
    let result = 'Titel';

    if (keys.length > 0) {
      const key = keys[0];
      const value = item[key];

      if (value !== undefined && value !== null) {
        result = value.toString();
      }
    }
    return result;
  }

  /**
   * Opens a modal for adding a new item.
   */
  public addCommand() {
    const modalRef = this.modal.open(this.getEditComponent(), {
      size: 'lg',
      centered: true
    });
    const comp = modalRef.componentInstance;

    comp.dataItem = { id: 0, name: '' };
    comp.save.subscribe((item: T) => {
      this.addItem(item)
        .subscribe({
          next: () => {
            comp.close();
            this.reloadData();
          },
          error: err => {
            this.messageBoxService.show(
              err.error,
              'Fehler beim Erstellen',
              'OK'
            );
          }
        });
    });
  }

  /**
   * Opens a modal for editing an existing item.
   * 
   * @param item - The item to edit.
   */
  public editCommand(item: T) {
    const modalRef = this.modal.open(this.getEditComponent(), {
      size: 'lg',
      centered: true
    });
    const comp = modalRef.componentInstance;

    this.queryItem(this.getItemKey(item))
      .subscribe(item => {
        comp.dataItem = { ...item };
        comp.save.subscribe((updated: T) => {
          this.updateItem(updated)
            .subscribe({
              next: () => {
                comp.close();
                this.reloadData();
              },
              error: err => {
                this.messageBoxService.show(
                  err.error,
                  'Fehler beim Speichern',
                  'OK'
                );
              }
            });
        });
      });
  }

  /**
   * Deletes an item after confirming the action with the user.
   * 
   * @param item - The item to delete.
   */
  public async deleteCommand(item: T) {
    const confirmed = await this.messageBoxService.confirm(
      `Möchten Sie den Eintrag '${this.getItemTitel(item)}' löschen?`,
      'Löschen bestätigen'
    );
    if (confirmed) {
      this.deleteItem(item)
        .subscribe({
          next: () => this.reloadData(),
          error: err => {
            this.messageBoxService.show(
              err.error,
              'Fehler beim Löschen',
              'OK'
            );
          }
        });
    }
  }

  /**
 * Gets the component used for editing items. Must be implemented by subclasses.
 */
  protected getEditComponent(): any {
    throw new Error("getEditComponent() must be implemented in a subclass.");
  }

  /**
   * Queries a single item by its key.
   * Must be implemented by subclasses to provide the logic for retrieving an item.
   * 
   * @param key - The unique identifier of the item to query.
   * @returns An Observable that emits the queried item.
   */
  protected queryItem(key: IdType): Observable<any> {
    throw new Error("queryItem(key) must be implemented in a subclass.");
  }

  /**
   * Adds a new item to the data source.
   * Must be implemented by subclasses to provide the logic for adding an item.
   * 
   * @param item - The item to add.
   * @returns An Observable that emits the added item.
   */
  protected addItem(item: any): Observable<any> {
    throw new Error("addItem(item) must be implemented in a subclass.");
  }

  /**
   * Updates an existing item in the data source.
   * Must be implemented by subclasses to provide the logic for updating an item.
   * 
   * @param item - The item to update.
   * @returns An Observable that emits the updated item.
   */
  protected updateItem(item: any): Observable<any> {
    throw new Error("updateItem(item) must be implemented in a subclass.");
  }

  /**
   * Deletes an item from the data source.
   * Must be implemented by subclasses to provide the logic for deleting an item.
   * 
   * @param item - The item to delete.
   * @returns An Observable that emits the result of the delete operation.
   */
  protected deleteItem(item: any): Observable<any> {
    throw new Error("deleteItem(item) must be implemented in a subclass.");
  }
}
