import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Form, Button, Alert } from 'react-bootstrap';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { financialGoalApi } from '../api/financialGoalApi';
import { budgetApi } from '../api/budgetApi';
import { CreateFinancialGoal } from '../models/FinancialGoalModels';
import { Budget } from '../models/BudgetModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';

const CreateFinancialGoalPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedBudgetId = searchParams.get('budgetId');

  const [formData, setFormData] = useState<CreateFinancialGoal>({
    name: '',
    description: '',
    targetAmount: 0,
    currentAmount: 0,
    deadline: new Date(),
    budgetId: preselectedBudgetId ? parseInt(preselectedBudgetId, 10) : 0
  });

  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [validated, setValidated] = useState(false);

  useEffect(() => {
    const loadBudgets = async () => {
      try {
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);
        
        if (!preselectedBudgetId && userBudgets.length > 0) {
          setFormData(prev => ({ ...prev, budgetId: userBudgets[0].id }));
        }
      } catch (err) {
        setError('Помилка завантаження списку бюджетів');
      } finally {
        setLoading(false);
      }
    };

    loadBudgets();
  }, [preselectedBudgetId]);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;

    if (!form.checkValidity() || formData.currentAmount > formData.targetAmount) {
      event.stopPropagation();
      setValidated(true);
      return;
    }

    try {
      setSubmitting(true);
      setError(null);
      
      const newGoal = await financialGoalApi.createGoal(formData);
      navigate(`/budgets/${formData.budgetId}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Помилка створення фінансової цілі');
    } finally {
      setSubmitting(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'budgetId' ? parseInt(value, 10) : 
              (name === 'targetAmount' || name === 'currentAmount') ? parseFloat(value) : 
              name === 'deadline' ? new Date(value) : value
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

  return (
    <>
      <AppHeader />
      <Container className="py-4">
        <Row className="justify-content-center">
          <Col md={8} lg={6}>
            <Card className="shadow-sm">
              <Card.Header className="bg-white py-3">
                <h1 className="h3 mb-0">Створення фінансової цілі</h1>
              </Card.Header>

              <Card.Body>
                {error && (
                  <Alert variant="danger" dismissible onClose={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form noValidate validated={validated} onSubmit={handleSubmit}>
                  <Form.Group className="mb-3">
                    <Form.Label>Назва цілі</Form.Label>
                    <Form.Control
                      required
                      type="text"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      disabled={submitting}
                      placeholder="Введіть назву цілі"
                      minLength={3}
                      maxLength={50}
                    />
                    <Form.Control.Feedback type="invalid">
                      Назва цілі обов'язкова (від 3 до 50 символів)
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Опис</Form.Label>
                    <Form.Control
                      as="textarea"
                      rows={3}
                      name="description"
                      value={formData.description}
                      onChange={handleInputChange}
                      disabled={submitting}
                      placeholder="Додайте опис цілі (необов'язково)"
                      maxLength={200}
                    />
                    <Form.Text className="text-muted">
                      Максимум 200 символів
                    </Form.Text>
                  </Form.Group>

                  <Row className="mb-3">
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Цільова сума</Form.Label>
                        <Form.Control
                          required
                          type="number"
                          name="targetAmount"
                          value={formData.targetAmount}
                          onChange={handleInputChange}
                          disabled={submitting}
                          min={0}
                          step={0.01}
                        />
                        <Form.Control.Feedback type="invalid">
                          Введіть коректну цільову суму
                        </Form.Control.Feedback>
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Початкова сума</Form.Label>
                        <Form.Control
                          required
                          type="number"
                          name="currentAmount"
                          value={formData.currentAmount}
                          onChange={handleInputChange}
                          disabled={submitting}
                          min={0}
                          step={0.01}
                          isInvalid={validated && formData.currentAmount > formData.targetAmount}
                        />
                        <Form.Control.Feedback type="invalid">
                          Початкова сума не може перевищувати цільову
                        </Form.Control.Feedback>
                      </Form.Group>
                    </Col>
                  </Row>

                  <Form.Group className="mb-3">
                    <Form.Label>Дата досягнення</Form.Label>
                    <Form.Control
                      required
                      type="date"
                      name="deadline"
                      value={formData.deadline.toISOString().split('T')[0]}
                      onChange={handleInputChange}
                      disabled={submitting}
                      min={new Date().toISOString().split('T')[0]}
                    />
                    <Form.Control.Feedback type="invalid">
                      Оберіть дату досягнення цілі
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-4">
                    <Form.Label>Бюджет</Form.Label>
                    <Form.Select
                      required
                      name="budgetId"
                      value={formData.budgetId}
                      onChange={handleInputChange}
                      disabled={submitting || !!preselectedBudgetId}
                    >
                      <option value="">Виберіть бюджет</option>
                      {budgets.map(budget => (
                        <option key={budget.id} value={budget.id}>
                          {budget.name}
                        </option>
                      ))}
                    </Form.Select>
                    <Form.Control.Feedback type="invalid">
                      Виберіть бюджет для цілі
                    </Form.Control.Feedback>
                  </Form.Group>

                  <div className="d-flex gap-2">
                    <Button
                      variant="secondary"
                      onClick={() => navigate(-1)}
                      disabled={submitting}
                    >
                      Скасувати
                    </Button>
                    <Button
                      type="submit"
                      variant="primary"
                      disabled={submitting}
                    >
                      {submitting ? (
                        <>
                          <span className="spinner-border spinner-border-sm me-2" />
                          Створення...
                        </>
                      ) : (
                        'Створити ціль'
                      )}
                    </Button>
                  </div>
                </Form>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default CreateFinancialGoalPage;