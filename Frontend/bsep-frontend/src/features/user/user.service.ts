import { Injectable } from '@angular/core';
import { User } from './model/user.model';
import { BehaviorSubject, Observable, switchMap, tap, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/app/env/environment';
import { Credentials } from './model/login.model';
import { AuthenticationResponse } from './model/authentication-response.model';
import { TokenStorage } from './jwt/token.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { EmailTokenRequest } from './model/passwordless-token-request.model';
import { RegistrationRequest } from './model/registration-request.model';
import { RegistrationRequestUpdate } from './model/registration-request-update.model';
import { RegistrationResponse } from './model/registration-response.model';
import { Verify2faRequest } from './model/verify-2fa.model';
import { Tokens } from './model/tokens.model';
import { ChangePasswordRequest } from './model/change-password-request.model';
import { ResetPassword } from './model/resetPassword.model';

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
    isTwoFactorEnabled: false,
  });

  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenStorage: TokenStorage
  ) {}

  changePassword(request: ChangePasswordRequest): Observable<boolean> {
    return this.http.post<boolean>(
      `${environment.apiHost}authentication/changePassword`,
      request
    );
  }
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

  requestPasswordReset(email: string) : Observable<boolean> {
    return this.http.post<boolean>(
      `${environment.apiHost}authentication/requestPasswordReset`,
      email
    );
  }

  getUnblocked(): Observable<User[]> {
    return this.http.get<User[]>(
      environment.apiHost + 'authentication/getUnblocked'
    );
  }

  updateUser(user: User): Observable<boolean> {
    return this.http.post<boolean>(
      `${environment.apiHost}authentication/updateUser`,
      user
    );
  }

  blockUser(user: User): Observable<boolean> {
    return this.http.post<boolean>(
      `${environment.apiHost}authentication/blockUser`,
      user
    );
  }

  resetPassword(resetPassword: ResetPassword): Observable<boolean> {
    return this.http.post<boolean>(
      `${environment.apiHost}authentication/resetPassword`,
      resetPassword
    );
  }
  updateAccessToken(accessToken: string, refreshToken: string, userId: number): Observable<string | null> {
    return this.http.post<boolean>(`${environment.apiHost}authentication/validateRefresh`, { userId, refreshToken })
      .pipe(
        switchMap((refreshValid: boolean) => {
          if (refreshValid) {
            return this.http.post<boolean>(
              `${environment.apiHost}authentication/validateAccess`,
              accessToken
            );
          } else {
            return of(false);
          }
        }),
        switchMap((accessValid: boolean) => {
          if (accessValid) {
            return this.http.post<string>(
              `${environment.apiHost}authentication/updateAccess`,
              userId
            );
          } else {
            return of(null);
          }
        })
      );
  }

  register(user: User): Observable<RegistrationResponse> {
    return this.http.post<RegistrationResponse>(
      environment.apiHost + 'authentication/register',
      user
    );
  }

  registerVerify2fa(verify2faRequest: Verify2faRequest): Observable<boolean> {
    return this.http.post<boolean>(
      environment.apiHost + 'authentication/register/verify2fa',
      verify2faRequest
    );
  }

  deleteData(id: number): Observable<boolean> {
    return this.http
      .delete<boolean>(`${environment.apiHost}authentication/delete-data/${id}`)
      .pipe(
        tap((response: boolean) => {
          if (response) {
            console.log('User data deleted successfully');
          } else {
            console.log('Failed to delete user data');
          }
        })
      );
  }

  login_old(login: Credentials): Observable<any> {
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

  login(credentials: Credentials): Observable<Tokens> {
    return this.http
      .post<Tokens>(environment.apiHost + 'authentication/login', credentials)
      .pipe(
        tap(
          (tokens: Tokens) => {
            if (tokens && !tokens.isTwoFactorEnabled) {
              this.tokenStorage.saveAccessToken({
                accessToken: tokens.accessToken,
                id: tokens.id,
              });
              this.tokenStorage.saveRefreshToken(tokens.refreshToken);
              this.setUser(tokens.refreshToken);
              this.router.navigate(['/home']);
            } else if (tokens && tokens.isTwoFactorEnabled) {
              const code = prompt('Please enter the verification code:');
              if (code) {
                this.http
                  .post<Tokens>(
                    environment.apiHost + 'authentication/login/verify2fa',
                    { code, userId: tokens.id, tempToken: tokens.tempToken }
                  )
                  .subscribe(
                    (response) => {
                      if (response) {
                        this.tokenStorage.saveAccessToken({
                          accessToken: response.accessToken,
                          id: response.id,
                        });
                        this.tokenStorage.saveRefreshToken(
                          response.refreshToken
                        );
                        alert('Successfully logged in!');
                        this.setUser(response.refreshToken);
                        this.router.navigate(['/home']);
                      }
                    },
                    (error) => {
                      alert('Invalid two factor code.');
                      console.error('Invalid code:', error);
                    }
                  );
              } else {
                alert('You must enter the code to login. Try again.');
              }
            }
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
      isTwoFactorEnabled: false,
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

  sendPasswordlessLink(login: Credentials): Observable<any> {
    login.password = '';
    return this.http.post<any>(
      environment.apiHost + 'authentication/requestPasswordlessLogin',
      login
    );
  }

  authenticatePasswordlessToken(token: EmailTokenRequest): Observable<any> {
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

  testRateLimiter(type: string) : Observable<any> {
    return this.http.get<any>(`${environment.apiHost}advertisements/${type}`)
    .pipe(
      tap(
        (response) => {
          if (response) {
            return response
          }
        },
        (error) => {
          console.error('Limit achieved!', error);
          return error;
        }
      )
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
