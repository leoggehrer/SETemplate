import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GenericEditComponent } from '../base/generic-edit/generic-edit.component';

@Component({
    selector: 'app-item-edit',
    imports: [CommonModule, FormsModule],
    templateUrl: './item-edit.component.html',
    styleUrl: './item-edit.component.css'
})
export class ItemEditComponent extends GenericEditComponent<IItem> {

    constructor(
        public override activeModal: NgbActiveModal) {
        super(activeModal);
    }

    public override get title(): string {
        return 'Item ' + super.title;
    }
}
