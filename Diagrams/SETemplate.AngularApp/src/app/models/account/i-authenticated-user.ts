//@CodeCopy
import { IdType } from '@app-models/i-key-model';

export interface IAuthenticatedUser {
  identityId: IdType;
  sessionToken: string;
  name: string;
  email: string;
  roles: any[];
}
