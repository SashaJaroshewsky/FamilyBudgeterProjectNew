import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Container, Card, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { categoryApi } from '../api/categoryApi';
import { budgetApi } from '../api/budgetApi';
import { CreateCategory, CategoryType } from '../models/CategoryModels';
import { Budget } from '../models/BudgetModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';
import './CreateCategoryPage.css';

const CreateCategoryPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedBudgetId = searchParams.get('budgetId');

  const [formData, setFormData] = useState<CreateCategory>({
    name: '',
    type: CategoryType.Expense,
    budgetId: preselectedBudgetId ? parseInt(preselectedBudgetId, 10) : 0,
    icon: ''
  });

  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [validated, setValidated] = useState(false);

  const availableIcons = [
    'bi-cart', 'bi-house', 'bi-car-front', 'bi-hospital',
    'bi-lightning', 'bi-phone', 'bi-gift', 'bi-cup-hot',
    'bi-basket', 'bi-shop', 'bi-bus-front', 'bi-wallet2',
    'bi-piggy-bank', 'bi-cash-coin', 'bi-bank', 'bi-credit-card'
  ];

  useEffect(() => {
    const fetchBudgets = async () => {
      try {
        setLoading(true);
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Помилка завантаження даних');
      } finally {
        setLoading(false);
      }
    };

    fetchBudgets();
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;

    setValidated(true);
    
    if (form.checkValidity() === false) {
      event.stopPropagation();
      return;
    }

    try {
      setSubmitting(true);
      setError(null);

      const newCategory = await categoryApi.createCategory(formData);
      // Додаємо невелику затримку для кращого UX
      await new Promise(resolve => setTimeout(resolve, 500));
      
      // Перевіряємо чи ми в контексті конкретного бюджету
      if (preselectedBudgetId) {
        navigate(`/budgets/${preselectedBudgetId}/categories`);
      } else {
        navigate('/categories');
      }
    } catch (err) {
      console.error('Error creating category:', err);
      setError('Помилка при створенні категорії. Спробуйте ще раз.');
    } finally {
      setSubmitting(false);
    }
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
                <h1 className="h3 mb-0">Створення категорії</h1>
              </Card.Header>

              <Card.Body>
                {error && (
                  <Alert variant="danger" className="mb-4" dismissible onClose={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form noValidate validated={validated} onSubmit={handleSubmit}>
                  <Form.Group className="mb-3">
                    <Form.Label>Тип категорії</Form.Label>
                    <div>
                      <Form.Check
                        inline
                        type="radio"
                        name="categoryType"
                        id="expense"
                        label="Витрата"
                        checked={formData.type === CategoryType.Expense}
                        onChange={() => setFormData(prev => ({ ...prev, type: CategoryType.Expense }))}
                      />
                      <Form.Check
                        inline
                        type="radio"
                        name="categoryType"
                        id="income"
                        label="Дохід"
                        checked={formData.type === CategoryType.Income}
                        onChange={() => setFormData(prev => ({ ...prev, type: CategoryType.Income }))}
                      />
                    </div>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Бюджет</Form.Label>
                    <Form.Select
                      required
                      value={formData.budgetId || ''}
                      onChange={(e) => setFormData(prev => ({ 
                        ...prev, 
                        budgetId: parseInt(e.target.value, 10) 
                      }))}
                      disabled={!!preselectedBudgetId}
                    >
                      <option value="">Виберіть бюджет</option>
                      {budgets.map(budget => (
                        <option key={budget.id} value={budget.id}>
                          {budget.name}
                        </option>
                      ))}
                    </Form.Select>
                    <Form.Control.Feedback type="invalid">
                      Виберіть бюджет
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Назва категорії</Form.Label>
                    <Form.Control
                      required
                      type="text"
                      value={formData.name}
                      onChange={(e) => setFormData(prev => ({ 
                        ...prev, 
                        name: e.target.value 
                      }))}
                      minLength={3}
                      maxLength={50}
                    />
                    <Form.Control.Feedback type="invalid">
                      Назва категорії має містити від 3 до 50 символів
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-4">
                    <Form.Label>Іконка</Form.Label>
                    <div className="icons-grid">
                      {availableIcons.map(icon => (
                        <div
                          key={icon}
                          className={`icon-item ${formData.icon === icon ? 'selected' : ''}`}
                          onClick={() => setFormData(prev => ({ ...prev, icon }))}
                          role="button"
                          tabIndex={0}
                          onKeyDown={(e) => {
                            if (e.key === 'Enter' || e.key === ' ') {
                              setFormData(prev => ({ ...prev, icon }));
                            }
                          }}
                        >
                          <i className={`bi ${icon} fs-4`}></i>
                        </div>
                      ))}
                    </div>
                    {validated && !formData.icon && (
                      <div className="text-danger small mt-2">
                        Будь ласка, виберіть іконку
                      </div>
                    )}
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
                        'Створити категорію'
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

export default CreateCategoryPage;