import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/app/env/environment';
import { Observable } from 'rxjs';
import { SignalrService } from '../signalr.service';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.css']
})
export class LogsComponent implements OnInit {
  logs: string[] = [];
  

  constructor(private http: HttpClient, private SignalrService: SignalrService) { }

  ngOnInit(): void {
    this.SignalrService.getAllLogs().subscribe(
      logs => {
        this.logs = logs;
      },
      error => {
        console.log(error);
      }
    );
  }
  
}
