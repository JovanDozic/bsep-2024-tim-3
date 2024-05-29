export interface User {
  id: number;
  email: string;
  password: string;
  firstname?: string;
  lastname?: string;
  address: string;
  city: string;
  country: string;
  phone: string;
  companyName?: string;
  taxId?: string;
  packageType: number;
  clientType: number;
  role: number; // Change type to number
  isTwoFactorEnabled?: boolean;
}
