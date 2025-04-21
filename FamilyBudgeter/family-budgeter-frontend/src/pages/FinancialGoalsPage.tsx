// src/pages/FinancialGoalsPage.tsx
import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Button, Badge, ProgressBar, Form } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { financialGoalApi } from '../api/financialGoalApi';
import { budgetApi } from '../api/budgetApi';
import { FinancialGoal, FinancialGoalStatus } from '../models/FinancialGoalModels';
import { Budget } from '../models/BudgetModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';

const FinancialGoalsPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [selectedBudgetId, setSelectedBudgetId] = useState<number | null>(null);
  const [goals, setGoals] = useState<FinancialGoal[]>([]);
  const [statusFilter, setStatusFilter] = useState<FinancialGoalStatus | 'all'>('all');

  useEffect(() => {
    const loadBudgets = async () => {
      try {
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);
        if (userBudgets.length > 0) {
          setSelectedBudgetId(userBudgets[0].id);
        }
      } catch (err) {
        setError('Помилка завантаження бюджетів');
      }
    };
    loadBudgets();
  }, []);

  useEffect(() => {
    const loadGoals = async () => {
      if (!selectedBudgetId) return;
      
      try {
        setLoading(true);
        let goalsList: FinancialGoal[];
        
        if (statusFilter === 'all') {
          goalsList = await financialGoalApi.getBudgetGoals(selectedBudgetId);
        } else {
          goalsList = await financialGoalApi.getGoalsByStatus(selectedBudgetId, statusFilter);
        }
        
        setGoals(goalsList);
        setError(null);
      } catch (err) {
        setError('Помилка завантаження фінансових цілей');
      } finally {
        setLoading(false);
      }
    };

    loadGoals();
  }, [selectedBudgetId, statusFilter]);

  const handleDeleteGoal = async (goalId: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цю ціль?')) return;

    try {
      await financialGoalApi.deleteGoal(goalId);
      setGoals(prev => prev.filter(goal => goal.id !== goalId));
    } catch (err) {
      setError('Помилка видалення цілі');
    }
  };

  const getStatusBadgeVariant = (status: FinancialGoalStatus) => {
    switch (status) {
      case FinancialGoalStatus.Completed:
        return 'success';
      case FinancialGoalStatus.InProgress:
        return 'primary';
      case FinancialGoalStatus.Planned:
        return 'warning';
      default:
        return 'secondary';
    }
  };

  return (
    <>
      <AppHeader />
      <Container className="py-4">
        <Card className="shadow-sm">
          <Card.Body>
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h1 className="h3 mb-0">Фінансові цілі</h1>
              <Button 
                variant="primary"
                onClick={() => navigate('/goals/create')}
                disabled={!selectedBudgetId}
              >
                <i className="bi bi-plus-lg me-2"></i>
                Додати ціль
              </Button>
            </div>

            <Row className="g-3 mb-4">
              <Col md={6}>
                <Form.Group>
                  <Form.Label>Бюджет</Form.Label>
                  <Form.Select
                    value={selectedBudgetId || ''}
                    onChange={(e) => setSelectedBudgetId(Number(e.target.value))}
                  >
                    {budgets.map(budget => (
                      <option key={budget.id} value={budget.id}>
                        {budget.name}
                      </option>
                    ))}
                  </Form.Select>
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group>
                  <Form.Label>Статус</Form.Label>
                  <Form.Select
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value as FinancialGoalStatus | 'all')}
                  >
                    <option value="all">Всі статуси</option>
                    {Object.values(FinancialGoalStatus).map(status => (
                      <option key={status} value={status}>
                        {status === FinancialGoalStatus.Completed ? 'Завершені' :
                         status === FinancialGoalStatus.InProgress ? 'В процесі' :
                         'Заплановані'}
                      </option>
                    ))}
                  </Form.Select>
                </Form.Group>
              </Col>
            </Row>

            {loading ? (
              <Loader />
            ) : error ? (
              <div className="text-center text-danger">{error}</div>
            ) : goals.length === 0 ? (
              <div className="text-center py-5">
                <i className="bi bi-trophy display-1 text-muted"></i>
                <p className="text-muted mt-3">
                  У вас поки немає фінансових цілей.
                  <br />
                  Натисніть "Додати ціль" щоб створити нову.
                </p>
              </div>
            ) : (
              <Row xs={1} md={2} lg={3} className="g-4">
                {goals.map(goal => (
                  <Col key={goal.id}>
                    <Card className="h-100">
                      <Card.Body>
                        <div className="d-flex justify-content-between align-items-start mb-3">
                          <h5 className="mb-0">{goal.name}</h5>
                          <Badge bg={getStatusBadgeVariant(goal.status)}>
                            {goal.status === FinancialGoalStatus.Completed ? 'Завершена' :
                             goal.status === FinancialGoalStatus.InProgress ? 'В процесі' :
                             'Запланована'}
                          </Badge>
                        </div>

                        <ProgressBar 
                          now={goal.percentComplete}
                          label={`${goal.percentComplete.toFixed(0)}%`}
                          variant={goal.percentComplete === 100 ? 'success' : 'primary'}
                          className="mb-3"
                        />

                        <div className="small text-muted mb-2">
                          {goal.currentAmount.toLocaleString()} / {goal.targetAmount.toLocaleString()} грн
                        </div>

                        <div className="small text-muted mb-3">
                          {goal.daysRemaining > 0 ? (
                            <span>Залишилось {goal.daysRemaining} днів</span>
                          ) : (
                            <span className="text-danger">Термін закінчився</span>
                          )}
                        </div>

                        <div className="d-flex gap-2">
                          <Button
                            variant="outline-primary"
                            size="sm"
                            onClick={() => navigate(`/goals/${goal.id}`)}
                          >
                            <i className="bi bi-pencil me-1"></i>
                            Редагувати
                          </Button>
                          <Button
                            variant="outline-danger"
                            size="sm"
                            onClick={() => handleDeleteGoal(goal.id)}
                          >
                            <i className="bi bi-trash me-1"></i>
                            Видалити
                          </Button>
                        </div>
                      </Card.Body>
                    </Card>
                  </Col>
                ))}
              </Row>
            )}
          </Card.Body>
        </Card>
      </Container>
    </>
  );
};

export default FinancialGoalsPage;