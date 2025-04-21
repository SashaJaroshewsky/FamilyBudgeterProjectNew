// src/api/transactionApi.ts
import api from './api';
import { 
  Transaction, 
  CreateTransaction, 
  UpdateTransaction, 
  TransactionFilter, 
  TransactionType
} from '../models/TransactionModels';
import { categoryApi } from './categoryApi';
import { Category, CategoryType } from '../models/CategoryModels';  

export const transactionApi = {
  /**
   * Отримання всіх транзакцій бюджету
   */
  getBudgetTransactions: async (budgetId: number): Promise<Transaction[]> => {
    // Get transactions
    const response = await api.get<Transaction[]>(`/transaction/budget/${budgetId}`);
    
    // Get all categories for this budget to determine transaction types
    const categories = await categoryApi.getBudgetCategories(budgetId);
    const categoryMap = new Map(categories.map(cat => [cat.id, cat]));

    // Map transactions and set their types based on category
    const transactions = response.data.map(transaction => ({
      ...transaction,
      type: categoryMap.get(transaction.categoryId)?.type === CategoryType.Income 
        ? TransactionType.Income 
        : TransactionType.Expense
    }));

    return transactions;
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
    
    // Get category to determine transaction type
    const category = await categoryApi.getCategoryById(data.categoryId);
    
    return {
      ...response.data,
      type: category.type === CategoryType.Income 
        ? TransactionType.Income 
        : TransactionType.Expense
    };
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