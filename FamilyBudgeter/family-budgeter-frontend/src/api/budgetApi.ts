// src/api/budgetApi.ts
import api from './api';
import { 
  Budget, 
  BudgetDetail, 
  CreateBudget, 
  UpdateBudget, 
  BudgetSummary,
  BudgetType
} from '../models/BudgetModels';
import { familyApi } from './familyApi';

export const budgetApi = {
  /**
   * Отримання всіх бюджетів сім'ї
   */
  getFamilyBudgets: async (familyId: number): Promise<Budget[]> => {
    const response = await api.get<Budget[]>(`/budget/family/${familyId}`);
    return response.data;
  },

  /**
   * Отримання бюджету за ідентифікатором
   */
  getBudgetById: async (id: number): Promise<BudgetDetail> => {
    const response = await api.get<BudgetDetail>(`/budget/${id}`);
    return response.data;
  },

  /**
   * Створення нового бюджету
   */
  createBudget: async (data: CreateBudget): Promise<Budget> => {
    const response = await api.post<Budget>('/budget', data);
    return response.data;
  },

  /**
   * Оновлення бюджету
   */
  updateBudget: async (id: number, data: UpdateBudget): Promise<Budget> => {
    const response = await api.put<Budget>(`/budget/${id}`, data);
    return response.data;
  },

  /**
   * Видалення бюджету
   */
  deleteBudget: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/budget/${id}`);
    return response.data.success;
  },

  /**
   * Отримання бюджетів за типом
   */
  getBudgetsByType: async (familyId: number, type: BudgetType): Promise<Budget[]> => {
    const response = await api.get<Budget[]>(`/budget/family/${familyId}/type/${type}`);
    return response.data;
  },

  /**
   * Отримання фінансового підсумку бюджету за період
   */
  getBudgetSummary: async (id: number, startDate: Date, endDate: Date): Promise<BudgetSummary> => {
    const response = await api.get<BudgetSummary>(`/budget/${id}/summary`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  },

  /**
   * Отримання бюджетів користувача
   */
  getUserBudgets: async (): Promise<Budget[]> => {
    // Get user's families first
    const families = await familyApi.getUserFamilies();
    
    // Then get budgets for each family
    const budgetsPromises = families.map(family => 
      api.get<Budget[]>(`/budget/family/${family.id}`)
    );
    
    const budgetResponses = await Promise.all(budgetsPromises);
    
    // Combine all budgets into a single array
    return budgetResponses.flatMap(response => response.data);
  },
};