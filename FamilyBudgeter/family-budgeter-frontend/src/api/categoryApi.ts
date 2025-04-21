// src/api/categoryApi.ts
import api from './api';
import { 
  Category, 
  CategoryDetail,
  CreateCategory, 
  UpdateCategory, 
  CategorySummary,
  CategoryType
} from '../models/CategoryModels';

export const categoryApi = {
  /**
   * Отримання всіх категорій бюджету
   */
  getBudgetCategories: async (budgetId: number): Promise<Category[]> => {
    const response = await api.get<Category[]>(`/category/budget/${budgetId}`);
    return response.data;
  },

  /**
   * Отримання категорії за ідентифікатором
   */
  getCategoryById: async (id: number): Promise<CategoryDetail> => {
    const response = await api.get<CategoryDetail>(`/category/${id}`);
    return response.data;
  },

  /**
   * Створення нової категорії
   */
  createCategory: async (data: CreateCategory): Promise<Category> => {
    const response = await api.post<Category>('/category', data);
    return response.data;
  },

  /**
   * Оновлення категорії
   */
  updateCategory: async (id: number, data: UpdateCategory): Promise<Category> => {
    const response = await api.put<Category>(`/category/${id}`, data);
    return response.data;
  },

  /**
   * Видалення категорії
   */
  deleteCategory: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/category/${id}`);
    return response.data.success;
  },

  /**
   * Отримання категорій за типом
   */
  getCategoriesByType: async (budgetId: number, type: CategoryType): Promise<Category[]> => {
    const response = await api.get<Category[]>(`/category/budget/${budgetId}/type/${type}`);
    return response.data;
  },

  /**
   * Отримання підсумків категорій за період
   */
  getCategorySummaries: async (budgetId: number, startDate: Date, endDate: Date): Promise<CategorySummary[]> => {
    const response = await api.get<CategorySummary[]>(`/category/budget/${budgetId}/summary`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  }
};