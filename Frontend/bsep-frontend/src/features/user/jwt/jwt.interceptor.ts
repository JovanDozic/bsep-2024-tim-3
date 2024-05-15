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
    const accessToken = localStorage.getItem(ACCESS_TOKEN);
    const refreshToken = this.tokenService.getRefreshToken();
    if (!accessToken || !refreshToken) {
      return next.handle(request);
    }

    const accessTokenRequest = request.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    const accessExpiration = this.tokenService.getTokenExpiration(accessToken);
    const refreshExpiration = this.tokenService.getTokenExpiration(refreshToken);
    const now = new Date().getTime();
    if (refreshExpiration.getTime() < now) {
      this.userService.logout();
      alert("Your session has expired");
      return next.handle(request);
    } else if (accessExpiration.getTime() < now) {
      if (this.tokenService.haveSameIdsInTokens(accessToken, refreshToken)) {
        this.userService.updateAccessToken(accessToken, refreshToken, parseInt(this.tokenService.getTokenId(refreshToken))).subscribe(
          response => {
            this.tokenService.saveToken(response);
            return next.handle(accessTokenRequest);
          },
          error => {
            console.error("Error updating access token:", error);
            this.userService.logout();
            alert("Your session has expired");
            return next.handle(request);
          }
        );
      } else {
        this.userService.logout();
        alert("Your session has expired");
        return next.handle(request);
      }
    } else {
      return next.handle(accessTokenRequest);
    }
  }
}
