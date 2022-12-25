import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // to get them from the paginationResult.result that contans the response body
  members: Member[] = [];

  // to get them from the pagination (pagination informations) result that contans the response headers
  pagination: Pagination;

  // to set the query string parameters
  pageSize: number = 5;
  pageNumber: number = 1;

  constructor(private membersService: MembersService) { }

  ngOnInit(): void {
    this.LoadMembers();
  }

  LoadMembers() {
    this.membersService.GetMembers(this.pageNumber, this.pageSize).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  // event for pagination
  PageChanged(event: any) {
    // To see what page the user click on it
    this.pageNumber = event.page;

    // Load the new members for the page the user clicked
    this.LoadMembers();
  }

}
