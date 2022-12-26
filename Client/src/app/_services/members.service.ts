import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl + 'Users/';
  members: Member[] = [];

  constructor(private http: HttpClient) {
  }

  GetMembers(userParams: UserParams) {

    // To send the pagination informations in the query string
    let params = this.GetPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    // To also send the filtering informations in the query string
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.GetPaginatedResult<Member[]>(this.baseUrl, params);
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

  private GetPaginatedResult<T>(url: string, params: HttpParams) {
    // To contain the response body (membr[]) and the response header (pagination information)
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    // Using this observe syntax to get the all 'response' not just the body
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {

        // Take the response body that containing the actual data
        paginatedResult.result = response.body;

        // Take the pagination informations
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return paginatedResult;
      })
    );
  }

  private GetPaginationHeaders(pageNumber: number, pageSize: number) {
    //To add the parameters in the query string
    let params = new HttpParams();

    // To add the parameters that I want to with params query string to the endpoint
    params = params.append('pageNumber', pageNumber.toString()); // toString() because I wanna send it as a string in the query string

    params = params.append('pageSize', pageSize.toString());

    return params;
  }

}
