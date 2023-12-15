import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AuthService } from './auth/auth.service';

export function AuthGuardFunction():Observable<boolean> {
  const _autheSrvice = inject(AuthService);
  const _route = inject(Router);
  return _autheSrvice.isAuth$.pipe(map(isAuth => {
    if (isAuth) {
      return true;
    } else {
      _route.navigate(['/auth']);
      return false;
    }
  }));
}

// export class AuthGuard  {


//   constructor(private _autheSrvice: AuthService, private _route: Router) {}

//   canActivate(
//     route: ActivatedRouteSnapshot,
//     state: RouterStateSnapshot
//   ):
//     | Observable<boolean | UrlTree>
//     | Promise<boolean | UrlTree>
//     | boolean
//     | UrlTree {
//     return this._autheSrvice.isAuth$.pipe(map(isAuth => {
//       if (isAuth) {
//         return true;
//       } else {
//         this._route.navigate(['/auth']);
//         return false;
//       }
//     }));
//   }
// }
