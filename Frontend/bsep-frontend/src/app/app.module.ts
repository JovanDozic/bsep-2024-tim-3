import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserModule } from 'src/features/user/user.module';
import { LayoutModule } from 'src/features/layout/layout.module';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { JwtInterceptor } from 'src/features/user/jwt/jwt.interceptor';
import { RecaptchaModule } from 'ng-recaptcha';
import { NotificationsModule } from 'src/features/notifications/notifications.module';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    LayoutModule,
    UserModule,
    AppRoutingModule,
    HttpClientModule,
    RecaptchaModule,
    NotificationsModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }, // Opciono, možete registrovati interceptor kao HTTP_INTERCEPTOR
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
