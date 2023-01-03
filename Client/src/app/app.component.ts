import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'FriendshipMaker';

  constructor(private accountService: AccountService, private presence: PresenceService) { }

  ngOnInit() {
    this.SetCurrentUser();
  }

  SetCurrentUser() {
    const user = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.accountService.SetCurrentUser(user);
      // To start the hub connection and send the user to get access to his token
      this.presence.CreateHubConnection(user);
    }
  }
}
