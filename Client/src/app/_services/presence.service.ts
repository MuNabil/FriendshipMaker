import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpTransportType, HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;

  // BehaviorSubject to keep track with online users and give them to any subscriber
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  CreateHubConnection(user: User) {

    // To creating the hub connection
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect() // to reconnect if  there is a network proplems
      .build();

    // To start the hub connection
    this.hubConnection.start().catch(error => console.log(error));

    // Listening to the server events of the API "UserIsOnline" & "UserIsOffline"

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        // Using spread operator to take the usernames that actully in the array and add the new inline username to it
        this.onlineUsersSource.next([...usernames, username]);
      });
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(u => u !== username)]);
      });
    });

    this.hubConnection.on('GetOnlineUsers', users => {
      this.onlineUsersSource.next(users);
    });

    // For notifying a users when they recieved a new messages
    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => { // anonymous object to recieve the anonymous that will send from this event
      this.toastr.info(knownAs + 'has send you a new message')
        .onTap  // triggered on toast click
        .pipe(take(1))
        .subscribe(() => {
          // To route him to the messages tab chat when he click on the toastr
          this.router.navigateByUrl('/members/' + username + '?tab=3')

        })
    });
  }

  StopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}