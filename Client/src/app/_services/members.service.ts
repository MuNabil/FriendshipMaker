import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl + 'Users/';
  constructor(private http: HttpClient) {
  }

  GetMembers() {
    return this.http.get<Member[]>(this.baseUrl);
  }

  GetMember(username: string) {
    return this.http.get<Member>(this.baseUrl + username);
  }

}
