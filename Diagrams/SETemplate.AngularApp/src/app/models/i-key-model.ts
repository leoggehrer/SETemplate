//@CodeCopy
import { IModel } from "./i-model";

export type IdType = number;
export interface IKeyModel extends IModel {
  id: IdType;
}
