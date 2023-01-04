import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
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

  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.GetUserParams();
  }

  ngOnInit(): void {
    this.LoadMembers();
  }

  LoadMembers() {
    // To always set the userParams before loading the members with filtering
    this.membersService.SetUserParams(this.userParams);
    this.membersService.GetMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  // event for pagination
  PageChanged(event: any) {
    // To see what page the user click on it
    this.userParams.pageNumber = event.page;

    // To set the userParams to with the new values 
    this.membersService.SetUserParams(this.userParams);

    // Load the new members for the page the user clicked
    this.LoadMembers();
  }

  // To reset the filters
  ResetFilters() {
    // To reset the params
    this.userParams = this.membersService.ResetUserParams();
    // send the params again to the service and get the new members
    this.LoadMembers();
  }

}
