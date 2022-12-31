import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  user: User;
  constructor(private viewContentRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    });
  }
  ngOnInit(): void {
    if (!this.user?.roles || this.user == null) {
      this.viewContentRef.clear();
      return;
    }

    if (this.user?.roles.some(role => this.appHasRole.includes(role))) {
      this.viewContentRef.createEmbeddedView(this.templateRef);
    }
    else {
      this.viewContentRef.clear();
    }
  }

}
