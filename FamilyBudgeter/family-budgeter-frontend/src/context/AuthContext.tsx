// src/context/AuthContext.tsx
import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { authApi } from '../api/authApi';
import { User, UserLogin, UserRegistration, AuthResponse } from '../models/AuthModels';

// Інтерфейс для контексту автентифікації
interface AuthContextType {
  user: User | null;
  loading: boolean;
  error: string | null;
  isAuthenticated: boolean;
  login: (data: UserLogin) => Promise<boolean>;
  register: (data: UserRegistration) => Promise<boolean>;
  logout: () => void;
  updateProfile: (userData: User) => Promise<boolean>;
  clearError: () => void;
}

// Створення контексту
const AuthContext = createContext<AuthContextType>({} as AuthContextType);

// Експорт хука для використання контексту
export const useAuth = () => useContext(AuthContext);

// Провайдер контексту
interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  // Ініціалізація - перевірка токена та завантаження користувача
  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const userData = await authApi.getCurrentUser();
          setUser(userData);
          setIsAuthenticated(true);
        } catch (err) {
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          setError('Сесія закінчилась. Увійдіть знову.');
        }
      }
      setLoading(false);
    };
    
    initAuth();
  }, []);

  // Вхід в систему
  const login = async (data: UserLogin): Promise<boolean> => {
    setLoading(true);
    try {
      const response: AuthResponse = await authApi.login(data);
      
      if (response.success && response.token && response.user) {
        localStorage.setItem('token', response.token);
        if (response.refreshToken) {
          localStorage.setItem('refreshToken', response.refreshToken);
        }
        setUser(response.user);
        setIsAuthenticated(true);
        setError(null);
        return true;
      } else {
        setError(response.message || 'Помилка входу');
        return false;
      }
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Помилка входу');
      }
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Реєстрація
  const register = async (data: UserRegistration): Promise<boolean> => {
    setLoading(true);
    try {
      const response: AuthResponse = await authApi.register(data);
      
      if (response.success && response.token && response.user) {
        localStorage.setItem('token', response.token);
        if (response.refreshToken) {
          localStorage.setItem('refreshToken', response.refreshToken);
        }
        setUser(response.user);
        setIsAuthenticated(true);
        setError(null);
        return true;
      } else {
        setError(response.message || 'Помилка реєстрації');
        return false;
      }
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Помилка реєстрації');
      }
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Вихід
  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setUser(null);
    setIsAuthenticated(false);
  };

  // Оновлення профілю
  const updateProfile = async (userData: User): Promise<boolean> => {
    setLoading(true);
    try {
      const updatedUser = await authApi.updateUser(userData);
      setUser(updatedUser);
      setError(null);
      return true;
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Помилка оновлення профілю');
      }
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Очищення помилок
  const clearError = () => {
    setError(null);
  };

  const contextValue: AuthContextType = {
    user,
    loading,
    error,
    isAuthenticated,
    login,
    register,
    logout,
    updateProfile,
    clearError
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};