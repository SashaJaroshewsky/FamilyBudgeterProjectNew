// src/api/financialGoalApi.ts
import api from './api';
import { 
  FinancialGoal, 
  CreateFinancialGoal, 
  UpdateFinancialGoal,
  UpdateFinancialGoalAmount,
  FinancialGoalStatus
} from '../models/FinancialGoalModels';

export const financialGoalApi = {
  /**
   * Отримання всіх фінансових цілей бюджету
   */
  getBudgetGoals: async (budgetId: number): Promise<FinancialGoal[]> => {
    const response = await api.get<FinancialGoal[]>(`/FinancialGoal/budget/${budgetId}`);
    return response.data;
  },

  /**
   * Отримання фінансової цілі за ідентифікатором
   */
  getGoalById: async (id: number): Promise<FinancialGoal> => {
    const response = await api.get<FinancialGoal>(`/FinancialGoal/${id}`);
    return response.data;
  },

  /**
   * Створення нової фінансової цілі
   */
  createGoal: async (data: CreateFinancialGoal): Promise<FinancialGoal> => {
    const response = await api.post<FinancialGoal>('/FinancialGoal', data);
    return response.data;
  },

  /**
   * Оновлення фінансової цілі
   */
  updateGoal: async (id: number, data: UpdateFinancialGoal): Promise<FinancialGoal> => {
    const response = await api.put<FinancialGoal>(`/FinancialGoal/${id}`, data);
    return response.data;
  },

  /**
   * Видалення фінансової цілі
   */
  deleteGoal: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/FinancialGoal/${id}`);
    return response.data.success;
  },

  /**
   * Оновлення суми фінансової цілі
   */
  updateGoalAmount: async (id: number, data: UpdateFinancialGoalAmount): Promise<FinancialGoal> => {
    const response = await api.patch<FinancialGoal>(`/FinancialGoal/${id}/amount`, data);
    return response.data;
  },

  /**
   * Отримання цілей за статусом
   */
  getGoalsByStatus: async (budgetId: number, status: FinancialGoalStatus): Promise<FinancialGoal[]> => {
    const response = await api.get<FinancialGoal[]>(`/FinancialGoal/budget/${budgetId}/status/${status}`);
    return response.data;
  },

  /**
   * Перевірка та оновлення статусу цілі
   */
  checkAndUpdateGoalStatus: async (id: number): Promise<boolean> => {
    const response = await api.post<{ statusChanged: boolean }>(`/FinancialGoal/${id}/check-status`);
    return response.data.statusChanged;
  }
};