import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl + 'Users/';
  members: Member[] = [];

  // To contain the response body (membr[]) and the response header (pagination information)
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) {
  }

  GetMembers(page?: number, itemsPerPage?: number) {
    //To add the parameters in the query string
    let params = new HttpParams();

    if (page !== null && itemsPerPage !== null) {
      // To add the parameters that I want to with params query string to the endpoint
      params = params.append('pageNumber', page.toString()); // toString() because I wanna send it as a string in the query string
      params = params.append('pageSize', itemsPerPage.toString());
    }

    // Using this observe syntax to get the all 'response' not just the body
    return this.http.get<Member[]>(this.baseUrl, { observe: 'response', params }).pipe(
      map(response => {

        // Take the member[]
        this.paginatedResult.result = response.body;

        // Take the pagination informations
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return this.paginatedResult;
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
