//@CustomCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { VersionEntityObjectBaseEditComponent }from '@app/components/entities/version-entity-object-base-edit.component';
//@CustomImportBegin
//@CustomImportEnd
@Component({
  selector:'app-version-entity-object-edit',
  imports: [ CommonModule, FormsModule, TranslateModule],
  templateUrl: './version-entity-object-edit.component.html',
  styleUrl: './version-entity-object-edit.component.css'
})
export class VersionEntityObjectEditComponent extends VersionEntityObjectBaseEditComponent {
//@CustomCodeBegin
//@CustomCodeEnd
}
