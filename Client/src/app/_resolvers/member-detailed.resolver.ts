import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable } from 'rxjs';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {

  constructor(private memberService: MembersService) { }

  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberService.GetMember(route.paramMap.get('username'));
    // the username that I sending it with the routerLink to member-detail-component that I will apply this resolver to it.

    // In the resolver we don't need to subscribe/unsubscribe the router will take care of this
  }
}
