//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IVersionEntityObject } from '@app-models/entities/i-version-entity-object';
import { VersionEntityObjectBaseListComponent }from '@app/components/entities/version-entity-object-base-list.component';
import { VersionEntityObjectEditComponent }from '@app/components/entities/version-entity-object-edit.component';
import { VersionEntityObjectService } from '@app-services/http/entities/version-entity-object-service';
//@CustomImportBegin
//@CustomImportEnd
@Component({
  standalone: true,
  selector:'app-version-entity-object-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './version-entity-object-list.component.html',
  styleUrl: './version-entity-object-list.component.css'
})
export class VersionEntityObjectListComponent extends VersionEntityObjectBaseListComponent {
  constructor(protected override dataAccessService: VersionEntityObjectService)
  {
    super(dataAccessService);
  }
  override ngOnInit(): void {
    this._queryParams.filter = '';
    this.reloadData();
  }
  protected override getItemKey(item: IVersionEntityObject): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'VersionEntityObjects';
  }
  override getEditComponent() {
    return VersionEntityObjectEditComponent;
  }
//@CustomCodeBegin
//@CustomCodeEnd
}
