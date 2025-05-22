//@CodeCopy
import { IModel } from "../models/i-model";
import { IQueryParams } from "@app/models/base/i-query-params";
import { Observable } from "rxjs";

export interface IApiQueryBaseService<T extends IModel> {
  getCount(): Observable<number>;
  getAll(): Observable<T[]>;
  query(params: IQueryParams): Observable<T[]>;
}
