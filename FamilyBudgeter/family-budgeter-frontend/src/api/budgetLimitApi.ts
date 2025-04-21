// src/api/budgetLimitApi.ts
import api from './api';
import { 
  BudgetLimit, 
  CreateBudgetLimit, 
  UpdateBudgetLimit
} from '../models/BudgetLimitModels';

export const budgetLimitApi = {
  /**
   * Отримання всіх лімітів бюджету
   */
  getBudgetLimits: async (budgetId: number): Promise<BudgetLimit[]> => {
    const response = await api.get<BudgetLimit[]>(`/budget-limit/budget/${budgetId}`);
    return response.data;
  },

  /**
   * Отримання ліміту за ідентифікатором
   */
  getBudgetLimitById: async (id: number): Promise<BudgetLimit> => {
    const response = await api.get<BudgetLimit>(`/budget-limit/${id}`);
    return response.data;
  },

  /**
   * Створення нового ліміту бюджету
   */
  createBudgetLimit: async (data: CreateBudgetLimit): Promise<BudgetLimit> => {
    const response = await api.post<BudgetLimit>('/budget-limit', data);
    return response.data;
  },

  /**
   * Оновлення ліміту бюджету
   */
  updateBudgetLimit: async (id: number, data: UpdateBudgetLimit): Promise<BudgetLimit> => {
    const response = await api.put<BudgetLimit>(`/budget-limit/${id}`, data);
    return response.data;
  },

  /**
   * Видалення ліміту бюджету
   */
  deleteBudgetLimit: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/budget-limit/${id}`);
    return response.data.success;
  },

  /**
   * Отримання активних лімітів бюджету
   */
  getActiveBudgetLimits: async (budgetId: number): Promise<BudgetLimit[]> => {
    const response = await api.get<BudgetLimit[]>(`/budget-limit/budget/${budgetId}/active`);
    return response.data;
  },

  /**
   * Отримання лімітів за категорією
   */
  getLimitsByCategory: async (categoryId: number): Promise<BudgetLimit[]> => {
    const response = await api.get<BudgetLimit[]>(`/budget-limit/category/${categoryId}`);
    return response.data;
  },

  /**
   * Перевірка, чи перевищено ліміт категорії
   */
  isLimitExceeded: async (categoryId: number): Promise<boolean> => {
    const response = await api.get<{ isExceeded: boolean }>(`/budget-limit/category/${categoryId}/exceeded`);
    return response.data.isExceeded;
  },

  /**
   * Отримання статусів лімітів бюджету
   */
  getLimitStatuses: async (budgetId: number): Promise<BudgetLimit[]> => {
    const response = await api.get<BudgetLimit[]>(`/budget-limit/budget/${budgetId}/status`);
    return response.data;
  }
};