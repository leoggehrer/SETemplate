//@CodeCopy
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environment/environment';
import { ILogon } from '@app-models/account/i-logon';
import { IAuthenticatedUser } from '@app-models/account/i-authenticated-user';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private BASE_URL = environment.API_BASE_URL + '/accounts';

  constructor(private httpClient: HttpClient) {}

  async login(logonData: ILogon): Promise<IAuthenticatedUser> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/login',
        logonData
      )
    );
  }

  async requestPassword(email: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/requestPassword',
        {
          email: email,
        }
      )
    );
  }

  async setPassword(
    email: string,
    code: string,
    password: string
  ): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(this.BASE_URL + '/setPassword', {
        email: email,
        code: code,
        password: password,
      })
    );
  }

  async changePassword(oldPassword: string, newPassword: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/changePassword',
        {
          oldPassword: oldPassword,
          newPassword: newPassword,
        }
      )
    );
  }
}
