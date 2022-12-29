import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { GetPaginatedResult, GetPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  GetMasseges(container: string, pageNumber: number, pageSize: number) {
    let params = GetPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);

    return GetPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  GetMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  SendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername: username, content });
    // I must send the objects to the API by the same names there (but in camelCase)
    // So if the name match the other name then send it else make a key with the same name there
  }

  DeleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
