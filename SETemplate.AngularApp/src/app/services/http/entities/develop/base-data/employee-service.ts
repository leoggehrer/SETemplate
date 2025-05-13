//@GeneratedCode
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiBaseService } from '@app-services/api-base.service';
import { environment } from '@environment/environment';
import { IEmployee } from '@app-models/entities/develop/base-data/i-employee';
//@CustomImportBegin
//@CustomImportEnd
@Injectable({
  providedIn: 'root',
})
export class EmployeeService extends ApiBaseService<IEmployee> {
  constructor(public override http: HttpClient) {
    super(http, environment.API_BASE_URL + '/employees');
  }
//@CustomCodeBegin
//@CustomCodeEnd
}
