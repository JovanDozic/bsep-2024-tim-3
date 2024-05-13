import { Injectable } from '@angular/core';
import { User} from './model/user.model';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/app/env/environment';
import { Login } from './model/login.model';
import { AuthenticationResponse } from './model/authentication-response.model';
import { TokenStorage } from './jwt/token.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  user$ = new BehaviorSubject<User>({id: 0, email:'', password:'', firstname:'', lastname:'', address:'', city: '', country:'', phone:'', companyName: '', taxId: '', packageType:0, clientType: 0 , role:0 });


  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenStorage: TokenStorage
  ) { }

  getUserById(userId: number): Observable<User> {

    return this.http.get<User>(environment.apiHost + 'authentication/getUser/' + userId);
  }
  getAllUsers():Observable<User[]> {
    
    return this.http.get<User[]>(environment.apiHost + 'authentication/getAllUsers');
  }

  updateUser(user: User): Observable<boolean> {
    console.log("usao u servis");
    return this.http.post<boolean>(`${environment.apiHost}authentication/updateUser`, user);
}
login(login: Login): Observable<AuthenticationResponse> {
  console.log(login);
  return this.http
    .post<AuthenticationResponse>(environment.apiHost + 'authentication/login', login)
    .pipe(
      tap(
        (authenticationResponse) => {
          this.tokenStorage.saveAccessToken(authenticationResponse);
          const refreshToken = this.getRefreshTokenFromCookie();
          this.setUser(refreshToken);
          this.router.navigate(['/home']);
        },
        (error) => {
          alert("Invalid credentials");
          console.error('Login failed:', error);
        }
      )
    );
}
getRefreshTokenFromCookie(): string | null{
  const cookie = document.cookie;
  const cookies = cookie.split("; ");

  for (const cookie in cookies){
    const [name, value] = cookie.split('=');
    if(name === 'refreshToken'){
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
    role: decodedToken.role 
};
console.log("User:", user);
this.user$.next(user);
}

logout(): void {
  this.router.navigate(['/home']).then(_ => {
    this.tokenStorage.clear();
    this.user$.next({id: 0, email:'', password:'', firstname:'', lastname:'', address:'', city: '', country:'', phone:'', companyName: '', taxId: '', packageType:0, clientType: 0 , role:0 });
  }
  );
}
registerUser(user: User): Observable<boolean> {
  return this.http.post<boolean>(`${environment.apiHost}authentication/register`, user);
}
}
