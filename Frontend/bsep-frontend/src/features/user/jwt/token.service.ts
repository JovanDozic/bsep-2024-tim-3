import { Injectable } from '@angular/core';
import { ACCESS_TOKEN, USER } from 'src/app/shared/constants';
import { AuthenticationResponse } from '../model/authentication-response.model';
import { CookieService } from 'ngx-cookie-service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root',
})
export class TokenStorage {
  constructor(private cookieService: CookieService) {}

  saveAccessToken(response: AuthenticationResponse): void {
    this.saveToken(response.accessToken);
    localStorage.removeItem(USER);
    localStorage.setItem(USER, response.id.toString());
  }

  saveToken(access: string){
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.setItem(ACCESS_TOKEN, access);
    
  }
  getAccessToken() {
    return localStorage.getItem(ACCESS_TOKEN);
  }

  getUserId() {
    const userIdString = localStorage.getItem(USER);
    if (userIdString) {
      return parseInt(userIdString, 10);
    }
    return 0;
  }

  getTokenExpiration(token: string): Date | null {
    const jwtHelper = new JwtHelperService();
    try {
      const decodedToken = jwtHelper.decodeToken(token);
      if (decodedToken && decodedToken.exp) {
        const expirationTimeInSeconds = decodedToken.exp;
        return new Date(expirationTimeInSeconds * 1000); // Convert to milliseconds
      }
    } catch (error) {
      console.error('Error decoding access token:', error);
    }
    return null;
  }

  getTokenId(token: string): string | null {
    const jwtHelper = new JwtHelperService();
    try {
      const decodedToken = jwtHelper.decodeToken(token);
      if (decodedToken && decodedToken.id) {
        return decodedToken.id;
      }
    } catch (error) {
      console.error('Error decoding access token:', error);
    }
    return null;
  }
  haveSameIdsInTokens(accessToken: string, refreshToken: string): boolean {
    const accessId = this.getTokenId(accessToken);
    const refreshId = this.getTokenId(refreshToken);

    return (
      accessId && refreshId && accessId.toString() === refreshId.toString()
    );
  }
  saveRefreshToken(refreshToken: string) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    if (refreshToken) {
      this.cookieService.set(
        'refreshToken',
        refreshToken,
        tomorrow,
        '/',
        'localhost',
        true,
        'Strict'
      );
    } else {
      this.cookieService.delete('refreshToken', '/', 'localhost', true);
    }
  }

  getRefreshToken(): string {
    return this.cookieService.get('refreshToken');
  }

  clear() {
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.removeItem(USER);
  }
}
