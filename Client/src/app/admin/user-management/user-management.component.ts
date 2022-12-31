import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]> = [];

  bsModalRef?: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.GetUsersWithRoles();
  }

  GetUsersWithRoles() {
    this.adminService.GetUsersWithRoles().subscribe(res => {
      this.users = res;
    })
  }

  OpenModalWithComponent(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        // here must contain a variable same as that in the RolesModalComponent
        user: user,
        roles: this.GetRolesArray(user)
      }
    };
    //To specify the component that is the modal in, and send some configurations
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);

    // when the EventEmitter in (RolesModalComponent) emit the new roles
    this.bsModalRef.content.updateRoles.subscribe(value => {
      const rolesToUpdate = {
        // To take only the name of the roles that checked
        roles: [...value.filter(role => role.checked === true).map(role => role.name)]
      };
      if (rolesToUpdate) {
        // To send it to the API to update them in DB
        this.adminService.UpdateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
          // when they updated successfuly into DB then update them also in the current user roles
          user.roles = [...rolesToUpdate.roles]
        })
      }
    });

  }

  private GetRolesArray(user) {
    // will contain all roles but userRoles checked and other roles unchecked
    const roles = [];
    // the user role
    const userRoles = user.roles;
    // All available roles in our App
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' }
    ]

    // To make the userRole is checked in all availableRoles
    availableRoles.forEach(role => {
      let isMatch = false;
      for (const userRole of userRoles) {
        if (userRole === role.name) {
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if (!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    });

    return roles;
  }

}
