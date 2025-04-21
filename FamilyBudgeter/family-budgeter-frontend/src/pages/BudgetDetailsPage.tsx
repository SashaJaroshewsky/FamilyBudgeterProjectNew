// src/pages/BudgetDetailsPage.tsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Alert, Table, Badge, ProgressBar, Form } from 'react-bootstrap';
import { budgetApi } from '../api/budgetApi';
import { categoryApi } from '../api/categoryApi';
import { BudgetDetail } from '../models/BudgetModels';
import { CategorySummary } from '../models/CategoryModels';
import { FinancialGoalStatus } from '../models/FinancialGoalModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';

const BudgetDetailsPage: React.FC = () => {
  const { budgetId } = useParams<{ budgetId: string }>();
  const navigate = useNavigate();

  const [budget, setBudget] = useState<BudgetDetail | null>(null);
  const [categorySummaries, setCategorySummaries] = useState<CategorySummary[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [dateRange, setDateRange] = useState<{ startDate: Date; endDate: Date }>({
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
    endDate: new Date()
  });

  useEffect(() => {
    const fetchBudgetDetails = async () => {
      if (!budgetId) return;

      try {
        setLoading(true);

        const budgetDetails = await budgetApi.getBudgetById(parseInt(budgetId, 10));
        setBudget(budgetDetails);

        const summaries = await categoryApi.getCategorySummaries(
          parseInt(budgetId, 10),
          dateRange.startDate,
          dateRange.endDate
        );
        setCategorySummaries(summaries);

      } catch (err) {
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('Виникла помилка при завантаженні деталей бюджету');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchBudgetDetails();
  }, [budgetId, dateRange]);

  const handleDateRangeChange = (type: 'startDate' | 'endDate', value: string) => {
    const date = new Date(value);
    setDateRange(prev => ({
      ...prev,
      [type]: date
    }));
  };

  if (loading) {
    return (
      <>
        <AppHeader />
        <Container className="d-flex justify-content-center align-items-center" style={{ minHeight: '80vh' }}>
          <Loader />
        </Container>
      </>
    );
  }

  if (error) {
    return (
      <>
        <AppHeader />
        <Container className="py-4">
          <Alert variant="danger">{error}</Alert>
        </Container>
      </>
    );
  }

  if (!budget) {
    return (
      <>
        <AppHeader />
        <Container className="py-4">
          <Alert variant="warning">Бюджет не знайдено</Alert>
        </Container>
      </>
    );
  }

  return (
    <>
      <AppHeader />
      <Container className="py-4">
        <Card className="shadow-sm mb-4">
          <Card.Body>
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h1 className="h2 mb-0">{budget.name}</h1>
              <div className="d-flex gap-2">
                <Button variant="outline-primary" onClick={() => navigate(`/budgets/${budget.id}/edit`)}>
                  <i className="bi bi-pencil me-2"></i>Редагувати бюджет
                </Button>
                <Button variant="primary" onClick={() => navigate(`/transactions/create?budgetId=${budget.id}`)}>
                  <i className="bi bi-plus-lg me-2"></i>Додати транзакцію
                </Button>
              </div>
            </div>

            <Row className="g-4">
              <Col md={4}>
                <Card>
                  <Card.Body>
                    <h5 className="card-title mb-3">Інформація про бюджет</h5>
                    <div className="mb-2">
                      <small className="text-muted">Сім'я:</small>
                      <div className="fw-medium">{budget.familyName}</div>
                    </div>
                    <div className="mb-2">
                      <small className="text-muted">Валюта:</small>
                      <div className="fw-medium">{budget.currency}</div>
                    </div>
                    <div>
                      <small className="text-muted">Тип:</small>
                      <div className="fw-medium">{budget.type}</div>
                    </div>
                  </Card.Body>
                </Card>
              </Col>

              <Col md={8}>
                <Card>
                  <Card.Body>
                    <h5 className="card-title mb-3">Період аналізу</h5>
                    <Row className="g-3">
                      <Col sm={6}>
                        <Form.Group>
                          <Form.Label>Початкова дата</Form.Label>
                          <Form.Control
                            type="date"
                            value={dateRange.startDate.toISOString().split('T')[0]}
                            onChange={(e) => handleDateRangeChange('startDate', e.target.value)}
                          />
                        </Form.Group>
                      </Col>
                      <Col sm={6}>
                        <Form.Group>
                          <Form.Label>Кінцева дата</Form.Label>
                          <Form.Control
                            type="date"
                            value={dateRange.endDate.toISOString().split('T')[0]}
                            onChange={(e) => handleDateRangeChange('endDate', e.target.value)}
                          />
                        </Form.Group>
                      </Col>
                    </Row>
                  </Card.Body>
                </Card>
              </Col>
            </Row>

            <Card className="mt-4">
              <Card.Header className="bg-white">
                <div className="d-flex justify-content-between align-items-center">
                  <h5 className="mb-0">Категорії</h5>
                  <Button 
                    variant="outline-primary" 
                    size="sm"
                    onClick={() => navigate(`/categories/create?budgetId=${budget.id}`)}
                  >
                    <i className="bi bi-plus-lg me-2"></i>Додати категорію
                  </Button>
                </div>
              </Card.Header>
              <Card.Body>
                {budget.categories.length > 0 ? (
                  <Table responsive hover>
                    <thead>
                      <tr>
                        <th>Назва</th>
                        <th>Тип</th>
                        <th className="text-end">Дії</th>
                      </tr>
                    </thead>
                    <tbody>
                      {budget.categories.map(category => (
                        <tr key={category.id}>
                          <td>{category.name}</td>
                          <td>
                            <Badge bg="secondary">{category.type}</Badge>
                          </td>
                          <td className="text-end">
                            <Button 
                              variant="link" 
                              size="sm"
                              onClick={() => navigate(`/categories/${category.id}`)}
                            >
                              Деталі
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                ) : (
                  <div className="text-center py-4">
                    <i className="bi bi-folder fs-1 text-muted"></i>
                    <p className="text-muted mt-2 mb-3">Категорії відсутні</p>
                    <Button 
                      variant="outline-primary"
                      onClick={() => navigate(`/categories/create?budgetId=${budget.id}`)}
                    >
                      Створити категорію
                    </Button>
                  </div>
                )}
              </Card.Body>
            </Card>

            <Card className="mt-4">
              <Card.Header className="bg-white">
                <div className="d-flex justify-content-between align-items-center">
                  <h5 className="mb-0">Фінансові цілі</h5>
                  <Button 
                    variant="outline-primary" 
                    size="sm"
                    onClick={() => navigate(`/goals/create?budgetId=${budget.id}`)}
                  >
                    <i className="bi bi-plus-lg me-2"></i>Додати ціль
                  </Button>
                </div>
              </Card.Header>
              <Card.Body>
                {budget.financialGoals.length > 0 ? (
                  <Row xs={1} md={2} lg={3} className="g-4">
                    {budget.financialGoals.map(goal => (
                      <Col key={goal.id}>
                        <Card>
                          <Card.Body>
                            <div className="d-flex justify-content-between align-items-start mb-3">
                              <h6 className="mb-0">{goal.name}</h6>
                              <Badge bg={
                                goal.status === FinancialGoalStatus.Completed ? 'success' :
                                goal.status === FinancialGoalStatus.InProgress ? 'primary' :
                                'warning'
                              }>
                                {goal.status}
                              </Badge>
                            </div>
                            <ProgressBar 
                              now={goal.percentComplete} 
                              label={`${goal.percentComplete.toFixed(0)}%`}
                              className="mb-3"
                            />
                            <div className="small text-muted mb-2">
                              {goal.currentAmount.toFixed(2)} / {goal.targetAmount.toFixed(2)} {budget.currency}
                            </div>
                            <div className="small text-muted">
                              {goal.daysRemaining > 0 ? (
                                <span>Залишилось {goal.daysRemaining} днів</span>
                              ) : (
                                <span className="text-danger">Термін закінчився</span>
                              )}
                            </div>
                          </Card.Body>
                        </Card>
                      </Col>
                    ))}
                  </Row>
                ) : (
                  <div className="text-center py-4">
                    <i className="bi bi-trophy fs-1 text-muted"></i>
                    <p className="text-muted mt-2 mb-3">Фінансові цілі відсутні</p>
                    <Button 
                      variant="outline-primary"
                      onClick={() => navigate(`/goals/create?budgetId=${budget.id}`)}
                    >
                      Створити ціль
                    </Button>
                  </div>
                )}
              </Card.Body>
            </Card>

            <Card className="mt-4">
              <Card.Header className="bg-white">
                <div className="d-flex justify-content-between align-items-center">
                  <h5 className="mb-0">Ліміти бюджету</h5>
                  <Button 
                    variant="outline-primary" 
                    size="sm"
                    onClick={() => navigate(`/budget-limits/create?budgetId=${budget.id}`)}
                  >
                    <i className="bi bi-plus-lg me-2"></i>Додати ліміт
                  </Button>
                </div>
              </Card.Header>
              <Card.Body>
                {budget.limits.length > 0 ? (
                  <Table responsive hover>
                    <thead>
                      <tr>
                        <th>Категорія</th>
                        <th>Ліміт</th>
                        <th>Витрачено</th>
                        <th>Залишилось</th>
                        <th>Період</th>
                      </tr>
                    </thead>
                    <tbody>
                      {budget.limits.map(limit => (
                        <tr key={limit.id}>
                          <td>{limit.categoryName}</td>
                          <td>{limit.amount.toFixed(2)} {budget.currency}</td>
                          <td>{limit.currentSpent.toFixed(2)} {budget.currency}</td>
                          <td>{(limit.amount - limit.currentSpent).toFixed(2)} {budget.currency}</td>
                          <td>
                            {new Date(limit.startDate).toLocaleDateString()} - {new Date(limit.endDate).toLocaleDateString()}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                ) : (
                  <div className="text-center py-4">
                    <i className="bi bi-shield-check fs-1 text-muted"></i>
                    <p className="text-muted mt-2 mb-3">Ліміти бюджету відсутні</p>
                    <Button 
                      variant="outline-primary"
                      onClick={() => navigate(`/budget-limits/create?budgetId=${budget.id}`)}
                    >
                      Створити ліміт
                    </Button>
                  </div>
                )}
              </Card.Body>
            </Card>

            <div className="d-flex gap-2 mt-4">
              <Button 
                variant="outline-primary"
                onClick={() => navigate(`/transactions?budgetId=${budget.id}`)}
              >
                <i className="bi bi-credit-card me-2"></i>Перегляд транзакцій
              </Button>
              <Button 
                variant="outline-primary"
                onClick={() => navigate(`/analysis?budgetId=${budget.id}`)}
              >
                <i className="bi bi-graph-up me-2"></i>Аналіз бюджету
              </Button>
              <Button 
                variant="outline-primary"
                onClick={() => navigate(`/regular-payments?budgetId=${budget.id}`)}
              >
                <i className="bi bi-calendar-check me-2"></i>Регулярні платежі
              </Button>
            </div>
          </Card.Body>
        </Card>
      </Container>
    </>
  );
};

export default BudgetDetailsPage;