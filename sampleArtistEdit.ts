import { Component, Input, Output } from '@angular/core';
import { IArtist } from '@app/models/entities/i-artist';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GenericEditComponent } from '../base/generic-edit/generic-edit.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-artist-edit',
    imports: [CommonModule, FormsModule],
    templateUrl: './artist-edit.component.html',
    styleUrl: './artist-edit.component.css'
})
export class ArtistEditComponent extends GenericEditComponent<IArtist> {

    constructor(
        public override activeModal: NgbActiveModal) {
        super(activeModal);
    }

    public override get title(): string {
        return 'Artist ' + super.title;
    }
}
