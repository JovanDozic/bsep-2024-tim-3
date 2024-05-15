import { Injectable } from "@angular/core";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable } from "rxjs";
import { ACCESS_TOKEN } from "src/app/shared/constants";
import { TokenStorage } from "./token.service";
import { UserService } from "../user.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private tokenService: TokenStorage, private userService: UserService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const accessTokenRequest = request.clone({
      setHeaders: {
        Authorization: `Bearer ` + localStorage.getItem(ACCESS_TOKEN),
      },
    });
    console.log('Interceptor activated:', request.url);
    const accessExpiration = this.tokenService.getTokenExpiration(ACCESS_TOKEN);
    const refreshToken = this.tokenService.getRefreshToken();
    const refreshExpiration = this.tokenService.getTokenExpiration(refreshToken);
    
    const now = new Date().getTime();
    if (refreshExpiration.getTime() < now) {
      console.log("validan")
      this.userService.logout()
      alert("Your session has expired")
    } else {
      if (accessExpiration.getTime() < now) {
        if(this.tokenService.haveSameIdsInTokens(ACCESS_TOKEN, refreshToken)){
          this.userService.updateAccessToken(ACCESS_TOKEN, refreshToken, parseInt(this.tokenService.getTokenId(refreshToken))).subscribe(
            response => {
              this.tokenService.saveToken(response);
            }
          );
        }
        console.log("nije")
      }
      return next.handle(accessTokenRequest);
    }

    return next.handle(accessTokenRequest);
  }
}