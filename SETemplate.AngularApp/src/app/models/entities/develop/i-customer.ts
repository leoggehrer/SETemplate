//@GeneratedCode
import { IVersionEntity } from '@app-models/i-version-entity';
import { ICompany } from '@app-models/entities/develop/i-company';
//@CustomImportBegin
//@CustomImportEnd
export interface ICustomer extends IVersionEntity {
  companyId: number;
  name: string;
  email: string;
  company: ICompany;
  id: number;
//@CustomCodeBegin
//@CustomCodeEnd
}
