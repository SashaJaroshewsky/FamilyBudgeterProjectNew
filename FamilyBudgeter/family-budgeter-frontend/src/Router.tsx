// src/Router.tsx
import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import PrivateRoute from './components/common/PrivateRoute';

// Сторінки
import HomePage from './pages/HomePage';
import AuthPage from './pages/AuthPage';
import FamilyPage from './pages/FamilyPage';
import BudgetPage from './pages/BudgetPage';
import BudgetDetailsPage from './pages/BudgetDetailsPage';
import TransactionsPage from './pages/TransactionsPage';
import FinancialGoalsPage from './pages/FinancialGoalsPage';
import RegularPaymentsPage from './pages/RegularPaymentsPage';
import AnalysisPage from './pages/AnalysisPage';
import NotificationsPage from './pages/NotificationsPage';
import ProfilePage from './pages/ProfilePage';
import SettingsPage from './pages/SettingsPage';
import ErrorPage from './pages/ErrorPage';
import CreateBudgetPage from './pages/CreateBudgetPage';
import CreateTransactionPage from './pages/CreateTransactionPage';
import CategoriesPage from './pages/CategoriesPage';
import CreateCategoryPage from './pages/CreateCategoryPage';

const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* Публічні маршрути */}
      <Route path="/login" element={<AuthPage type="login" />} />
      <Route path="/register" element={<AuthPage type="register" />} />

      {/* Захищені маршрути */}
      <Route path="/" element={
        <PrivateRoute>
          <HomePage />
        </PrivateRoute>
      } />

      {/* Управління сім'ями */}
      <Route path="/families" element={
        <PrivateRoute>
          <FamilyPage />
        </PrivateRoute>
      } />
      <Route path="/families/:familyId" element={
        <PrivateRoute>
          <FamilyPage isDetail />
        </PrivateRoute>
      } />

      {/* Управління бюджетами */}
      <Route path="/budgets" element={
        <PrivateRoute>
          <BudgetPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/:budgetId" element={
        <PrivateRoute>
          <BudgetDetailsPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/create" element={
        <PrivateRoute>
          <CreateBudgetPage />
        </PrivateRoute>
      } />

      {/* Транзакції */}
      <Route path="/transactions/create" element={
        <PrivateRoute>
          <CreateTransactionPage />
        </PrivateRoute>
      } />
      <Route path="/transactions" element={
        <PrivateRoute>
          <TransactionsPage />
        </PrivateRoute>
      } />

      {/* Фінансові цілі */}
      <Route path="/goals" element={
        <PrivateRoute>
          <FinancialGoalsPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/:budgetId/goals" element={
        <PrivateRoute>
          <FinancialGoalsPage />
        </PrivateRoute>
      } />

      {/* Регулярні платежі */}
      <Route path="/regular-payments" element={
        <PrivateRoute>
          <RegularPaymentsPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/:budgetId/regular-payments" element={
        <PrivateRoute>
          <RegularPaymentsPage />
        </PrivateRoute>
      } />

      {/* Аналіз */}
      <Route path="/analysis" element={
        <PrivateRoute>
          <AnalysisPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/:budgetId/analysis" element={
        <PrivateRoute>
          <AnalysisPage />
        </PrivateRoute>
      } />

      {/* Категорії */}
      <Route path="/categories" element={
        <PrivateRoute>
          <CategoriesPage />
        </PrivateRoute>
      } />
      <Route path="/categories/create" element={
        <PrivateRoute>
          <CreateCategoryPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/:budgetId/categories" element={
        <PrivateRoute>
          <CategoriesPage />
        </PrivateRoute>
      } />
      <Route path="/budgets/:budgetId/categories/create" element={
        <PrivateRoute>
          <CreateCategoryPage />
        </PrivateRoute>
      } />

      {/* Сповіщення */}
      <Route path="/notifications" element={
        <PrivateRoute>
          <NotificationsPage />
        </PrivateRoute>
      } />

      {/* Профіль */}
      <Route path="/profile" element={
        <PrivateRoute>
          <ProfilePage />
        </PrivateRoute>
      } />

      {/* Налаштування */}
      <Route path="/settings" element={
        <PrivateRoute>
          <SettingsPage />
        </PrivateRoute>
      } />

      {/* Обробка помилок */}
      <Route path="/error" element={<ErrorPage />} />
      <Route path="*" element={<Navigate to="/error" replace />} />
    </Routes>
  );
};

export default AppRoutes;