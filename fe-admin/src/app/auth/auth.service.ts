import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { iUser, Users } from '../shared/models/user.model'
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _isAuth: BehaviorSubject<boolean>;
  public isAuth$: Observable<boolean>;
  private _currentUser: BehaviorSubject<any>;
  public currentUser$: Observable<Partial<iUser>>;


  constructor(private http: HttpClient) {
    let _isAuth = localStorage.getItem('_isAuth') === 'true' ? true : false;
    let user = localStorage.getItem('User');
    user ? user = JSON.parse(user) : null;
    this._isAuth = new BehaviorSubject(_isAuth);
    this._currentUser = new BehaviorSubject(user ? user : {});
    this.currentUser$ = this._currentUser.asObservable();
    this.isAuth$ = this._isAuth.asObservable();
  }

  tryLogIn(user: iUser): boolean {
    let findByLoginUser = Users.find((u) => u.email === user.email);
    let auth = findByLoginUser
      ? Users.find((u) => u.password === user.password)?.password === user.password
      : false;
    if (auth) {
      this._logIn();
      localStorage.setItem('User',JSON.stringify(findByLoginUser))
      this._currentUser.next(findByLoginUser ? findByLoginUser : {});
    }
    return auth;
  }
  LogIn(user: iUser): boolean {
    let findByLoginUser = Users.find((u) => u.email === user.email);
    let auth = findByLoginUser
      ? Users.find((u) => u.password === user.password)?.password === user.password
      : false;
    if (auth) {
      this._logIn();
      localStorage.setItem('User',JSON.stringify(findByLoginUser))
      this._currentUser.next(findByLoginUser ? findByLoginUser : {});
    }
    return auth;
  }

  private _logIn() {
    this._isAuth.next(true);
    localStorage.setItem('_isAuth', 'true');
  }

  logOut() {
    this._isAuth.next(false);
    this._currentUser.next({})
    localStorage.removeItem('User');
    localStorage.setItem('_isAuth', 'false');
  }
}
