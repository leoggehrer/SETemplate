//@CodeCopy
import { Injectable } from '@angular/core';
import { environment } from '@environment/environment';
import { ILogon } from '@app-models/account/i-logon';
import { IAuthenticatedUser } from '@app-models/account/i-authenticated-user';
import { AccountService } from '@app-services/http/base/account.service';
import { StorageService } from '@app-services/storage.service';
import { StorageLiterals } from '@app/literals/storage-literals';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _user?: IAuthenticatedUser;
  public authenticatedUserChanged = new BehaviorSubject(this._user);

  public get user(): IAuthenticatedUser | undefined {
    return this._user;
  }

  public get isLoginRequired(): boolean {
    return environment.loginRequired;
  }

  public get isLoggedIn(): boolean {
    return this._user != null;
  }

  public isAuthenticated: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    this._user != null
  );

  constructor(
    private accountService: AccountService,
    private storageService: StorageService) {
    this.loadUserFromStorage();
  }

  public async login(email: string, password: string): Promise<IAuthenticatedUser> {
    const logonData = {
      email: email,
      password: password,
    } as ILogon;

    this._user = await this.accountService.login(logonData);
    if (this._user) {
      this.updateUserInStorage(this._user);
      this.notifyForUserChanged();
    }
    return this._user;
  }

  public async logout() {
    try {
      if (this._user) {
        await this.accountService.logout(this._user.sessionToken);
      }
    }
    finally {
      this.removeUserFromStorage();
      this._user = undefined;
      this.notifyForUserChanged();
    }
  }

  /**
   * Prüft, ob der Nutzer genau die übergebene Rolle (als string) besitzt.
   */
  public hasRole(role: string): boolean {
    if (!this._user || !Array.isArray(this._user.roles)) {
      return false;
    }

    return this._user.roles
      .some(r => r.designation.toLowerCase() === role.toLowerCase());
  }

  /**
   * Prüft, ob der Nutzer mindestens eine der übergebenen Rollen besitzt.
   * Erwartet, dass jede Rolle als eigener String übergeben wird:
   *   hasAnyRole('Admin', 'Moderator')
   * Wenn du stattdessen schon ein Array hast, dann rufe es so auf:
   *   hasAnyRole(...meinArrayVonRollen)
   */
  public hasAnyRole(...roles: string[]): boolean {
    return roles.some(role => this.hasRole(role));
  }

  public async isSessionAlive(): Promise<boolean> {
    if (this._user) {
      return this.accountService.isSessionAlive(this._user.sessionToken);
    }
    return false;
  }

  public async requestPassword(email: string): Promise<any> {
    var res = await this.accountService.requestPassword(email);

    return res;
  }

  public async changePassword(oldPassword: string, newPassword: string): Promise<any> {
    var res = await this.accountService.changePassword(
      oldPassword,
      newPassword
    );

    return res;
  }

  public resetUser() {
    this._user = undefined;
    this.removeUserFromStorage();
    this.notifyForUserChanged();
  }

  private notifyForUserChanged() {
    this.authenticatedUserChanged.next(this._user);
    this.isAuthenticated.next(!!this._user);
  }

  private loadUserFromStorage() {
    this._user = this.storageService.getData(StorageLiterals.USER) as IAuthenticatedUser;
    this.notifyForUserChanged();
  }

  private updateUserInStorage(user: IAuthenticatedUser) {
    return this.storageService.setData(StorageLiterals.USER, user);
  }

  private removeUserFromStorage() {
    return this.storageService.remove(StorageLiterals.USER);
  }
}
