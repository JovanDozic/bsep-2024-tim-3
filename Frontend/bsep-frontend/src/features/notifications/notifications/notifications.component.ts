import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SignalrService } from '../signalr.service';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {
  notifications: string[] = [];

  constructor(
    private signalrService: SignalrService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.signalrService.notificationReceived.subscribe(message => {
      this.notifications.push(message);
    });
  }

  goToLogs(): void {
    this.router.navigate(['/logs']);
  }
}