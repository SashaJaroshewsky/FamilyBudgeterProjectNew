// src/api/authApi.ts
import api from './api';
import { UserRegistration, UserLogin, AuthResponse, User, ChangePassword, RefreshToken } from '../models/AuthModels';

export const authApi = {
  /**
   * Реєстрація нового користувача
   */
  register: async (data: UserRegistration): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/register', data);
    return response.data;
  },

  /**
   * Вхід в систему
   */
  login: async (data: UserLogin): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/login', data);
    return response.data;
  },

  /**
   * Оновлення токену
   */
  refreshToken: async (data: RefreshToken): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/refresh-token', data);
    return response.data;
  },

  /**
   * Отримання інформації про поточного користувача
   */
  getCurrentUser: async (): Promise<User> => {
    const response = await api.get<User>('/auth/me');
    return response.data;
  },

  /**
   * Оновлення інформації користувача
   */
  updateUser: async (userData: User): Promise<User> => {
    const response = await api.put<User>('/auth/me', userData);
    return response.data;
  },

  /**
   * Зміна пароля
   */
  changePassword: async (passwordData: ChangePassword): Promise<boolean> => {
    const response = await api.post<{ success: boolean }>('/auth/change-password', passwordData);
    return response.data.success;
  }
};