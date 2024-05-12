export interface Advertisement {
    id: number,
    slogan?: string,
    startDate: Date,
    endDate: Date, 
    description: string, 
    clientId: number
    deadline: Date,
    status: number
}