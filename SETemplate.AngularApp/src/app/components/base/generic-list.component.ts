//@CodeCopy
import { IQueryParams } from "@app/models/base/i-query-params";
import { IKey } from "@app/models/i-key";
import { MessageBoxService } from "@app/services/message-box-service.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Observable } from "rxjs";

/**
 * A generic list component for managing a collection of items of type T.
 * Provides functionality for searching, adding, editing, and deleting items.
 * 
 * @template T - A type that extends the IKey interface.
 */
export abstract class GenericListComponent<T extends IKey> {
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
        protected messageBoxService: MessageBoxService) { }

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
     * Reloads the data items based on the current query parameters.
     */
    protected reloadData() {
        if (this._queryParams.values.length === 0) {
            this.getAllItems().subscribe(data => {
                this.dataItems = this.sortData(data);
            });
        } else {
            this.queryItems(this._queryParams).subscribe(data => {
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
     * Opens a modal for adding a new item.
     */
    public addItem() {
        const modalRef = this.modal.open(this.getEditComponent(), {
            size: 'lg',
            centered: true
        });
        const comp = modalRef.componentInstance;
        comp.dataItem = { id: 0, name: '' };

        comp.save.subscribe((item: T) => {
            this.createAction(item).subscribe({
                next: () => {
                    comp.close();
                    this.reloadData();
                },
                error: err => {
                    this.messageBoxService.show(
                        'Erstellung fehlgeschlagen:\n' + err.error,
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
    public editItem(item: T) {
        const modalRef = this.modal.open(this.getEditComponent(), {
            size: 'lg',
            centered: true
        });
        const comp = modalRef.componentInstance;
        comp.dataItem = { ...item };

        comp.save.subscribe((updated: T) => {
            this.updateAction(updated).subscribe({
                next: () => {
                    comp.close();
                    this.reloadData();
                },
                error: err => {
                    this.messageBoxService.show(
                        'Speichern fehlgeschlagen:\n' + err.error,
                        'Fehler beim Speichern',
                        'OK'
                    );
                }
            });
        });
    }

    /**
     * Deletes an item after confirming the action with the user.
     * 
     * @param item - The item to delete.
     */
    public async deleteItem(item: T) {
        const confirmed = await this.messageBoxService.confirm(
            `Möchten Sie '${this.getItemTitel(item)}' wirklich löschen?`,
            'Löschen bestätigen'
        );
        if (confirmed) {
            this.deleteAction(item).subscribe({
                next: () => this.reloadData(),
                error: err => {
                    this.messageBoxService.show(
                        'Löschen fehlgeschlagen:\n' + err.error,
                        'Fehler beim Löschen',
                        'OK'
                    );
                }
            });
        }
    }

    // 🧩 Abstract Members

    /**
     * Retrieves all items. Must be implemented by subclasses.
     */
    protected abstract getAllItems(): Observable<T[]>;

    /**
     * Queries items based on the provided query parameters. Must be implemented by subclasses.
     * 
     * @param params - The query parameters.
     */
    protected abstract queryItems(params: IQueryParams): Observable<T[]>;

    /**
     * Gets the title of an item. Must be implemented by subclasses.
     * 
     * @param item - The item for which to get the title.
     */
    protected abstract getItemTitel(item: T): string;

    /**
     * Creates a new item. Must be implemented by subclasses.
     * 
     * @param item - The item to create.
     */
    protected abstract createAction(item: T): Observable<T>;

    /**
     * Updates an existing item. Must be implemented by subclasses.
     * 
     * @param item - The item to update.
     */
    protected abstract updateAction(item: T): Observable<T>;

    /**
     * Deletes an item. Must be implemented by subclasses.
     * 
     * @param item - The item to delete.
     */
    protected abstract deleteAction(item: T): Observable<any>;

    /**
     * Gets the component used for editing items. Must be implemented by subclasses.
     */
    protected abstract getEditComponent(): any;
}
