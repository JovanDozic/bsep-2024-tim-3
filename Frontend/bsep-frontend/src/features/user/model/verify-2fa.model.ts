export interface Verify2faRequest {
  email?: string;
  tempToken?: string;
  code: string;
}
