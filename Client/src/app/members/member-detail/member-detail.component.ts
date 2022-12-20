import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private memberService: MembersService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.LoadMember();

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
  }

  LoadMember() {
    let username: string = this.route.snapshot.paramMap.get('username');
    this.memberService.GetMember(username)
      .subscribe(res => {
        this.member = res;
        this.galleryImages = this.GetPhotos();
      });
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

}
