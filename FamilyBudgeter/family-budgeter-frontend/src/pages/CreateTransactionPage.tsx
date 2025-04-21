import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Container, Card, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import { transactionApi } from '../api/transactionApi';
import { budgetApi } from '../api/budgetApi';
import { categoryApi } from '../api/categoryApi';
import { CreateTransaction, TransactionType } from '../models/TransactionModels';
import { Budget } from '../models/BudgetModels';
import { Category } from '../models/CategoryModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';


const CreateTransactionPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedBudgetId = searchParams.get('budgetId');

  const [formData, setFormData] = useState<CreateTransaction>({
    amount: 0,
    description: '',
    date: new Date(),
    categoryId: 0,
    budgetId: preselectedBudgetId ? parseInt(preselectedBudgetId, 10) : 0,
  });

  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [transactionType, setTransactionType] = useState<TransactionType>(TransactionType.Expense);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [file, setFile] = useState<File | null>(null);
  const [validated, setValidated] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);

        if (preselectedBudgetId) {
          const budgetCategories = await categoryApi.getBudgetCategories(parseInt(preselectedBudgetId, 10));
          setCategories(budgetCategories);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Помилка завантаження даних');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [preselectedBudgetId]);

  const handleBudgetChange = async (budgetId: string) => {
    try {
      setFormData(prev => ({ ...prev, budgetId: parseInt(budgetId, 10), categoryId: 0 }));
      if (budgetId) {
        const budgetCategories = await categoryApi.getBudgetCategories(parseInt(budgetId, 10));
        setCategories(budgetCategories.filter(c => 
          transactionType === TransactionType.Income ? 
          c.type === 'Income' : c.type === 'Expense'
        ));
      } else {
        setCategories([]);
      }
    } catch (err) {
      setError('Помилка завантаження категорій');
    }
  };

  const handleTypeChange = (type: TransactionType) => {
    setTransactionType(type);
    setFormData(prev => ({ ...prev, categoryId: 0 }));
    if (formData.budgetId) {
      const filteredCategories = categories.filter(c => 
        type === TransactionType.Income ? c.type === 'Income' : c.type === 'Expense'
      );
      setCategories(filteredCategories);
    }
  };

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

      // Create transaction
      const newTransaction = await transactionApi.createTransaction({
        ...formData,
        amount: Number(formData.amount) // Ensure amount is a number
      });

      // Upload receipt if exists
      if (file) {
        try {
          await transactionApi.uploadReceiptImage(newTransaction.id, file);
        } catch (uploadError) {
          console.error('Failed to upload receipt:', uploadError);
          // Continue with navigation even if receipt upload fails
        }
      }

      // Add small delay for better UX
      await new Promise(resolve => setTimeout(resolve, 500));

      // Navigate based on context
      if (preselectedBudgetId) {
        navigate(`/budgets/${preselectedBudgetId}/transactions`);
      } else {
        navigate('/transactions');
      }
    } catch (err) {
      console.error('Error creating transaction:', err);
      setError(
        err instanceof Error && err.message 
          ? err.message 
          : 'Помилка при створенні транзакції. Спробуйте ще раз.'
      );
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
                <h1 className="h3 mb-0">Нова транзакція</h1>
              </Card.Header>

              <Card.Body>
                {error && (
                  <Alert variant="danger" className="mb-4" dismissible onClose={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form noValidate validated={validated} onSubmit={handleSubmit}>
                  <Form.Group className="mb-3">
                    <Form.Label>Тип транзакції</Form.Label>
                    <div>
                      <Form.Check
                        inline
                        type="radio"
                        name="transactionType"
                        id="expense"
                        label="Витрата"
                        checked={transactionType === TransactionType.Expense}
                        onChange={() => handleTypeChange(TransactionType.Expense)}
                      />
                      <Form.Check
                        inline
                        type="radio"
                        name="transactionType"
                        id="income"
                        label="Дохід"
                        checked={transactionType === TransactionType.Income}
                        onChange={() => handleTypeChange(TransactionType.Income)}
                      />
                    </div>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Бюджет</Form.Label>
                    <Form.Select
                      required
                      value={formData.budgetId || ''}
                      onChange={(e) => handleBudgetChange(e.target.value)}
                      disabled={preselectedBudgetId !== null}
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
                    <Form.Label>Категорія</Form.Label>
                    <Form.Select
                      required
                      value={formData.categoryId || ''}
                      onChange={(e) => setFormData(prev => ({ 
                        ...prev, 
                        categoryId: parseInt(e.target.value, 10) 
                      }))}
                      disabled={!formData.budgetId}
                    >
                      <option value="">Виберіть категорію</option>
                      {categories.map(category => (
                        <option key={category.id} value={category.id}>
                          {category.name}
                        </option>
                      ))}
                    </Form.Select>
                    <Form.Control.Feedback type="invalid">
                      Виберіть категорію
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Сума</Form.Label>
                    <Form.Control
                      required
                      type="number"
                      min="0.01"
                      step="0.01"
                      value={formData.amount}
                      onChange={(e) => setFormData(prev => ({ 
                        ...prev, 
                        amount: parseFloat(e.target.value) 
                      }))}
                    />
                    <Form.Control.Feedback type="invalid">
                      Введіть коректну суму
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Опис</Form.Label>
                    <Form.Control
                      required
                      as="textarea"
                      rows={3}
                      value={formData.description}
                      onChange={(e) => setFormData(prev => ({ 
                        ...prev, 
                        description: e.target.value 
                      }))}
                      minLength={3}
                      maxLength={500}
                    />
                    <Form.Control.Feedback type="invalid">
                      Опис має містити від 3 до 500 символів
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Дата</Form.Label>
                    <DatePicker
                      selected={formData.date}
                      onChange={(date: Date | null) => {
                        if (date) {
                          setFormData(prev => ({ ...prev, date }));
                        }
                      }}
                      className="form-control"
                      dateFormat="dd.MM.yyyy"
                      maxDate={new Date()}
                      required
                    />
                  </Form.Group>

                  <Form.Group className="mb-4">
                    <Form.Label>Чек (необов'язково)</Form.Label>
                    <Form.Control
                      type="file"
                      accept="image/*"
                      onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                        if (e.target.files) {
                          setFile(e.target.files[0] || null);
                        }
                      }}
                    />
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
                        'Створити транзакцію'
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

export default CreateTransactionPage;