import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  user: User;
  baseUrl = environment.apiUrl;
  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user
      }
    })
  }


  ngOnInit(): void {
    this.initializeUploader();
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',  // Send the request to the API endpoint to save the file
      authToken: 'Bearer ' + this.user?.token,  // Send the Bearer auth tokin with the request to the API endpoint
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // Maximum size that I have from cloudinary account
    });

    // uploader configurations
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false  // Because I use Bearer token to send the credentials with the file.
    }

    // when file is uploaded successfully
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);
      }
    }
  }

  // Set the drop zone to allow dragging and dropping files
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }



  SetMainPhoto(photo: Photo) {
    this.memberService.SetMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.SetCurrentUser(this.user); // To update the localStorage
      this.member.photoUrl = photo.url; // update the current diplaying photo in this component
      // Updating the member photos
      this.member.photos.forEach(p => {
        if (p.isMain) p.isMain = false;
        if (p.id === photo.id) p.isMain = true;
      })
    })
  }

  DeletePhoto(photoId: number) {
    this.memberService.DeletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(p => p.id !== photoId);
    })
  }

}
