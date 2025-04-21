// src/api/analysisApi.ts
import api from './api';
import {
  DateAmountPair,
  BudgetComparison,
  CategoryExpense,
  UserExpense,
  ExpenseChangeAnalysis
} from '../models/AnalysisModels';
import { BudgetSummary } from '../models/BudgetModels';
import { CategorySummary } from '../models/CategoryModels';

export const analysisApi = {
  /**
   * Отримання загальних підсумків бюджету
   */
  getBudgetSummary: async (budgetId: number, startDate: Date, endDate: Date): Promise<BudgetSummary> => {
    const response = await api.get<BudgetSummary>(`/analysis/budget/${budgetId}/summary`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  },

  /**
   * Отримання підсумків по категоріях
   */
  getCategorySummaries: async (budgetId: number, startDate: Date, endDate: Date): Promise<CategorySummary[]> => {
    const response = await api.get<CategorySummary[]>(`/analysis/budget/${budgetId}/category-summaries`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  },

  /**
   * Отримання тенденцій витрат
   */
  getExpenseTrend: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date, 
    groupBy: string = 'month'
  ): Promise<Record<string, number>> => {
    const response = await api.get<Record<string, number>>(`/analysis/budget/${budgetId}/expense-trend`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
        groupBy
      }
    });
    return response.data;
  },

  /**
   * Отримання тенденцій доходів
   */
  getIncomeTrend: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date, 
    groupBy: string = 'month'
  ): Promise<Record<string, number>> => {
    const response = await api.get<Record<string, number>>(`/analysis/budget/${budgetId}/income-trend`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
        groupBy
      }
    });
    return response.data;
  },

  /**
   * Порівняння бюджету з фактичними витратами
   */
  compareBudgetWithActual: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date
  ): Promise<BudgetComparison> => {
    const response = await api.get<BudgetComparison>(`/analysis/budget/${budgetId}/compare-with-actual`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  },

  /**
   * Прогноз витрат
   */
  forecastExpenses: async (budgetId: number, months: number = 3): Promise<Record<string, number>> => {
    const response = await api.get<Record<string, number>>(`/analysis/budget/${budgetId}/forecast`, {
      params: { months }
    });
    return response.data;
  },

  /**
   * Аналіз топ витрат
   */
  analyzeTopExpenses: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date, 
    limit: number = 10
  ): Promise<Record<string, number>> => {
    const response = await api.get<Record<string, number>>(`/analysis/budget/${budgetId}/top-expenses`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
        limit
      }
    });
    return response.data;
  },

  /**
   * Аналіз топ доходів
   */
  analyzeTopIncomes: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date, 
    limit: number = 10
  ): Promise<Record<string, number>> => {
    const response = await api.get<Record<string, number>>(`/analysis/budget/${budgetId}/top-incomes`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
        limit
      }
    });
    return response.data;
  },

  /**
   * Аналіз витрат за користувачами
   */
  analyzeExpensesByUser: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date
  ): Promise<Record<string, number>> => {
    const response = await api.get<Record<string, number>>(`/analysis/budget/${budgetId}/expenses-by-user`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  },

  /**
   * Аналіз змін витрат
   */
  analyzeExpenseChanges: async (
    budgetId: number, 
    startDate: Date, 
    endDate: Date
  ): Promise<ExpenseChangeAnalysis> => {
    const response = await api.get<ExpenseChangeAnalysis>(`/analysis/budget/${budgetId}/expense-changes`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
    return response.data;
  }
};