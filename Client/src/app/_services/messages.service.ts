import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { GetPaginatedResult, GetPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  baseUrl = environment.apiUrl;

  // For SegnalR
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  // For SignalR
  CreateHubConnection(user: User, otherUsername: string) {
    // Create the hub connection
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => {
          return user.token;
        }
      })
      .withAutomaticReconnect()
      .build();

    // Start the hub connection
    this.hubConnection.start().catch(error => console.log(error));

    // to listen to the event and take the messages from it
    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThreadSource.next(messages);
    });

    // To listen to the event that send the new message that the user send it to update the chat to be live
    this.hubConnection.on('NewMessage', message => {
      // To upate the array that in the BehaviorSubject because you can't mutating its state
      // we will take the array add the new message to it send it in the next() of the subject again
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message]); // this will create a new array adding the new message at the end of it
      })
    });

    // If the recipient is in the group so he open the chat So mark all messages as readed
    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some(c => c.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if (!message.readAt) message.readAt = new Date(Date.now());
          })
          this.messageThreadSource.next([...messages]); // Create a new array with new messages value
        })
      }
    });

  }
  StopHubConnection() {
    if (this.hubConnection) { // if there is a hub connection -> stop it
      this.hubConnection.stop();
    }
  }


  GetMasseges(container: string, pageNumber: number, pageSize: number) {
    let params = GetPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);

    return GetPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  GetMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  // For SignalR We will create a new message using the hub instead of the controller
  async SendMessage(username: string, content: string) {
    // Invokes a hub method on the server using the specified name and arguments.
    return this.hubConnection.invoke('SendMessage', { recipientUsername: username, content })
      .catch(error => console.log(error)); // the interceptor can't handel this because it's no longer an http request
  }

  DeleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
