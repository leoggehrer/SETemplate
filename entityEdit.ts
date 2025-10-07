import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IKeyModel } from '@app/models/i-key-model';
import { GenericEditComponent } from '@app/components/base/generic-edit.component';

/* Ersetzen Sie hier 'IKey' durch den gewuenschten 'Type' (z.B.: 'IAlbum') */
interface IItem extends IKeyModel {

}
/* Ersetzen Sie in Component 'item' durch den gewuenschten Namen (z.B.: album) */
@Component({
    selector: 'app-item-edit',
    imports: [CommonModule, FormsModule],
    templateUrl: './item-edit.component.html',
    styleUrl: './item-edit.component.css'
})
// Ersetzen Sie hier Item durch den gewuenschten Namen (z.B.: Item -> Album)
export class ItemEditComponent extends GenericEditComponent<IItem> {

    constructor(
        public override activeModal: NgbActiveModal) {
        super(activeModal);
    }

    /*
    *  Passen Sie hier den Titel fuer die Ueberschtsseite an.
    *  Default: Item Hinzuf�gen oder Item Berbeiten
    */
    public override get title(): string {
        return 'Item ' + super.title;
    }
}
