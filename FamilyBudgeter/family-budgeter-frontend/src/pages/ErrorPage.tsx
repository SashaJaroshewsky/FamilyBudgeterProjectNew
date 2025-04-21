// src/pages/ErrorPage.tsx
import React from 'react';
import { useNavigate } from 'react-router-dom';

const ErrorPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div className="error-page">
      <div className="error-container">
        <div className="error-icon">
          <i className="icon-error"></i>
        </div>
        <h1>Щось пішло не так</h1>
        <p>Сторінку, яку ви шукаєте, не знайдено або виникла помилка.</p>
        <div className="error-actions">
          <button 
            className="btn btn-primary" 
            onClick={() => navigate('/')}
          >
            На головну
          </button>
          <button 
            className="btn btn-outline" 
            onClick={() => navigate(-1)}
          >
            Назад
          </button>
        </div>
      </div>
    </div>
  );
};

export default ErrorPage;