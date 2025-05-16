//@CodeCopy
import { IdType, IKey } from "@app/models/i-key";
import { IQueryParams } from "@app/models/base/i-query-params";
import { Observable } from "rxjs";

export interface IApiEntityBaseService<T extends IKey> {
  getCount(): Observable<number>;
  getAll(): Observable<T[]>;
  getById(id: number): Observable<T>;
  query(params: IQueryParams): Observable<T[]>;
  create(item: T): Observable<T>;
  update(item: T): Observable<T>;
  delete(item: T): Observable<any>;
  deleteById(id: IdType): Observable<any>;
}
