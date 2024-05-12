import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Advertisement } from "./model/advertisement.model";
import { Observable } from "rxjs";
import { environment } from "src/app/env/environment";

@Injectable({
    providedIn: 'root'
  })

export class AdvertisementService {
    constructor(
        private http: HttpClient,
        private router: Router
    ) {}

    createAdvertisement(ad: Advertisement): Observable<boolean> {
        console.log("usao u servis");
        console.log(ad.deadline);
        console.log(ad.description);
        console.log(ad.endDate);
        console.log(ad.startDate);
        return this.http.post<boolean>(`${environment.apiHost}advertisements/create`, ad);
    }
    getAllAdvertisements():Observable<Advertisement[]> {
    
        return this.http.get<Advertisement[]>(environment.apiHost + 'advertisements/getAll');
      }
    updateAdvertisement(ad: Advertisement): Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiHost}advertisements/update`, ad);
    }
}