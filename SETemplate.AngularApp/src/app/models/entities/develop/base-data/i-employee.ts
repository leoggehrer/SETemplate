//@GeneratedCode
import { IVersionEntity } from '@app-models/i-version-entity';
import { ICompany } from '@app-models/entities/develop/i-company';
//@CustomImportBegin
//@CustomImportEnd
export interface IEmployee extends IVersionEntity {
  companyId: number;
  firstName: string;
  lastName: string;
  email: string;
  company: ICompany;
  id: number;
//@CustomCodeBegin
//@CustomCodeEnd
}
