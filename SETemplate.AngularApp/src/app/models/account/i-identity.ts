//@CodeCopy
import { IdType } from '@app-models/i-key-model';

export interface IIdentity extends IKeyModel {
    name: string;
    email: string;
    timeOutInMinutes: number;
    state: number;
}
