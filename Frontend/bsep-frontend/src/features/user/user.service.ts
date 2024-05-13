import { Injectable } from '@angular/core';
import { User} from './model/user.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/app/env/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  user$ = new BehaviorSubject<User>({id: 0, email:'', password:'', firstname:'', lastname:'', address:'', city: '', country:'', phone:'', companyName: '', taxId: '', packageType:0, clientType: 0 , role:0 });


  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  getUserById(userId: number): Observable<User> {

    return this.http.get<User>(environment.apiHost + 'authentication/getUser/' + userId);
  }
  getAllUsers():Observable<User[]> {
    
    return this.http.get<User[]>(environment.apiHost + 'authentication/getAllUsers');
  }

  updateUser(user: User): Observable<boolean> {
    console.log("usao u service update user");
    return this.http.post<boolean>(`${environment.apiHost}authentication/updateUser`, user);
}

registerUser(user: User): Observable<boolean> {
  return this.http.post<boolean>(`${environment.apiHost}authentication/register`, user);
}

}
