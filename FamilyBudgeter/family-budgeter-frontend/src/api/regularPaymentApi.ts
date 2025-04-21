// src/api/regularPaymentApi.ts
import api from './api';
import { 
  RegularPayment, 
  CreateRegularPayment, 
  UpdateRegularPayment,
  PaymentFrequency
} from '../models/RegularPaymentModels';

export const regularPaymentApi = {
  /**
   * Отримання всіх регулярних платежів бюджету
   */
  getBudgetRegularPayments: async (budgetId: number): Promise<RegularPayment[]> => {
    const response = await api.get<RegularPayment[]>(`/regular-payment/budget/${budgetId}`);
    return response.data;
  },

  /**
   * Отримання регулярного платежу за ідентифікатором
   */
  getRegularPaymentById: async (id: number): Promise<RegularPayment> => {
    const response = await api.get<RegularPayment>(`/regular-payment/${id}`);
    return response.data;
  },

  /**
   * Створення нового регулярного платежу
   */
  createRegularPayment: async (data: CreateRegularPayment): Promise<RegularPayment> => {
    const response = await api.post<RegularPayment>('/regular-payment', data);
    return response.data;
  },

  /**
   * Оновлення регулярного платежу
   */
  updateRegularPayment: async (id: number, data: UpdateRegularPayment): Promise<RegularPayment> => {
    const response = await api.put<RegularPayment>(`/regular-payment/${id}`, data);
    return response.data;
  },

  /**
   * Видалення регулярного платежу
   */
  deleteRegularPayment: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/regular-payment/${id}`);
    return response.data.success;
  },

  /**
   * Отримання регулярних платежів за категорією
   */
  getRegularPaymentsByCategory: async (categoryId: number): Promise<RegularPayment[]> => {
    const response = await api.get<RegularPayment[]>(`/regular-payment/category/${categoryId}`);
    return response.data;
  },

  /**
   * Отримання регулярних платежів за частотою
   */
  getRegularPaymentsByFrequency: async (budgetId: number, frequency: PaymentFrequency): Promise<RegularPayment[]> => {
    const response = await api.get<RegularPayment[]>(`/regular-payment/budget/${budgetId}/frequency/${frequency}`);
    return response.data;
  },

  /**
   * Отримання майбутніх регулярних платежів
   */
  getUpcomingRegularPayments: async (budgetId: number, startDate: Date, endDate: Date): Promise<RegularPayment[]> => {
    const response = await api.get<RegularPayment[]>(`/regular-payment/budget/${budgetId}/upcoming`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  }
};