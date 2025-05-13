import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageBoxService } from '@app/services/message-box-service.service';
import { GenericListComponent } from '@app/components/base/generic-list.component';
import { IQueryParams } from '@app/models/base/i-query-params';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-item-list',
  imports: [CommonModule, FormsModule ],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent extends GenericListComponent<IItem> implements OnInit {

  constructor(
    protected override modal: NgbModal,
    private dataAccessService: DataAccessService,
    protected override messageBoxService: MessageBoxService) {
      super(modal, messageBoxService);
  }

  ngOnInit(): void {
    this._queryParams.filter = 'name.Contains(@0)';
    this.reloadData();
  }

  protected override sortData(items: IItem[]): IItem[] {
    return items.sort((a, b) => a.name.localeCompare(b.name));
  }
  protected override getAllItems() {
    return this.dataAccessService.getAll();
  }
  protected override queryItems(params: IQueryParams): Observable<IItem[]> {
    return this.dataAccessService.query(params);
  }
  protected override getItemTitel(item: IItem): string {
    return item.name;
  }
  protected override createAction(item: IItem): Observable<IItem> {
    return this.dataAccessService.create(item);
  }
  protected override updateAction(item: IItem): Observable<IItem> {
    return this.dataAccessService.update(item);
  }
  protected override deleteAction(item: IItem): Observable<any> {
    return this.dataAccessService.delete(item);
  }
  protected override getEditComponent() {
    return ItemEditComponent;
  }
}

