import { Component } from '@angular/core';
import {
  RegistrationRequest,
  RegistrationRequestStatus,
} from '../model/registration-request.model';
import { UserService } from '../user.service';
import { RegistrationRequestUpdate } from '../model/registration-request-update.model';

@Component({
  selector: 'app-registration-requests-management',
  templateUrl: './registration-requests-management.component.html',
  styleUrls: ['./registration-requests-management.component.css'],
})
export class RegistrationRequestsManagementComponent {
  requests: RegistrationRequest[] = [];

  constructor(private userService: UserService) {
    this.userService.getAllRegisterRequests().subscribe((requests) => {
      console.log(requests);
      this.requests = requests;
    });
  }

  approveRequest(request: RegistrationRequest) {
    const update: RegistrationRequestUpdate = {
      id: request.id,
      reason: null,
    };
    this.userService.approveRegistrationRequest(update).subscribe(() => {
      request.status = RegistrationRequestStatus.Approved;
    });
  }

  rejectRequest(request: RegistrationRequest) {
    const reason = prompt('Enter reason for rejection:');
    const update: RegistrationRequestUpdate = {
      id: request.id,
      reason: reason,
    };
    this.userService.rejectRegistrationRequest(update).subscribe(() => {
      request.status = RegistrationRequestStatus.Rejected;
      request.reason = reason;
    });
  }
}
