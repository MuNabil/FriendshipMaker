import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  // Map to make the cache -> the key will be the userParams that contains the request details and the value will be the response themselvs
  memberCache = new Map();

  // To keep these infrmation here to keep the pagination information.
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    // Note that: you can inject a service into service but you can't inject two services each one in the othe one because this will cause a circule refrences error.
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      // To intiate the userParams and send the user to the ctor to set the age there.
      this.userParams = new UserParams(user);
    })
  }

  GetUserParams(): UserParams {
    return this.userParams;
  }

  SetUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }

  ResetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  GetMembers(userParams: UserParams) {

    // To check if this request is in the cache
    let response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    // To send the pagination informations in the query string
    let params = this.GetPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    // To also send the filtering informations in the query string
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.GetPaginatedResult<Member[]>(this.baseUrl + 'Users/', params)
      .pipe(
        // To save the response in the cache then returning it.
        map(response => {
          // The key of the cache will be the userParams as a string seprated by '-'
          this.memberCache.set(Object.values(userParams).join('-'), response);
          return response;
        })
      );
  }

  GetMember(username: string) {
    // To get only the values from the memberCache Map
    const paginatedResultFromCache = [...this.memberCache.values()];

    // the values is ((PaginatedResult { result: members, pagination: I DON'T WANT IT }));
    const members = paginatedResultFromCache.reduce((arr, response) => arr.concat(response.result), []);

    // To get the user with the wanted username
    const member = members.find((member: Member) => member.username === username);

    // To return it if he is in the cache
    if (member) {
      return of(member);
    }

    // If he is not in the cache request it from the server
    return this.http.get<Member>(this.baseUrl + 'Users/' + username);
  }

  UpdateMember(member: Member) {
    return this.http.put(this.baseUrl + 'Users/', member).pipe(
      map(() => {
        const memberIndex = this.members.indexOf(member);
        this.members[memberIndex] = member;
      })
    );
  }

  SetMainPhoto(photoId: number) {
    return this.http.put(`${this.baseUrl}Users/set-main-photo/${photoId}`, {});
  }

  DeletePhoto(photoId: number) {
    return this.http.delete(`${this.baseUrl}Users/delete-photo/${photoId}`);
  }

  //Deal with like feature
  AddLike(username: string) {
    return this.http.post(this.baseUrl + 'Likes/' + username, {});
  }

  GetLikes(predicate: string) {
    return this.http.get<Partial<Member[]>>(`${this.baseUrl}Likes?predicate=${predicate}`);
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
