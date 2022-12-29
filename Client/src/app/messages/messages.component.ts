import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessagesService } from '../_services/messages.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;

  messages: Message[] = [];
  pagination?: Pagination;

  //To overcome the photo switch
  loading = false;

  constructor(private messageService: MessagesService) { }

  ngOnInit(): void {
    this.LoadMessages();
  }

  LoadMessages() {
    this.loading = true;
    this.messageService.GetMasseges(this.container, this.pageNumber, this.pageSize).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
      this.loading = false;
    })
  }

  PageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.LoadMessages();
    }
  }

  DeleteMessage(id: number) {
    this.messageService.DeleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id === id), 1); // to  remove the message from messages array
    })
  }

}
