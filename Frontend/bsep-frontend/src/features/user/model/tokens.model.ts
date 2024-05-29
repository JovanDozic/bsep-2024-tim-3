export interface Tokens {
  id: number;
  accessToken?: string;
  refreshToken?: string;
  isTwoFactorEnabled: boolean;
  tempToken?: string;
}
