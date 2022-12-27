import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
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

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.LoadLikes();
  }

  LoadLikes() {
    this.memberService.GetLikes(this.predicate).subscribe(members => {
      this.members = members;
    })
  }

}
