import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  // rather than creating a new model I will use the member but a partial of its properities not all of them;
  members: Partial<Member[]> = [];
  predicate = 'liked';

  // To send the Pagination informations
  pageNumber = 1;
  pageSize = 5;
  // To recieve the Pagination informations
  pagination?: Pagination;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.LoadLikes();
  }

  LoadLikes() {
    this.memberService.GetLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  // When user change the page
  PageChanged(event: any) {
    // To get the new pageNumber that user clicked
    this.pageNumber = event.page;

    // To load the members in the new page
    this.LoadLikes();
  }

}
