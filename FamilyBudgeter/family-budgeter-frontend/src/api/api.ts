// src/api/api.ts
import axios, { AxiosError, InternalAxiosRequestConfig, AxiosResponse } from 'axios';
import { AuthResponse } from '../models/AuthModels';

// API базова URL з .env змінної або за замовчуванням
const API_URL =   'https://localhost:7023/api';

// Створюємо екземпляр axios з налаштуваннями
const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Інтерцептор запитів - додає токен до заголовків
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Інтерцептор відповідей - оновлює токен або виконує вихід при необхідності
api.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config;
    
    // Перевіряємо, чи містить originalRequest властивість _retry
    // і робимо приведення типів для безпечного використання
    if (error.response?.status === 401 && originalRequest && !(originalRequest as any)._retry) {
      (originalRequest as any)._retry = true;
      
      try {
        // Спроба оновити токен
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          // Немає refreshToken, виконуємо вихід
          handleLogout();
          return Promise.reject(error);
        }
        
        const token = localStorage.getItem('token');
        const response = await axios.post<AuthResponse>(`${API_URL}/auth/refresh-token`, { 
          token, 
          refreshToken 
        });
        
        if (response.data.success && response.data.token) {
          // Зберігаємо нові токени
          localStorage.setItem('token', response.data.token);
          if (response.data.refreshToken) {
            localStorage.setItem('refreshToken', response.data.refreshToken);
          }
          
          // Повторюємо оригінальний запит з новим токеном
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
          }
          return api(originalRequest);
        } else {
          // Помилка оновлення токена, виконуємо вихід
          handleLogout();
          return Promise.reject(error);
        }
      } catch (refreshError) {
        // Помилка при спробі оновити токен, виконуємо вихід
        handleLogout();
        return Promise.reject(refreshError);
      }
    }
    
    return Promise.reject(error);
  }
);

// Функція для виходу користувача при проблемах з аутентифікацією
const handleLogout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('user');
  
  // Перенаправлення на сторінку входу
  window.location.href = '/login';
};

export default api;