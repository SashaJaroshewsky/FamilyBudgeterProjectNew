import { Category } from './CategoryModels';
import { FinancialGoal } from './FinancialGoalModels';
import { BudgetLimit } from './BudgetLimitModels';

export enum BudgetType {
    Monthly = 'Monthly',
    Yearly = 'Yearly',
    Special = 'Special'
}

export interface Budget {
    id: number;
    name: string;
    currency: string;
    type: BudgetType;
    familyId: number;
    familyName: string;
}

export interface BudgetDetail extends Budget {
    categories: Category[];
    financialGoals: FinancialGoal[];
    limits: BudgetLimit[];
}

export interface CreateBudget {
    name: string;
    currency: string;
    type: BudgetType;
    familyId: number;
}

export interface UpdateBudget {
    name: string;
    currency: string;
    type: BudgetType;
}

export interface BudgetSummary {
    id: number;
    name: string;
    currency: string;
    totalIncome: number;
    totalExpense: number;
    balance: number;
    startDate: Date;
    endDate: Date;
}