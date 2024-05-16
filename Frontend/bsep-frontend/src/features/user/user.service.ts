import { Injectable } from '@angular/core';
import { User } from './model/user.model';
import { BehaviorSubject, Observable, switchMap, tap, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/app/env/environment';
import { Login } from './model/login.model';
import { AuthenticationResponse } from './model/authentication-response.model';
import { TokenStorage } from './jwt/token.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { EmailTokenRequest } from './model/passwordless-token-request.model';
import { RegistrationRequest } from './model/registration-request.model';
import { RegistrationRequestUpdate } from './model/registration-request-update.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  user$ = new BehaviorSubject<User>({
    id: 0,
    email: '',
    password: '',
    firstname: '',
    lastname: '',
    address: '',
    city: '',
    country: '',
    phone: '',
    companyName: '',
    taxId: '',
    packageType: 0,
    clientType: 0,
    role: 0,
  });

  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenStorage: TokenStorage
  ) {}

  getUserById(userId: number): Observable<User> {
    return this.http.get<User>(
      environment.apiHost + 'authentication/getUser/' + userId
    );
  }
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(
      environment.apiHost + 'authentication/getAllUsers'
    );
  }

  updateUser(user: User): Observable<boolean> {
    return this.http.post<boolean>(
      `${environment.apiHost}authentication/updateUser`,
      user
    );
  }

  updateAccessToken(accessToken: string, refreshToken: string, userId: number): Observable<string | null> {
    return this.http.post<boolean>(`${environment.apiHost}authentication/validateRefresh`, { userId, refreshToken })
      .pipe(
        switchMap((refreshValid: boolean) => {
          if (refreshValid) {
            return this.http.post<boolean>(`${environment.apiHost}authentication/validateAccess`, accessToken);
          } else {
            return of(false);
          }
        }),
        switchMap((accessValid: boolean) => {
          if (accessValid) {
            return this.http.post<string>(`${environment.apiHost}authentication/updateAccess`, userId);
          } else {
            return of(null);
          }
        })
      );
  }
  register(user: User): Observable<boolean> {
    return this.http.post<boolean>(environment.apiHost + 'authentication/register', user)
  }

  login(login: Login): Observable<any> {
    return this.http
      .post<AuthenticationResponse>(
        environment.apiHost + 'authentication/login',
        login,
        { observe: 'response' }
      )
      .pipe(
        tap(
          (authenticationResponse: any) => {
            this.tokenStorage.saveAccessToken(authenticationResponse.body);
  
            const refreshToken = authenticationResponse.body.refreshToken;
            this.tokenStorage.saveRefreshToken(refreshToken);
  
            this.setUser(refreshToken);
            this.router.navigate(['/home']);
          },
          (error) => {
            alert('Invalid credentials or account not activated.');
            console.error('Login failed:', error);
          }
        )
      );
  }
  getRefreshTokenFromCookie(): string | null {
    const cookie = document.cookie;
    const cookies = cookie.split('; ');

    for (const cookie in cookies) {
      const [name, value] = cookie.split('=');
      if (name === 'refreshToken') {
        return value;
      }
    }
    return null;
  }
  private setUser(refreshToken: string | null): void {
    if (refreshToken === null) {
      return;
    }

    const jwtHelperService = new JwtHelperService();
    const decodedToken = jwtHelperService.decodeToken(refreshToken);

    let user: User = {
      id: +decodedToken.id,
      email: decodedToken.email,
      password: '',
      address: '',
      city: '',
      country: '',
      phone: '',
      packageType: 0,
      clientType: 0,
      role: decodedToken.role,
    };
    this.user$.next(user);
  }

  logout(): void {
    this.router.navigate(['/home']).then((_) => {
      this.tokenStorage.clear();
      this.user$.next({
        id: 0,
        email: '',
        password: '',
        firstname: '',
        lastname: '',
        address: '',
        city: '',
        country: '',
        phone: '',
        companyName: '',
        taxId: '',
        packageType: 0,
        clientType: 0,
        role: 0,
      });
    });
    this.tokenStorage.saveRefreshToken(null);
    }

  sendPasswordlessLink(login: Login): Observable<any> {
    login.password = '';
    return this.http.post<any>(
      environment.apiHost + 'authentication/requestPasswordlessLogin',
      login
    );
  }

  authenticatePasswordlessToken(
    token: EmailTokenRequest
  ): Observable<any> {
    return this.http
      .post<AuthenticationResponse>(
        environment.apiHost + 'authentication/authenticatePasswordlessLogin',
        token,
        { observe: 'response' }
      )
      .pipe(
        tap(
          (response) => {
            this.tokenStorage.saveAccessToken(response.body);
  
            const refreshToken = response.body.refreshToken;
            this.tokenStorage.saveRefreshToken(refreshToken);
  
            this.setUser(refreshToken);
            this.router.navigate(['/home']);
          },
          (error) => {
            console.error('Passwordless login failed:', error);
            return error;
          }
        )
      );
  }

  authenticateEmailActivationToken(
    token: EmailTokenRequest
  ): Observable<AuthenticationResponse> {
    return this.http
      .post<AuthenticationResponse>(
        environment.apiHost + 'authentication/activateAccount',
        token
      )
      .pipe(
        tap(
          (response) => {
            if (response) {
              alert('Account activated! You can now login!');
            }
            this.router.navigate(['/home']);
          },
          (error) => {
            console.error('Email activation failed:', error);
            return error;
          }
        )
      );
  }

  getAllRegisterRequests(): Observable<RegistrationRequest[]> {
    const token = this.tokenStorage.getAccessToken();
    const headers = {
      Authorization: `Bearer ${token}`, // TODO: Interceptor ne radi, neka neko vidi zasto nije moja regija
    };
    return this.http.get<RegistrationRequest[]>(
      environment.apiHost + 'authentication/getAllRegistrationRequests',
      { headers }
    );
  }

  approveRegistrationRequest(
    update: RegistrationRequestUpdate
  ): Observable<any> {
    const token = this.tokenStorage.getAccessToken();
    const headers = {
      Authorization: `Bearer ${token}`,
    };
    return this.http.post<any>(
      `${environment.apiHost}authentication/approveRequest`,
      update,
      { headers }
    );
  }

  rejectRegistrationRequest(
    update: RegistrationRequestUpdate
  ): Observable<any> {
    const token = this.tokenStorage.getAccessToken();
    const headers = {
      Authorization: `Bearer ${token}`,
    };
    return this.http.post<any>(
      `${environment.apiHost}authentication/rejectRequest`,
      update,
      { headers }
    );
  }
}
