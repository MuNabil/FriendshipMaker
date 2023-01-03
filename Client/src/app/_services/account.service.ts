import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';
map

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl + 'Account/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presence: PresenceService) { }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'register', model).pipe(
      map((user: User) => {
        if (user) {
          this.SetCurrentUser(user);

          // connect the user to the presence hub
          this.presence.CreateHubConnection(user);
        }
      })
    );
  }

  Login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((user: User) => {
        if (user) {
          this.SetCurrentUser(user);

          // connect the user to the presence hub
          this.presence.CreateHubConnection(user);
        }
      })
    );
  }
  Logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);

    // disconnect the user from the presence hub
    this.presence.StopHubConnection();
  }

  SetCurrentUser(user: User) {
    user.roles = [];
    //To get only the roles from the payload(Data)
    const roles = this.GetDecodedToken(user.token).role;
    // When user has one role it will be a string not array
    // So I need to convert it into array anyway
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  GetDecodedToken(token: string) {
    // atob method allow to decode the token  then [1] to take the payload(Data)
    return JSON.parse(atob(token.split('.')[1]));
  }
}
