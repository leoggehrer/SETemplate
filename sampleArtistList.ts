import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IArtist } from '@app/models/entities/i-artist';
import { ArtistService } from '@app/services/http/entities/artist-service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageBoxService } from '@app/services/message-box-service.service';
import { GenericListComponent } from '@app/components/base/generic-list.component';
import { IQueryParams } from '@app/models/base/i-query-params';
import { Observable } from 'rxjs';
import { ArtistEditComponent } from '@app/components/artist-edit/artist-edit.component';

@Component({
  selector: 'app-artist-list',
  imports: [CommonModule, FormsModule ],
  templateUrl: './artist-list.component.html',
  styleUrl: './artist-list.component.css'
})
export class ArtistListComponent extends GenericListComponent<IArtist> implements OnInit {

  constructor(
    protected override modal: NgbModal,
    private artistService: ArtistService,
    protected override messageBoxService: MessageBoxService) {
      super(modal, messageBoxService);
  }

  ngOnInit(): void {
    this._queryParams.filter = 'name.Contains(@0)';
    this.reloadData();
  }

  protected override sortData(items: IArtist[]): IArtist[] {
    return items.sort((a, b) => a.name.localeCompare(b.name));
  }
  protected override getAllItems() {
    return this.artistService.getAll();
  }
  protected override queryItems(params: IQueryParams): Observable<IArtist[]> {
    return this.artistService.query(params);
  }
  protected override getItemTitel(item: IArtist): string {
    return item.name;
  }
  protected override createAction(item: IArtist): Observable<IArtist> {
    return this.artistService.create(item);
  }
  protected override updateAction(item: IArtist): Observable<IArtist> {
    return this.artistService.update(item);
  }
  protected override deleteAction(item: IArtist): Observable<any> {
    return this.artistService.delete(item);
  }
  protected override getEditComponent() {
    return ArtistEditComponent;
  }
}

