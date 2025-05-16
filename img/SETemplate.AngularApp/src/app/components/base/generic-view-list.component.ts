//@CodeCopy
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Directive } from "@angular/core";
import { IViewEntity } from "@app/models/i-view-entity";
import { IQueryParams } from "@app/models/base/i-query-params";
import { IApiViewBaseService } from "@app/services/i-api-view-base.service";
import { MessageBoxService } from "@app/services/message-box-service.service";

/**
 * A generic list component for managing a collection of items of type T.
 * Provides functionality for searching, adding, editing, and deleting items.
 * 
 * @template T - A type that extends the IKey interface.
 */
@Directive()
export abstract class GenericViewListComponent<T extends IViewEntity> {
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
    protected viewService: IApiViewBaseService<T>,
    protected messageBoxService: MessageBoxService) { }

  /**
   * Gets the current search term.
   */
  public get searchTerm(): string {
    return this._searchTerm;
  }

  /**
      * Gets the current search term.
  */
  public get pageTitle(): string {
    return 'Items';
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
   * Sorts the data items. Can be overridden by subclasses to provide custom sorting logic.
   * 
   * @param items - The data items to sort.
   * @returns The sorted data items.
   */
  protected sortData(items: T[]): T[] {
    return items;
  }

}
