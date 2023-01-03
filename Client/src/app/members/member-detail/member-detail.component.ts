import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MessagesService } from 'src/app/_services/messages.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  // For Messages
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  messages: Message[] = [];

  //for SignalR to send the current username to the hub
  user: User;

  constructor(public presence: PresenceService, private route: ActivatedRoute,
    private messageService: MessagesService, private accountService: AccountService, private router: Router) {

    // To get the current user to send his name to the hub
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);

    // To turn of the reuse route strategy in this component
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;

  }

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
      // Instead of loading the messages thread from controller we will load it from the hub
      this.messageService.CreateHubConnection(this.user, this.member.username);
    }
    else { // To disconnect from the hub when they go away from the messages tab
      this.messageService.StopHubConnection();
    }
  }
  ngOnDestroy(): void {
    // To disconnect from the hub when they go away from this component
    this.messageService.StopHubConnection();
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
