import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  navigateTo: string;
  constructor(public accountService: AccountService) {
    this.accountService.currentUser$.subscribe(user => {
      if (user)
        this.navigateTo = '/members';
      else this.navigateTo = '/';
    });
  }

  ngOnInit(): void {
  }

}
