import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'FriendshipMaker';
  users: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.GetAllUsers();
  }

  private GetAllUsers() {
    this.http.get('https://localhost:5001/api/Users/').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }
}
