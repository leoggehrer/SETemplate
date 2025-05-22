import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environment/environment';
import { ILogon as ILogon } from '@app-models/account/i-logon';
import { IAuthenticatedUser as IAuthenticatedUser } from '@app-models/account/i-authenticated-user';
import { stringToDate } from '@app/converter/date-converter';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private BASE_URL = environment.API_BASE_URL + '/accounts';

  constructor(private httpClient: HttpClient) {
  }

  async login(logonData: ILogon): Promise<IAuthenticatedUser> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/logon',
        logonData
      )
    ).then((user) => {
      stringToDate(user);
      return user;
    })
    .catch((error) => {
      console.error('Login failed:', error);
      throw new Error('Login failed');
    });
  }

  async refreshLogin(): Promise<IAuthenticatedUser> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/refreshLogin',
        null
      )
    ).then((user) => {
      stringToDate(user);
      return user;
    });
  }

  async logout(): Promise<void> {
    return firstValueFrom(
      this.httpClient.post<void>(this.BASE_URL + '/logoutUser', null)
    );
  }

  async requestPassword(email: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<boolean>(this.BASE_URL + '/requestPassword', {
        email: email,
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

  async requestEmailChange(email: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<boolean>(this.BASE_URL + '/requestEmailChange', {
        email: email,
      })
    );
  }

  async requestEmailChangeForUser(
    forUserEmail: string,
    email: string
  ): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<boolean>(this.BASE_URL + '/requestEmailChange', {
        forUserEmail: forUserEmail,
        email: email,
      })
    );
  }
}
