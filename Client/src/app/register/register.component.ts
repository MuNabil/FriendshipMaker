import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Output() cancleRegister = new EventEmitter<boolean>();
  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  Register() {
    this.accountService.Register(this.model).subscribe(
      response => {
        console.log(response);
        this.Cancel();
      }, err => {
        console.log(err);
        this.toastr.error(err.error);
      }
    );
  }

  Cancel() {
    this.cancleRegister.emit(false);
  }
}
