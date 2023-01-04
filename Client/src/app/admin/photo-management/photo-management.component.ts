import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.GetPhotosForApproval();
  }

  GetPhotosForApproval() {
    this.adminService.GetPhotosForApproval().subscribe(photos => this.photos = photos);
  }

  ApprovePhoto(photoId: number) {
    this.adminService.ApprovePhoto(photoId).subscribe(() => {
      this.RemovePhotoFormPhotos(photoId);
    });
  }

  RejectPhoto(photoId: number) {
    this.adminService.RejectPhoto(photoId).subscribe(() => {
      this.RemovePhotoFormPhotos(photoId);
    });
  }

  private RemovePhotoFormPhotos(photoId: number) {
    this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
  }
}
