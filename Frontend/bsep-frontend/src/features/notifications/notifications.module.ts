import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalrService } from './signalr.service';
import { LogsComponent } from './notifications_logs/logs.component';
import { NotificationsComponent } from './notifications/notifications.component';

@NgModule({
  declarations: [
    LogsComponent,
    NotificationsComponent,
  ],
  imports: [
    CommonModule
  ],
  exports: [  
    LogsComponent,
    NotificationsComponent,
  ],
  providers: [SignalrService]
})
export class NotificationsModule { }
