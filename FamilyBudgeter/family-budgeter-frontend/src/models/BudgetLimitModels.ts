export interface BudgetLimit {
    id: number;
    amount: number;
    startDate: Date;
    endDate: Date;
    categoryId: number;
    categoryName: string;
    budgetId: number;
    currentSpent: number;
    percentUsed: number;
}

export interface CreateBudgetLimit {
    amount: number;
    startDate: Date;
    endDate: Date;
    categoryId: number;
    budgetId: number;
}

export interface UpdateBudgetLimit {
    amount: number;
    startDate: Date;
    endDate: Date;
}