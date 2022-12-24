import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancleRegister = new EventEmitter<boolean>();
  registerForm: FormGroup;
  maxDate: Date = new Date();
  validationErrors: string[] = [];

  constructor(private accountService: AccountService, private fb: FormBuilder, private router: Router) {

  }

  ngOnInit(): void {
    this.FormIntialization();

    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  FormIntialization() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      gender: ['male', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.MatchValues('password')]]
    });

    // To check if password change after confirmPassword match it. are they still match?
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });

  }

  MatchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : { misMatching: true };
    }
  }

  Register() {
    this.accountService.Register(this.registerForm.value).subscribe(
      response => {
        this.router.navigateByUrl('/members');
      }, errors => {
        this.validationErrors = errors;
      }
    );
  }

  Cancel() {
    this.cancleRegister.emit(false);
  }
}
