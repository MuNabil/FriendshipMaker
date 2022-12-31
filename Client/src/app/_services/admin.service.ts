import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl + 'admin/';

  constructor(private http: HttpClient) { }

  GetUsersWithRoles() {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'users-with-roles');
  }

  UpdateUserRoles(username: string, roles: string[]) {
    return this.http.post<Partial<User[]>>(this.baseUrl + 'edit-roles/' + username + '?roles=' + roles, {});
  }
}
