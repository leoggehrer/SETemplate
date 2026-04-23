//@BaseCode
import { Directive } from "@angular/core";
import { IdType, IKeyModel } from "@app/models/i-key-model";
import { IApiEntityBaseService } from "@app/services/i-api-entity-base.service";
import { GenericBaseListComponent } from "./generic-base-list.component";
import { Observable, take } from "rxjs";

/**
 * A generic list component for managing a collection of items of type T.
 * Provides functionality for searching, adding, editing, and deleting items.
 * 
 * @template T - A type that extends the IKey interface.
 */
@Directive()
export abstract class GenericEntityListComponent<T extends IKeyModel> extends GenericBaseListComponent<T> {
  /**
   * Constructor for the GenericListComponent.
   * 
   * @param modal - The modal service for opening modals.
   * @param messageBoxService - The service for displaying message boxes.
   */
  constructor(protected entityService: IApiEntityBaseService<T>) {
    super(entityService);
  }

  /**
   * Initializes the component and sets up permissions for various operations.
   */
  public override ngOnInit(): void {
    super.ngOnInit();

    this.entityService.hasCurrentUserPermission("Query").pipe(take(1)).subscribe(permission => {
      this._canSearch = permission;
      this.afterSetPermissions("Query");
    });
    this.entityService.hasCurrentUserPermission("Create").pipe(take(1)).subscribe(permission => {
      this._canAdd = permission;
      this.afterSetPermissions("Create");
    });
    this.entityService.hasCurrentUserPermission("Update").pipe(take(1)).subscribe(permission => {
      this._canEdit = permission;
      this.afterSetPermissions("Update");
    });
    this.entityService.hasCurrentUserPermission("Delete").pipe(take(1)).subscribe(permission => {
      this._canDelete = permission;
      this.afterSetPermissions("Delete");
    });
  }

  /**
   * Method called after permissions are set for an operation.
   * Can be overridden in derived classes to implement custom logic.
   * 
   * @param permission - The permission type that was set.
   */
  protected afterSetPermissions(permission: string): void {
    // Override this method to implement custom logic after permissions are set
  }

  /**
   * Retrieves a new instance of the item type T.
   * This method is used to create a template for new items.
   * 
   * @returns An Observable emitting a new instance of the item type T.
   */
  protected override getTemplate(): Observable<any> {
    return this.entityService.getTemplate();
  }

  /**
   * Queries a single entity item by its unique key.
   * 
   * @param key - The unique identifier of the entity to retrieve.
   * @returns An Observable emitting the entity item corresponding to the given key.
   */
  protected override queryItem(key: IdType): Observable<any> {
    return this.entityService.getById(key);
  }

  /**
   * Adds a new entity item to the collection.
   * 
   * @param item - The entity item to add.
   * @returns An Observable emitting the created entity item.
   */
  protected override addItem(item: any): Observable<any> {
    return this.entityService.create(item);
  }

  /**
   * Updates an existing entity item in the collection.
   * 
   * @param item - The entity item to update.
   * @returns An Observable emitting the updated entity item.
   */
  protected override updateItem(item: any): Observable<any> {
    return this.entityService.update(item);
  }

  /**
   * Deletes an entity item from the collection.
   * 
   * @param item - The entity item to delete.
   * @returns An Observable emitting the result of the delete operation.
   */
  protected override deleteItem(item: any): Observable<any> {
    return this.entityService.deleteById(this.getItemKey(item));
  }
}
