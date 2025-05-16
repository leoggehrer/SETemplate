//@CodeCopy
import { IViewEntity } from "../models/i-view-entity";
import { IQueryParams } from "@app/models/base/i-query-params";
import { Observable } from "rxjs";

export interface IApiViewBaseService<T extends IViewEntity> {
  getCount(): Observable<number>;
  getAll(): Observable<T[]>;
  query(params: IQueryParams): Observable<T[]>;
}
