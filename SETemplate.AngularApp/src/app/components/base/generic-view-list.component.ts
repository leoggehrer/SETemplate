//@BaseCode
import { Directive, OnInit } from "@angular/core";
import { IViewModel } from "@app/models/i-view-model";
import { IApiViewBaseService } from "@app/services/i-api-view-base.service";
import { GenericBaseListComponent } from "./generic-base-list.component";

/**
 * A generic list component for managing a collection of items of type T.
 * Provides functionality for searching, adding, editing, and deleting items.
 * 
 * @template T - A type that extends the IKey interface.
 */
@Directive()
export abstract class GenericViewListComponent<T extends IViewModel> extends GenericBaseListComponent<T> implements OnInit {

  /**
   * Constructor for the GenericListComponent.
   * 
   * @param modal - The modal service for opening modals.
   * @param messageBoxService - The service for displaying message boxes.
   */
  constructor(protected override viewService: IApiViewBaseService<T>) {
    super(viewService);
  }

  /**
   * Initializes the component.
   */
  ngOnInit(): void {
  }
}
