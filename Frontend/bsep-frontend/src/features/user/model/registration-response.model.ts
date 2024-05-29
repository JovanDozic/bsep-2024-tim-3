export interface RegistrationResponse {
  email?: string;
  isSuccess: boolean;
  message?: string;
  isTwoFactorEnabled?: boolean;
  twoFactorQrCode?: string;
}
