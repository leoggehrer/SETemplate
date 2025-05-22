//@CodeCopy
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Directive } from "@angular/core";
import { IdType, IKeyModel } from "@app/models/i-key-model";
import { IApiEntityBaseService } from "@app/services/i-api-entity-base.service";
import { MessageBoxService } from "@app/services/message-box-service.service";
import { GenericBaseListComponent } from "./generic-base-list.component";
import { Observable } from "rxjs";

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
  constructor(
    protected override modal: NgbModal,
    protected entityService: IApiEntityBaseService<T>,
    protected override messageBoxService: MessageBoxService) {
    super(modal, entityService, messageBoxService);
  }

  /**
   * Indicates whether the search functionality is enabled for the entity list.
   * Always returns true, allowing search operations.
   */
  public override get canSearch(): boolean {
    return true;
  }

  /**
   * Indicates whether the add functionality is enabled for the entity list.
   * Always returns true, allowing add operations.
   */
  public override get canAdd(): boolean {
    return true;
  }

  /**
   * Indicates whether the edit functionality is enabled for the entity list.
   * Always returns true, allowing edit operations.
   */
  public override get canEdit(): boolean {
    return true;
  }

  /**
   * Indicates whether the delete functionality is enabled for the entity list.
   * Always returns true, allowing delete operations.
   */
  public override get canDelete(): boolean {
    return true;
  }

  /**
   * Retrieves the unique key (id) for the given item.
   * 
   * @param item - The entity item from which to extract the key.
   * @returns The unique identifier of the item.
   */
  protected override getItemKey(item: T): IdType {
    return item.id;
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
    return this.entityService.delete(item);
  }
}
