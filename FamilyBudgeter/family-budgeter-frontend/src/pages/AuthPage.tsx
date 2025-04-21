// src/pages/AuthPage.tsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import LoginForm from '../components/auth/LoginForm';
import RegisterForm from '../components/auth/RegisterForm';

interface AuthPageProps {
  type: 'login' | 'register';
}

const AuthPage: React.FC<AuthPageProps> = ({ type }) => {
  const [pageType, setPageType] = useState<'login' | 'register'>(type);
  const { isAuthenticated, loading, error, clearError } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
  // Перенаправлення на головну сторінку, якщо користувач вже авторизований
  useEffect(() => {
    if (isAuthenticated) {
      const { from } = location.state as { from?: { pathname: string } } || {};
      navigate(from?.pathname || '/', { replace: true });
    }
  }, [isAuthenticated, navigate, location]);

  // Очищення помилок при зміні типу сторінки
  useEffect(() => {
    clearError();
  }, [pageType, clearError]);

  // Переключення між формами логіну та реєстрації
  const togglePageType = () => {
    setPageType(prevType => prevType === 'login' ? 'register' : 'login');
  };

  return (
    <div className="auth-page">
      <div className="auth-container">
        <div className="auth-logo">
          <h1>FamilyBudgeter</h1>
          <p>Планування та контроль сімейних фінансів</p>
        </div>
        
        {pageType === 'login' ? (
          <>
            <LoginForm />
            <div className="auth-footer">
              <p>Немає облікового запису? <button onClick={togglePageType}>Зареєструватися</button></p>
            </div>
          </>
        ) : (
          <>
            <RegisterForm />
            <div className="auth-footer">
              <p>Вже маєте обліковий запис? <button onClick={togglePageType}>Увійти</button></p>
            </div>
          </>
        )}
        
        {error && <div className="auth-error">{error}</div>}
      </div>
    </div>
  );
};

export default AuthPage;