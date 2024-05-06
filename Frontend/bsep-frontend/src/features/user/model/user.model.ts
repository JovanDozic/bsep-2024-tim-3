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
    type: ClientType
}

export enum ClientType {
    LegalEntity = 1,
    NaturalPerson
  }