//@CodeCopy
import { IViewModel } from "../models/i-view-model";
import { IApiQueryBaseService } from "./i-api-query-base.service";

export interface IApiViewBaseService<T extends IViewModel> extends IApiQueryBaseService<T> {
}
