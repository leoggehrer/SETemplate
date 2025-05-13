//@GeneratedCode
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiBaseService } from '@app-services/api-base.service';
import { environment } from '@environment/environment';
import { ICompany } from '@app-models/entities/develop/i-company';
//@CustomImportBegin
//@CustomImportEnd
@Injectable({
  providedIn: 'root',
})
export class CompanyService extends ApiBaseService<ICompany> {
  constructor(public override http: HttpClient) {
    super(http, environment.API_BASE_URL + '/companies');
  }
//@CustomCodeBegin
//@CustomCodeEnd
}
