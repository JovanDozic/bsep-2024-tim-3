import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationRequestsManagementComponent } from './registration-requests-management.component';

describe('RegistrationRequestsManagementComponent', () => {
  let component: RegistrationRequestsManagementComponent;
  let fixture: ComponentFixture<RegistrationRequestsManagementComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegistrationRequestsManagementComponent]
    });
    fixture = TestBed.createComponent(RegistrationRequestsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
