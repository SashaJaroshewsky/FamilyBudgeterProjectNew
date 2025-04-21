import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Container, Card, Row, Col, Form, Button, Alert } from 'react-bootstrap';
import { categoryApi } from '../api/categoryApi';
import { budgetApi } from '../api/budgetApi';
import { Category, CategoryType } from '../models/CategoryModels';
import { Budget } from '../models/BudgetModels';
import AppHeader from '../components/common/AppHeader';
import CategoryItem from '../components/category/CategoryItem';
import Loader from '../components/common/Loader';


const CategoriesPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedBudgetId = searchParams.get('budgetId');

  const [categories, setCategories] = useState<Category[]>([]);
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Фільтри
  const [selectedBudgetId, setSelectedBudgetId] = useState<string>(preselectedBudgetId || '');
  const [selectedType, setSelectedType] = useState<string>('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);

        if (selectedBudgetId) {
          let budgetCategories = await categoryApi.getBudgetCategories(parseInt(selectedBudgetId, 10));
          
          if (selectedType) {
            budgetCategories = budgetCategories.filter(c => c.type === selectedType);
          }
          
          setCategories(budgetCategories);
        } else if (userBudgets.length > 0) {
          const allCategories: Category[] = [];
          for (const budget of userBudgets) {
            const budgetCategories = await categoryApi.getBudgetCategories(budget.id);
            allCategories.push(...(selectedType ? budgetCategories.filter(c => c.type === selectedType) : budgetCategories));
          }
          setCategories(allCategories);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Помилка завантаження даних');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [selectedBudgetId, selectedType]);

  const handleCreateCategory = () => {
    const budgetId = selectedBudgetId || (budgets.length > 0 ? budgets[0].id.toString() : '');
    navigate(`/categories/create${budgetId ? `?budgetId=${budgetId}` : ''}`);
  };

  const handleDeleteCategory = async (categoryId: number) => {
    if (window.confirm('Ви впевнені, що хочете видалити цю категорію?')) {
      try {
        await categoryApi.deleteCategory(categoryId);
        setCategories(prev => prev.filter(c => c.id !== categoryId));
      } catch (err) {
        setError('Помилка видалення категорії');
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
              <h1 className="h2 mb-0">Категорії</h1>
              <Button 
                variant="primary" 
                onClick={handleCreateCategory}
                disabled={budgets.length === 0}
              >
                <i className="bi bi-plus-lg me-2"></i>
                Нова категорія
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
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label>Бюджет</Form.Label>
                      <Form.Select
                        value={selectedBudgetId}
                        onChange={(e) => setSelectedBudgetId(e.target.value)}
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
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label>Тип категорії</Form.Label>
                      <Form.Select
                        value={selectedType}
                        onChange={(e) => setSelectedType(e.target.value)}
                      >
                        <option value="">Всі типи</option>
                        <option value={CategoryType.Income}>Доходи</option>
                        <option value={CategoryType.Expense}>Витрати</option>
                      </Form.Select>
                    </Form.Group>
                  </Col>
                </Row>
              </Card.Body>
            </Card>

            {categories.length > 0 ? (
              <Row xs={1} md={2} lg={3} className="g-4">
                {categories.map(category => (
                  <Col key={category.id}>
                    <CategoryItem
                      category={category}
                      onDelete={handleDeleteCategory}
                    />
                  </Col>
                ))}
              </Row>
            ) : (
              <div className="text-center py-5">
                <i className="bi bi-folder fs-1 text-muted"></i>
                <h3 className="h4 mt-3">Категорії відсутні</h3>
                <p className="text-muted mb-4">
                  {selectedBudgetId || selectedType ? 
                    'Змініть параметри фільтрації для відображення категорій' : 
                    'Створіть свою першу категорію для початку організації транзакцій'
                  }
                </p>
                {budgets.length > 0 && (
                  <Button variant="primary" onClick={handleCreateCategory}>
                    <i className="bi bi-plus-lg me-2"></i>
                    Створити категорію
                  </Button>
                )}
              </div>
            )}
          </Card.Body>
        </Card>
      </Container>
    </>
  );
};

export default CategoriesPage;