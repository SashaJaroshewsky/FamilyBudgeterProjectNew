export interface DateAmountPair {
    date: Date;
    amount: number;
}

export interface CategoryComparison {
    categoryName: string;
    budgeted: number;
    actual: number;
    difference: number;
    percentageUsed: number;
}

export interface BudgetComparison {
    categories: CategoryComparison[];
    totalBudgeted: number;
    totalActual: number;
    totalDifference: number;
}

export interface CategoryExpense {
    categoryName: string;
    amount: number;
}

export interface UserExpense {
    userName: string;
    amount: number;
}

export interface CategoryExpenseChange {
    categoryName: string;
    currentPeriod: number;
    previousPeriod: number;
    change: number;
    changePercent: number;
}

export interface ExpenseChangeAnalysis {
    categoryChanges: CategoryExpenseChange[];
    totalCurrentPeriod: number;
    totalPreviousPeriod: number;
    totalChange: number;
    totalChangePercent: number;
}