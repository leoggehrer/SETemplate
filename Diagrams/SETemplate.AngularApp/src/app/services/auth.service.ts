//@CodeCopy
import { Injectable } from '@angular/core';
import { ILogon } from '@app-models/account/i-logon';
import { IAuthenticatedUser } from '@app-models/account/i-authenticated-user';
import { AccountService } from '@app-services/http/account.service';
import { StorageService } from '@app-services/storage.service';
import { StorageLiterals } from '@app/literals/storage-literals';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public user?: IAuthenticatedUser;

  authenticatedUserChanged = new BehaviorSubject(this.user);
  isAuthenticated: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    this.user != null
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

    this.user = await this.accountService.login(logonData);
    if (this.user) {
      this.updateUserInStorage(this.user);
      this.notifyForUserChanged();
    }
    return this.user;
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

  public async logout() {
    this.removeUserFromStorage();
    this.user = undefined;
    this.notifyForUserChanged();
  }

  private notifyForUserChanged() {
    this.authenticatedUserChanged.next(this.user);
    this.isAuthenticated.next(!!this.user);
  }

  private loadUserFromStorage() {
    this.user = this.storageService.getData(
      StorageLiterals.USER
    ) as IAuthenticatedUser;
    this.notifyForUserChanged();
  }

  private updateUserInStorage(user: IAuthenticatedUser) {
    return this.storageService.setData(StorageLiterals.USER, user);
  }

  private removeUserFromStorage() {
    return this.storageService.remove(StorageLiterals.USER);
  }
}
