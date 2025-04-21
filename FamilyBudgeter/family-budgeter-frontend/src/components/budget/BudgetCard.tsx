// src/components/budget/BudgetCard.tsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Badge, Button, Spinner, Alert, Row, Col } from 'react-bootstrap';
import { Budget, BudgetType } from '../../models/BudgetModels';
import { budgetApi } from '../../api/budgetApi';


interface BudgetCardProps {
  budget: Budget;
}

const BudgetCard: React.FC<BudgetCardProps> = ({ budget }) => {
  const navigate = useNavigate();
  const [summary, setSummary] = useState<{ income: number; expense: number; balance: number } | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const getBudgetTypeName = (type: BudgetType): string => {
    switch (type) {
      case BudgetType.Monthly:
        return 'Місячний';
      case BudgetType.Yearly:
        return 'Річний';
      case BudgetType.Special:
        return 'Спеціальний';
      default:
        return 'Невідомий';
    }
  };

  useEffect(() => {
    const fetchBudgetSummary = async () => {
      try {
        setLoading(true);

        const now = new Date();
        let startDate: Date, endDate: Date;

        switch (budget.type) {
          case BudgetType.Monthly:
            startDate = new Date(now.getFullYear(), now.getMonth(), 1);
            endDate = new Date(now.getFullYear(), now.getMonth() + 1, 0);
            break;
          case BudgetType.Yearly:
            startDate = new Date(now.getFullYear(), 0, 1);
            endDate = new Date(now.getFullYear(), 11, 31);
            break;
          default:
            startDate = new Date(now.getFullYear(), now.getMonth() - 1, now.getDate());
            endDate = now;
            break;
        }

        const budgetSummary = await budgetApi.getBudgetSummary(budget.id, startDate, endDate);

        setSummary({
          income: budgetSummary.totalIncome,
          expense: budgetSummary.totalExpense,
          balance: budgetSummary.balance
        });
      } catch (err) {
        console.error('Помилка отримання підсумку бюджету:', err);
        setError('Не вдалося отримати фінансові дані');
      } finally {
        setLoading(false);
      }
    };

    fetchBudgetSummary();
  }, [budget.id, budget.type]);

  const handleClick = () => {
    navigate(`/budgets/${budget.id}`);
  };

  return (
    <Card 
      className="h-100 shadow-sm budget-card" 
      onClick={handleClick}
    >
      <Card.Header className="bg-white">
        <div className="d-flex justify-content-between align-items-center">
          <h5 className="mb-0">{budget.name}</h5>
          <Badge bg="secondary">{getBudgetTypeName(budget.type)}</Badge>
        </div>
      </Card.Header>
      
      <Card.Body>
        <Row className="mb-3">
          <Col xs={6}>
            <small className="text-muted d-block">Сім'я</small>
            <span className="fw-medium">{budget.familyName}</span>
          </Col>
          <Col xs={6} className="text-end">
            <small className="text-muted d-block">Валюта</small>
            <span className="fw-medium">{budget.currency}</span>
          </Col>
        </Row>
        
        {loading ? (
          <div className="text-center py-3">
            <Spinner animation="border" size="sm" className="me-2" />
            <span className="text-muted">Завантаження...</span>
          </div>
        ) : error ? (
          <Alert variant="danger" className="mb-0 py-2">
            {error}
          </Alert>
        ) : summary && (
          <div className="budget-summary">
            <Row className="g-2">
              <Col xs={12}>
                <div className="p-2 bg-light rounded">
                  <small className="text-muted d-block">Доходи</small>
                  <span className="text-success fw-medium">
                    {summary.income.toFixed(2)} {budget.currency}
                  </span>
                </div>
              </Col>
              <Col xs={12}>
                <div className="p-2 bg-light rounded">
                  <small className="text-muted d-block">Витрати</small>
                  <span className="text-danger fw-medium">
                    {summary.expense.toFixed(2)} {budget.currency}
                  </span>
                </div>
              </Col>
              <Col xs={12}>
                <div className="p-2 bg-light rounded">
                  <small className="text-muted d-block">Баланс</small>
                  <span className={`fw-medium ${summary.balance >= 0 ? 'text-success' : 'text-danger'}`}>
                    {summary.balance.toFixed(2)} {budget.currency}
                  </span>
                </div>
              </Col>
            </Row>
          </div>
        )}
      </Card.Body>
      
      <Card.Footer className="bg-white border-top">
        <div className="d-flex gap-2 justify-content-end">
          <Button 
            variant="outline-primary" 
            size="sm"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/budgets/${budget.id}`);
            }}
          >
            <i className="bi bi-info-circle me-1"></i>
            Деталі
          </Button>
          <Button 
            variant="outline-success" 
            size="sm"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/transactions/create?budgetId=${budget.id}`);
            }}
          >
            <i className="bi bi-plus-lg me-1"></i>
            Додати транзакцію
          </Button>
        </div>
      </Card.Footer>
    </Card>
  );
};

export default BudgetCard;