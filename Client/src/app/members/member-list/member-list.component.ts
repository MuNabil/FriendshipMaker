import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // to get them from the paginationResult.result that contains the response body
  members: Member[] = [];

  // to get them from the pagination (pagination informations) result that contans the response headers
  pagination: Pagination;

  // to set the query string parameters
  userParams: UserParams;

  // To get the current user to set the gender
  user: User;

  // To display the list of genders as selected list to the user
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' }
  ];

  constructor(private membersService: MembersService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      // To intiate the userParams and send the user to the ctor to set the age there.
      this.userParams = new UserParams(user);
    })
  }

  ngOnInit(): void {
    this.LoadMembers();
  }

  LoadMembers() {
    this.membersService.GetMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  // event for pagination
  PageChanged(event: any) {
    // To see what page the user click on it
    this.userParams.pageNumber = event.page;

    // Load the new members for the page the user clicked
    this.LoadMembers();
  }

  // To reset the filters
  ResetFilters() {
    // To reset the params
    this.userParams = new UserParams(this.user);
    // send the params again to the api and get the new response
    this.LoadMembers();
  }

}
