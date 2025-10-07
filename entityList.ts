import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IKeyModel } from '@app/models/i-key-model';
import { MessageBoxService } from '@app/services/message-box-service.service';
import { GenericEntityListComponent } from '@app/components/base/generic-entity-list.component';

/* Ersetzen Sie hier 'IKey' durch den gewuenschten 'Type' (z.B.: 'IAlbum') */
interface IItem extends IKeyModel {

}
/* Ersetzen Sie in Component 'item' durch den gewuenschten Namen (z.B.: album) */
@Component({
    selector: 'app-item-list',
    imports: [CommonModule, FormsModule],
    templateUrl: './item-list.component.html',
    styleUrl: './item-list.component.css'
})
export class ItemListComponent extends GenericEntityListComponent<IItem> implements OnInit {

    constructor(
        protected override modal: NgbModal,
        protected dataAccessService: DataAccessService,
        protected override messageBoxService: MessageBoxService) {
        super(modal, dataAccessService, messageBoxService);
    }

    ngOnInit(): void {
        // Passen Sie hier den Filter entsprechend an
        this._queryParams.filter = 'name.Contains(@0)';
        this.reloadData();
    }

    /* 
    *  Passen Sie hier den Titel fuer die Ueberschtsseite an.
    *  Default: Items
    */
    public override get pageTitle(): string {
        return super.pageTitle;
    }

    /* 
    *  Passen Sie hier den Titel fuer die Loeschbestaetigung an.
    *  Default: id
    */
    public override getItemTitel(item: IItem): string {
        return super.getItemTitel(item);
    }

    /* 
    *  Hier k�nnen Sie die Sortierung der Anzeige anpassen
    *  z.B.: return items.sort((a, b) => a.name.localeCompare(b.name));
    *  Default: keine Sortierung
    */
    protected override sortData(items: IItem[]): IItem[] {
        return super.sortData(items);
    }

    /*
    *  Geben Sie hier die Komponente fuer das Bearbeiten eines Eintrages an.
    *  (z.B.: AlbumEditComponent)
    *  Default: keine Komponente
    */
    protected override getEditComponent() {
        return ItemEditComponent;
    }
}

