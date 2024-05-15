import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginPasswordlessAuthenticateComponent } from './login-passwordless-authenticate.component';

describe('LoginPasswordlessAuthenticateComponent', () => {
  let component: LoginPasswordlessAuthenticateComponent;
  let fixture: ComponentFixture<LoginPasswordlessAuthenticateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LoginPasswordlessAuthenticateComponent]
    });
    fixture = TestBed.createComponent(LoginPasswordlessAuthenticateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
