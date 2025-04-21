export enum PaymentFrequency {
    Daily = 'Daily',
    Weekly = 'Weekly',
    BiWeekly = 'BiWeekly',
    Monthly = 'Monthly',
    Quarterly = 'Quarterly',
    Yearly = 'Yearly'
}

export interface RegularPayment {
    id: number;
    name: string;
    amount: number;
    description?: string;
    startDate: Date;
    endDate?: Date;
    frequency: PaymentFrequency;
    dayOfMonth: number;
    categoryId: number;
    categoryName: string;
    budgetId: number;
}

export interface CreateRegularPayment {
    name: string;
    amount: number;
    description?: string;
    startDate: Date;
    endDate?: Date;
    frequency: PaymentFrequency;
    dayOfMonth: number;
    categoryId: number;
    budgetId: number;
}

export interface UpdateRegularPayment {
    name: string;
    amount: number;
    description?: string;
    startDate: Date;
    endDate?: Date;
    frequency: PaymentFrequency;
    dayOfMonth: number;
    categoryId: number;
}