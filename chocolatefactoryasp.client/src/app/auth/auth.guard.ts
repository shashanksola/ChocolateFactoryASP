// src/app/auth/auth.guard.ts

import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

    const token = localStorage.getItem('token') || '';
    if (token === '') {
      this.router.navigate(['/login'])
      return false;
    }
    const decodedToken: any = jwtDecode(token);

    const currentTime = Math.floor(Date.now() / 1000); // Convert ms to seconds

    if (decodedToken.exp < currentTime) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }
}
