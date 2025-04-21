// src/api/notificationApi.ts
import api from './api';
import { 
  Notification, 
  NotificationType,
  NotificationFilter
} from '../models/NotificationModels';

export const notificationApi = {
  /**
   * Отримання всіх сповіщень користувача
   */
  getUserNotifications: async (): Promise<Notification[]> => {
    const response = await api.get<Notification[]>('/notification');
    return response.data;
  },

  /**
   * Отримання непрочитаних сповіщень
   */
  getUnreadNotifications: async (): Promise<Notification[]> => {
    const response = await api.get<Notification[]>('/notification/unread');
    return response.data;
  },

  /**
   * Отримання сповіщення за ідентифікатором
   */
  getNotificationById: async (id: number): Promise<Notification> => {
    const response = await api.get<Notification>(`/notification/${id}`);
    return response.data;
  },

  /**
   * Позначення сповіщення як прочитаного
   */
  markAsRead: async (id: number): Promise<boolean> => {
    const response = await api.post<{ success: boolean }>(`/notification/${id}/mark-read`);
    return response.data.success;
  },

  /**
   * Позначення всіх сповіщень як прочитаних
   */
  markAllAsRead: async (): Promise<boolean> => {
    const response = await api.post<{ success: boolean }>('/notification/mark-all-read');
    return response.data.success;
  },

  /**
   * Видалення сповіщення
   */
  deleteNotification: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/notification/${id}`);
    return response.data.success;
  },

  /**
   * Отримання сповіщень за типом
   */
  getNotificationsByType: async (type: NotificationType): Promise<Notification[]> => {
    const response = await api.get<Notification[]>(`/notification/by-type/${type}`);
    return response.data;
  },

  /**
   * Фільтрація сповіщень
   */
  filterNotifications: async (filter: NotificationFilter): Promise<Notification[]> => {
    const response = await api.post<Notification[]>('/notification/filter', filter);
    return response.data;
  }
};