import { Injectable } from '@angular/core';
import { ACCESS_TOKEN, USER } from 'src/app/shared/constants';
import { AuthenticationResponse } from '../model/authentication-response.model';

@Injectable({
  providedIn: 'root',
})
export class TokenStorage {
  constructor() {}

  saveAccessToken(response: AuthenticationResponse): void {
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.removeItem(USER);
    localStorage.setItem(ACCESS_TOKEN, response.accessToken);
    localStorage.setItem(USER, response.id.toString());
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

  clear() {
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.removeItem(USER);
  }
}