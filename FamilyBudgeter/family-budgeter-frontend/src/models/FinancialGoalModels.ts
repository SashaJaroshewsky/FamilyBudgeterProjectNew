export enum FinancialGoalStatus {
  Completed = 'Completed',
  InProgress = 'InProgress',
  Planned = 'Planned'
}

export interface FinancialGoal {
  id: number;
  name: string;
  status: FinancialGoalStatus;
  currentAmount: number;
  targetAmount: number;
  percentComplete: number;
  daysRemaining: number;
}

export interface CreateFinancialGoal {
    name: string;
    description?: string;
    targetAmount: number;
    currentAmount: number;
    deadline: Date;
    budgetId: number;
}

export interface UpdateFinancialGoal {
    name: string;
    description?: string;
    targetAmount: number;
    currentAmount: number;
    deadline: Date;
    status: FinancialGoalStatus;
}

export interface UpdateFinancialGoalAmount {
    currentAmount: number;
}