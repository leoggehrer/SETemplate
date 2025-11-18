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
  /**
   * The currently authenticated user, if any.
   */
  private _user?: IAuthenticatedUser;

  /**
   * Emits the current authenticated user whenever it changes.
   */
  public authenticatedUserChanged = new BehaviorSubject(this._user);

  /**
   * Gets the current authenticated user.
   */
  public get user(): IAuthenticatedUser | undefined {
    return this._user;
  }

  /**
   * Indicates if login is required based on environment settings.
   */
  public get isLoginRequired(): boolean {
    return environment.loginRequired;
  }

  /**
   * Indicates if a user is currently logged in.
   */
  public get isLoggedIn(): boolean {
    return this._user != null;
  }

  /**
   * Emits the authentication state (true if logged in, false otherwise).
   */
  public isAuthenticated: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    this._user != null
  );

  constructor(
    private accountService: AccountService,
    private storageService: StorageService) {
    this.loadUserFromStorage();
  }

  /**
   * Attempts to log in with the provided email and password.
   * On success, updates the user state and storage.
   * @param email User's email address.
   * @param password User's password.
   * @returns The authenticated user.
   */
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

  /**
   * Logs out the current user, clears session and storage, and updates state.
   */
  public async logout() {
    try {
      if (this._user) {
        await this.accountService.logout(this._user.sessionToken);
      }
      // Optionally, you can reload the application to reset state
      window.location.reload();
    }
    finally {
      this.removeUserFromStorage();
      this._user = undefined;
      this.notifyForUserChanged();
    }
  }

  /**
   * Checks if the user has the specified role.
   * @param role The role to check.
   * @returns True if the user has the role, false otherwise.
   */
  public hasRole(role: string): boolean {
    if (!this._user || !Array.isArray(this._user.roles)) {
      return false;
    }

    return this._user.roles
      .some(r => r.designation.toLowerCase() === role.toLowerCase());
  }

  /**
   * Checks if the user has at least one of the specified roles.
   * @param roles The roles to check.
   * @returns True if the user has any of the roles, false otherwise.
   */
  public hasAnyRole(...roles: string[]): boolean {
    return roles.some(role => this.hasRole(role));
  }

  /**
   * Checks if the current session is still alive.
   * @returns True if the session is alive, false otherwise.
   */
  public async isSessionAlive(): Promise<boolean> {
    if (this._user) {
      return this.accountService.isSessionAlive(this._user.sessionToken);
    }
    return false;
  }

  /**
   * Requests a password reset for the given email.
   * @param email The user's email address.
   * @returns The result of the password request operation.
   */
  public async requestPassword(email: string): Promise<any> {
    var res = await this.accountService.requestPassword(email);

    return res;
  }

  /**
   * Changes the user's password.
   * @param oldPassword The current password.
   * @param newPassword The new password.
   * @returns The result of the password change operation.
   */
  public async changePassword(oldPassword: string, newPassword: string): Promise<any> {
    var res = await this.accountService.changePassword(
      oldPassword,
      newPassword
    );

    return res;
  }

  /**
   * Resets the user state and clears storage.
   */
  public resetUser() {
    this._user = undefined;
    this.removeUserFromStorage();
    this.notifyForUserChanged();
  }

  /**
   * Notifies subscribers about user and authentication state changes.
   */
  private notifyForUserChanged() {
    this.authenticatedUserChanged.next(this._user);
    this.isAuthenticated.next(!!this._user);
  }

  /**
   * Loads the user from storage and checks if the session is alive.
   */
  private async loadUserFromStorage() {
    this._user = this.storageService.getData(StorageLiterals.USER) as IAuthenticatedUser;

    //if (this._user != null) {
    //  try {
    //    const sessionAlive = await this.accountService.isSessionAlive(this._user.sessionToken);
    //    if (!sessionAlive) {
    //      this.resetUser();
    //    }
    //  } catch (error) {
    //    console.error('Fehler beim Überprüfen der Session:', error);
    //    this.resetUser();
    //  }
    //}
    this.notifyForUserChanged();
  }

  /**
   * Updates the user data in storage.
   * @param user The authenticated user.
   */
  private updateUserInStorage(user: IAuthenticatedUser) {
    return this.storageService.setData(StorageLiterals.USER, user);
  }

  /**
   * Removes the user data from storage.
   */
  private removeUserFromStorage() {
    return this.storageService.remove(StorageLiterals.USER);
  }
}
