import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
map

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl + 'Account/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'register', model).pipe(
      map((response: User) => {
        if (response) {
          this.SetCurrentUser(response);
        }
      })
    );
  }

  Login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: User) => {
        if (response) {
          this.SetCurrentUser(response);
        }
      })
    );
  }
  Logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  SetCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
}
