import { Injectable } from '@angular/core';
import { ACCESS_TOKEN, USER } from 'src/app/shared/constants';

@Injectable({
  providedIn: 'root',
})
export class TokenStorage {
  constructor() {}

  saveAccessToken(token: string, userId: number): void {
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.removeItem(USER);
    localStorage.setItem(ACCESS_TOKEN, token);
    localStorage.setItem(USER, userId.toString());
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