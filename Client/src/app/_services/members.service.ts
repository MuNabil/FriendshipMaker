import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl + 'Users/';
  members: Member[] = [];
  constructor(private http: HttpClient) {
  }

  GetMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl).pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }

  GetMember(username: string) {
    const member = this.members.find(x => x.username === username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + username);
  }

  UpdateMember(member: Member) {
    return this.http.put(this.baseUrl, member).pipe(
      map(() => {
        const memberIndex = this.members.indexOf(member);
        this.members[memberIndex] = member;
      })
    );
  }

  SetMainPhoto(photoId: number) {
    return this.http.put(`${this.baseUrl}set-main-photo/${photoId}`, {});
  }

  DeletePhoto(photoId: number) {
    return this.http.delete(`${this.baseUrl}delete-photo/${photoId}`);
  }

}
