import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { environment } from 'src/app/env/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection;
  notificationReceived: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7198/notificationHub`)
      .build();
    
      this.hubConnection.start()
      .then(() => console.log('SignalR connected'))
      .catch(err => {
        console.error('SignalR connecting error: ', err);
        console.log('Connection URL:', `https://localhost:7198/notificationHub`);
      });
    

    this.hubConnection.on('ReceiveNotification', (message: string) => {
      console.log('Received message:', message);
      this.notificationReceived.emit(message);
    });
  }

  getAllLogs(): Observable<any> {
    return this.http.get<any>(`${environment.apiHost}authentication/getAllLogs`);
  }
}
