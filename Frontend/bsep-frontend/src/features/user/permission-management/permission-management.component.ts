import { Component } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';

@Component({
  selector: 'app-permission-management',
  templateUrl: './permission-management.component.html',
  styleUrls: ['./permission-management.component.css']
})
export class PermissionManagementComponent {

  constructor(private route: ActivatedRoute) {}
  ngOnInit() {
    this.route.params.subscribe(params => {
      const permissionId = +params['permissionId'];
      console.log("prosledjeni id:" + permissionId)
      // Preuzmi detalje korisnika sa servera na osnovu userId-a i postavi ih u selectedUser
    });
  }
}
