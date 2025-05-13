//@CodeCopy
import { IdType } from '@app-models/i-key';

export interface AuthenticatedUser {
  identityId: IdType;
  sessionToken: string;
  jsonWebToken: string;
  name: string;
  email: string;
  roles: any[];
  roleLevel: number;
  contentLanguage: string;
}
