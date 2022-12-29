import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessagesService } from 'src/app/_services/messages.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  // For Messages
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute, private messageService: MessagesService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    // To get the queryParams that sended with the routerLink in [queryParams]='{tab: 3}'
    this.route.queryParams.subscribe(params => {
      params.tab ? this.SelectTab(params.tab) : this.SelectTab(0);
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.GetPhotos();
  }

  GetPhotos() {
    const imgUrls = [];
    for (const img of this.member.photos) {
      imgUrls.push(
        {
          small: img?.url,
          medium: img?.url,
          big: img?.url
        }
      );
    }
    return imgUrls;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.LoadMessages();
    }
  }

  LoadMessages() {
    this.messageService.GetMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  // To route the user to the any tab directlly
  SelectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

}
