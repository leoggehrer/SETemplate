//@GeneratedCode
import { IVersionEntity } from '@app-models/i-version-entity';
import { ICustomer } from '@app-models/entities/develop/i-customer';
import { IEmployee } from '@app-models/entities/develop/base-data/i-employee';
//@CustomImportBegin
//@CustomImportEnd
export interface ICompany extends IVersionEntity {
  name: string;
  address: string;
  description: string;
  customers: Customer[];
  employees: Employee[];
  id: number;
//@CustomCodeBegin
//@CustomCodeEnd
}
