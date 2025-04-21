// src/components/auth/RegisterForm.tsx
import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { UserRegistration } from '../../models/AuthModels';

const RegisterForm: React.FC = () => {
  const { register, loading } = useAuth();
  const [formData, setFormData] = useState<UserRegistration>({
    email: '',
    password: '',
    firstName: '',
    lastName: ''
  });
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState<{ 
    email?: string; 
    password?: string;
    confirmPassword?: string;
    firstName?: string;
    lastName?: string;
  }>({});

  const validateForm = () => {
    const newErrors: { 
      email?: string; 
      password?: string;
      confirmPassword?: string;
      firstName?: string;
      lastName?: string;
    } = {};
    let isValid = true;

    // Валідація email
    if (!formData.email) {
      newErrors.email = 'Поле "Електронна пошта" обов\'язкове';
      isValid = false;
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Невірний формат електронної пошти';
      isValid = false;
    }

    // Валідація імені
    if (!formData.firstName) {
      newErrors.firstName = 'Поле "Ім\'я" обов\'язкове';
      isValid = false;
    }

    // Валідація прізвища
    if (!formData.lastName) {
      newErrors.lastName = 'Поле "Прізвище" обов\'язкове';
      isValid = false;
    }

    // Валідація пароля
    if (!formData.password) {
      newErrors.password = 'Поле "Пароль" обов\'язкове';
      isValid = false;
    } else if (formData.password.length < 6) {
      newErrors.password = 'Пароль має містити щонайменше 6 символів';
      isValid = false;
    }

    // Валідація підтвердження пароля
    if (formData.password !== confirmPassword) {
      newErrors.confirmPassword = 'Паролі не співпадають';
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    if (name === 'confirmPassword') {
      setConfirmPassword(value);
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      await register(formData);
    }
  };

  return (
    <div className="register-form">
      <h2>Реєстрація</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="firstName">Ім'я</label>
          <input
            type="text"
            id="firstName"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            disabled={loading}
          />
          {errors.firstName && <div className="error">{errors.firstName}</div>}
        </div>
        
        <div className="form-group">
          <label htmlFor="lastName">Прізвище</label>
          <input
            type="text"
            id="lastName"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            disabled={loading}
          />
          {errors.lastName && <div className="error">{errors.lastName}</div>}
        </div>
        
        <div className="form-group">
          <label htmlFor="email">Електронна пошта</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            disabled={loading}
          />
          {errors.email && <div className="error">{errors.email}</div>}
        </div>
        
        <div className="form-group">
          <label htmlFor="password">Пароль</label>
          <input
            type="password"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            disabled={loading}
          />
          {errors.password && <div className="error">{errors.password}</div>}
        </div>
        
        <div className="form-group">
          <label htmlFor="confirmPassword">Підтвердження пароля</label>
          <input
            type="password"
            id="confirmPassword"
            name="confirmPassword"
            value={confirmPassword}
            onChange={handleChange}
            disabled={loading}
          />
          {errors.confirmPassword && <div className="error">{errors.confirmPassword}</div>}
        </div>
        
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Завантаження...' : 'Зареєструватися'}
        </button>
      </form>
    </div>
  );
};

export default RegisterForm;