export interface User {
    id: number,
    name: string,
    surname: string,
    email: string,
    password: string,
    address: string,
    city: string,
    country: string,
    phone: string,
    type: UserType
}

export enum UserType {
    Admin = 1,
    Employee,
    Client
  }