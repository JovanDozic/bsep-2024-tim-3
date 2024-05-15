export interface RegistrationRequest {
  id: number;
  userId?: number;
  email: string;
  registrationDate: Date;
  status: RegistrationRequestStatus;
  reason?: string;
  tokenExpirationDate?: Date;
}

export enum RegistrationRequestStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
}
