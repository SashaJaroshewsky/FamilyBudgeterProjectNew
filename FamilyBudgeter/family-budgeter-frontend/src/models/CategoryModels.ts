import { Transaction } from './TransactionModels';
import { BudgetLimit } from './BudgetLimitModels';

export enum CategoryType {
    Expense = 'Expense',
    Income = 'Income'
}

export interface Category {
    id: number;
    name: string;
    icon?: string;
    type: CategoryType;
    budgetId: number;
}

export interface CategoryDetail extends Category {
    transactions: Transaction[];
    limits: BudgetLimit[];
    currentAmount: number;
    budgetLimit?: number;
    percentOfLimit?: number;
}

export interface CreateCategory {
    name: string;
    icon?: string;
    type: CategoryType;
    budgetId: number;
}

export interface UpdateCategory {
    name: string;
    icon?: string;
    type: CategoryType;
}

export interface CategorySummary {
    amount: number;
    transactionsCount: number;
    limit?: number;
    percentOfLimit: number;
}