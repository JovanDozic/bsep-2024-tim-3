import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalrService } from './signalr.service';
import { LogsComponent } from './notifications_logs/logs.component';

@NgModule({
  declarations: [
    LogsComponent,
  ],
  imports: [
    CommonModule
  ],
  providers: [SignalrService]
})
export class NotificationsModule { }
