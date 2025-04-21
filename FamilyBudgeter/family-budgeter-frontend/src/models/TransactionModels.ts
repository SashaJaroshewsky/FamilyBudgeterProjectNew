export enum TransactionType {
    Income = 'Income',
    Expense = 'Expense'
}

export interface Transaction {
    id: number;
    amount: number;
    description: string;
    date: Date;
    type: TransactionType;
    currency: string;
    receiptImageUrl?: string;
    categoryId: number;
    categoryName: string;
    budgetId: number;
    budgetName: string;
    createdByUserId: number;
    createdByUserName: string;
}

export interface CreateTransaction {
    amount: number;
    description: string;
    date: Date;
    receiptImageUrl?: string;
    categoryId: number;
    budgetId: number;
}

export interface UpdateTransaction {
    amount: number;
    description: string;
    date: Date;
    receiptImageUrl?: string;
    categoryId: number;
}

export interface TransactionFilter {
    budgetId?: number;
    categoryId?: number;
    startDate?: Date;
    endDate?: Date;
    minAmount?: number;
    maxAmount?: number;
    searchTerm?: string;
    transactionType?: TransactionType; // Changed from 'type' to 'transactionType'
}

export interface ReceiptUpload {
    transactionId: number;
    file: File;
}