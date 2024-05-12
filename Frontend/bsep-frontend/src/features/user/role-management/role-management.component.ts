import { Component, OnInit } from '@angular/core';
import { User } from '../model/user.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrls: ['./role-management.component.css']
})
export class RoleManagementComponent implements OnInit{
  
  selectedUser: User | null = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const userId = +params['userId'];
      console.log("prosledjeni id:" + userId)
      // Preuzmi detalje korisnika sa servera na osnovu userId-a i postavi ih u selectedUser
    });
  }
}
