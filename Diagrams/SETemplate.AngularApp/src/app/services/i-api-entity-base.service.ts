//@CodeCopy
import { IdType, IKeyModel } from "@app/models/i-key-model";
import { Observable } from "rxjs";
import { IApiQueryBaseService } from "./i-api-query-base.service";

export interface IApiEntityBaseService<T extends IKeyModel> extends IApiQueryBaseService<T> {

  hasCurrentUserPermission(actionName: string): Observable<boolean>;
  
  getItemKey(item: T): IdType;
  getTemplate(): Observable<T>;
  getById(id: IdType): Observable<T>;

  create(item: T): Observable<T>;
  update(item: T): Observable<T>;
  delete(item: T): Observable<any>;
  deleteById(id: IdType): Observable<any>;
}
