import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginPasswordlessComponent } from './LoginPasswordlessComponent';

describe('LoginPasswordlessComponent', () => {
  let component: LoginPasswordlessComponent;
  let fixture: ComponentFixture<LoginPasswordlessComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LoginPasswordlessComponent],
    });
    fixture = TestBed.createComponent(LoginPasswordlessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
