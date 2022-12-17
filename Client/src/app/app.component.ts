import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'FriendshipMaker';

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.SetCurrentUser();
  }

  SetCurrentUser() {
    this.accountService.SetCurrentUser(JSON.parse(localStorage.getItem('user')));
  }
}
