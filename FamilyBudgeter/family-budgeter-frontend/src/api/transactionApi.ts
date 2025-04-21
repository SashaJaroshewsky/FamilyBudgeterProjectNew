// src/api/transactionApi.ts
import api from './api';
import { 
  Transaction, 
  CreateTransaction, 
  UpdateTransaction, 
  TransactionFilter 
} from '../models/TransactionModels';

export const transactionApi = {
  /**
   * Отримання всіх транзакцій бюджету
   */
  getBudgetTransactions: async (budgetId: number): Promise<Transaction[]> => {
    const response = await api.get<Transaction[]>(`/transaction/budget/${budgetId}`);
    return response.data;
  },

  /**
   * Отримання транзакції за ідентифікатором
   */
  getTransactionById: async (id: number): Promise<Transaction> => {
    const response = await api.get<Transaction>(`/transaction/${id}`);
    return response.data;
  },

  /**
   * Створення нової транзакції
   */
  createTransaction: async (data: CreateTransaction): Promise<Transaction> => {
    const response = await api.post<Transaction>('/transaction', data);
    return response.data;
  },

  /**
   * Оновлення транзакції
   */
  updateTransaction: async (id: number, data: UpdateTransaction): Promise<Transaction> => {
    const response = await api.put<Transaction>(`/transaction/${id}`, data);
    return response.data;
  },

  /**
   * Видалення транзакції
   */
  deleteTransaction: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/transaction/${id}`);
    return response.data.success;
  },

  /**
   * Отримання транзакцій за категорією
   */
  getTransactionsByCategory: async (categoryId: number): Promise<Transaction[]> => {
    const response = await api.get<Transaction[]>(`/transaction/category/${categoryId}`);
    return response.data;
  },

  /**
   * Отримання транзакцій за діапазоном дат
   */
  getTransactionsByDateRange: async (budgetId: number, startDate: Date, endDate: Date): Promise<Transaction[]> => {
    const response = await api.get<Transaction[]>(`/transaction/budget/${budgetId}/date-range`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  },

  /**
   * Отримання власних транзакцій користувача
   */
  getUserTransactions: async (): Promise<Transaction[]> => {
    const response = await api.get<Transaction[]>(`/transaction/my-transactions`);
    return response.data;
  },

  /**
   * Фільтрація транзакцій
   */
  filterTransactions: async (filter: TransactionFilter): Promise<Transaction[]> => {
    const response = await api.post<Transaction[]>(`/transaction/filter`, filter);
    return response.data;
  },

  /**
   * Завантаження зображення чеку
   */
  uploadReceiptImage: async (transactionId: number, file: File): Promise<string> => {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await api.post<{ imageUrl: string }>(
      `/transaction/${transactionId}/receipt`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      }
    );
    
    return response.data.imageUrl;
  }
};