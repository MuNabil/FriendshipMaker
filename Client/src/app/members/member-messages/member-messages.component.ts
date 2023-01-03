import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessagesService } from 'src/app/_services/messages.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() messages: Message[];
  // to send a message
  @Input() recipientUsername: string;
  messageContent: string;
  // To reset/clean the message input after sending a message
  @ViewChild('messageForm') messageForm: NgForm;

  constructor(public messageService: MessagesService) { }

  ngOnInit(): void {
  }

  SendMessage() {
    this.messageService.SendMessage(this.recipientUsername, this.messageContent).then(() => {
      this.messageForm.reset();
    })
  }

}
