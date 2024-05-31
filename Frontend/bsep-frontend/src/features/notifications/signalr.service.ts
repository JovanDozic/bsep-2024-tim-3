import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { environment } from 'src/app/env/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection;

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/notificationHub')
      .build();

    this.hubConnection.on('ReceiveNotification', (message: string) => {
      this.showNotification(message);
    });

    this.hubConnection.start().catch(err => console.error('SignalR connecting error: ', err));

    this.getAllLogs().subscribe(logs => {
      console.log('Initial logs:', logs);
    });
  }

  private showNotification(message: string): void {
    if (!('Notification' in window)) {
      console.log('This browser does not support desktop notification');
      return;
    }

    if (Notification.permission === 'granted') {
      this.displayNotification(message);
    } else if (Notification.permission !== 'denied') {
      Notification.requestPermission().then(permission => {
        if (permission === 'granted') {
          this.displayNotification(message);
        }
      });
    }
  }

  private displayNotification(message: string): void {
    const notification = new Notification('New Notification', {
      body: message
    });

    notification.onclick = () => {
      console.log('Notification clicked');
    };
  }

  getAllLogs(): Observable<any> {
    return this.http.get<any>(`${environment.apiHost}authentication/getAllLogs`);
  }
}
