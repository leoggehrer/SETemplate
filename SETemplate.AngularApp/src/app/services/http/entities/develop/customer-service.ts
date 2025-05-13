//@GeneratedCode
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiBaseService } from '@app-services/api-base.service';
import { environment } from '@environment/environment';
import { ICustomer } from '@app-models/entities/develop/i-customer';
//@CustomImportBegin
//@CustomImportEnd
@Injectable({
  providedIn: 'root',
})
export class CustomerService extends ApiBaseService<ICustomer> {
  constructor(public override http: HttpClient) {
    super(http, environment.API_BASE_URL + '/customers');
  }
//@CustomCodeBegin
//@CustomCodeEnd
}
