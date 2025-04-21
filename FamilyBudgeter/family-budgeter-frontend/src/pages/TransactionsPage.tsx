import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Container, Card, Row, Col, Form, Button, Badge, Alert } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import { transactionApi } from '../api/transactionApi';
import { budgetApi } from '../api/budgetApi';
import { categoryApi } from '../api/categoryApi';
import { Transaction, TransactionType, TransactionFilter } from '../models/TransactionModels';
import { Budget } from '../models/BudgetModels';
import { Category } from '../models/CategoryModels';
import AppHeader from '../components/common/AppHeader';
import TransactionItem from '../components/transaction/TransactionItem';
import Loader from '../components/common/Loader';



const TransactionsPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedBudgetId = searchParams.get('budgetId');

  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Фільтри
  const [filters, setFilters] = useState<TransactionFilter>({
    budgetId: preselectedBudgetId ? parseInt(preselectedBudgetId, 10) : undefined,
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
    endDate: new Date(),
    transactionType: undefined, // Changed from 'type' to 'transactionType'
    categoryId: undefined,
    minAmount: undefined,
    maxAmount: undefined,
    searchTerm: undefined
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);

        if (filters.budgetId) {
          const budgetCategories = await categoryApi.getBudgetCategories(filters.budgetId);
          setCategories(budgetCategories);
        }

        const filteredTransactions = await transactionApi.filterTransactions(filters);
        setTransactions(filteredTransactions);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Помилка завантаження даних');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [filters]);

  const handleBudgetChange = async (budgetId: string) => {
    const parsedId = budgetId ? parseInt(budgetId, 10) : undefined;
    setFilters(prev => ({ ...prev, budgetId: parsedId, categoryId: undefined }));

    if (parsedId) {
      try {
        const budgetCategories = await categoryApi.getBudgetCategories(parsedId);
        setCategories(budgetCategories);
      } catch (err) {
        setError('Помилка завантаження категорій');
      }
    } else {
      setCategories([]);
    }
  };

  const handleDeleteTransaction = async (id: number) => {
    if (window.confirm('Ви впевнені, що хочете видалити цю транзакцію?')) {
      try {
        await transactionApi.deleteTransaction(id);
        setTransactions(prev => prev.filter(t => t.id !== id));
      } catch (err) {
        setError('Помилка видалення транзакції');
      }
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
        <Card className="shadow-sm">
          <Card.Body>
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h1 className="h2 mb-0">Транзакції</h1>
              <Button 
                variant="primary"
                onClick={() => navigate(`/transactions/create${filters.budgetId ? `?budgetId=${filters.budgetId}` : ''}`)}
              >
                <i className="bi bi-plus-lg me-2"></i>
                Нова транзакція
              </Button>
            </div>

            {error && (
              <Alert variant="danger" className="mb-4" dismissible onClose={() => setError(null)}>
                {error}
              </Alert>
            )}

            <Card className="mb-4">
              <Card.Body>
                <Row className="g-3">
                  <Col md={4} lg={3}>
                    <Form.Group>
                      <Form.Label>Бюджет</Form.Label>
                      <Form.Select
                        value={filters.budgetId || ''}
                        onChange={(e) => handleBudgetChange(e.target.value)}
                      >
                        <option value="">Всі бюджети</option>
                        {budgets.map(budget => (
                          <option key={budget.id} value={budget.id}>
                            {budget.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col md={4} lg={3}>
                    <Form.Group>
                      <Form.Label>Категорія</Form.Label>
                      <Form.Select
                        value={filters.categoryId || ''}
                        onChange={(e) => setFilters(prev => ({ 
                          ...prev, 
                          categoryId: e.target.value ? parseInt(e.target.value, 10) : undefined 
                        }))}
                        disabled={!filters.budgetId}
                      >
                        <option value="">Всі категорії</option>
                        {categories.map(category => (
                          <option key={category.id} value={category.id}>
                            {category.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col md={4} lg={3}>
                    <Form.Group>
                      <Form.Label>Тип</Form.Label>
                      <Form.Select
                        value={filters.transactionType || ''}
                        onChange={(e) => setFilters(prev => ({ 
                          ...prev, 
                          transactionType: e.target.value as TransactionType | undefined 
                        }))}
                      >
                        <option value="">Всі типи</option>
                        <option value={TransactionType.Income}>Доходи</option>
                        <option value={TransactionType.Expense}>Витрати</option>
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col md={12} lg={3}>
                    <Form.Group>
                      <Form.Label>Період</Form.Label>
                      <div className="d-flex gap-2">
                        <DatePicker
                          selected={filters.startDate}
                          onChange={(date: Date | null) => setFilters(prev => ({ 
                            ...prev, 
                            startDate: date || undefined 
                          }))}
                          className="form-control"
                          dateFormat="dd.MM.yyyy"
                          placeholderText="Початок"
                        />
                        <DatePicker
                          selected={filters.endDate}
                          onChange={(date: Date | null) => setFilters(prev => ({ 
                            ...prev, 
                            endDate: date || undefined 
                          }))}
                          className="form-control"
                          dateFormat="dd.MM.yyyy"
                          placeholderText="Кінець"
                        />
                      </div>
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label>Пошук</Form.Label>
                      <Form.Control
                        type="text"
                        placeholder="Пошук за описом..."
                        value={filters.searchTerm || ''}
                        onChange={(e) => setFilters(prev => ({ 
                          ...prev, 
                          searchTerm: e.target.value || undefined 
                        }))}
                      />
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label>Сума</Form.Label>
                      <div className="d-flex gap-2">
                        <Form.Control
                          type="number"
                          placeholder="Від"
                          value={filters.minAmount || ''}
                          onChange={(e) => setFilters(prev => ({ 
                            ...prev, 
                            minAmount: e.target.value ? parseFloat(e.target.value) : undefined 
                          }))}
                        />
                        <Form.Control
                          type="number"
                          placeholder="До"
                          value={filters.maxAmount || ''}
                          onChange={(e) => setFilters(prev => ({ 
                            ...prev, 
                            maxAmount: e.target.value ? parseFloat(e.target.value) : undefined 
                          }))}
                        />
                      </div>
                    </Form.Group>
                  </Col>
                </Row>
              </Card.Body>
            </Card>

            {transactions.length > 0 ? (
              <div className="transaction-list">
                {transactions.map(transaction => (
                  <TransactionItem
                    key={transaction.id}
                    transaction={transaction}
                    onDelete={handleDeleteTransaction}
                  />
                ))}
              </div>
            ) : (
              <div className="text-center py-5">
                <i className="bi bi-receipt fs-1 text-muted"></i>
                <h3 className="h4 mt-3">Транзакції відсутні</h3>
                <p className="text-muted mb-4">
                  {Object.values(filters).some(value => value !== undefined) ? 
                    'Змініть параметри фільтрації для відображення транзакцій' : 
                    'Створіть свою першу транзакцію для початку ведення обліку'
                  }
                </p>
                <Button 
                  variant="primary"
                  onClick={() => navigate('/transactions/create')}
                >
                  <i className="bi bi-plus-lg me-2"></i>
                  Створити транзакцію
                </Button>
              </div>
            )}
          </Card.Body>
        </Card>
      </Container>
    </>
  );
};

export default TransactionsPage;