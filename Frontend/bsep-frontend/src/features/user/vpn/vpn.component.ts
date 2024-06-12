import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/app/env/environment';
import * as base64 from 'base64-js';


@Component({
  selector: 'app-vpn',
  templateUrl: './vpn.component.html',
  styleUrls: ['./vpn.component.css']
})
export class VpnComponent {
  messageFromComponent: string;
  decodedMessage: string;
  constructor(private http: HttpClient) { }


  getHiddenComponentMessage() {
    this.http.get(`${environment.apiHost}vpn`, { responseType: 'text' }).subscribe(
      (data) => {
        this.messageFromComponent = data;
        const bytes = base64.toByteArray(data);
        this.decodedMessage = new TextDecoder('utf-8').decode(bytes);
      },
      (error) => {
        console.error('Error fetching data:', error);
      }
    );
  }
}