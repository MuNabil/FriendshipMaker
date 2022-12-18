
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  model: any = {};
  constructor(public accountService: AccountService, private router: Router,
    private toastr: ToastrService) {

  }

  ngOnInit(): void {

  }
  Login() {
    this.accountService.Login(this.model).subscribe({
      next: response => {
        this.router.navigateByUrl('/members')
      }
    });
  }

  Logout() {
    this.accountService.Logout();
    this.router.navigateByUrl('/');
  }

}
