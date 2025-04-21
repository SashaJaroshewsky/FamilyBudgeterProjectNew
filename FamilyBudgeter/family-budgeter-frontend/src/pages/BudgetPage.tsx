// src/pages/BudgetPage.tsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Alert, Form } from 'react-bootstrap';
import { budgetApi } from '../api/budgetApi';
import { familyApi } from '../api/familyApi';
import { Budget } from '../models/BudgetModels';
import { Family } from '../models/FamilyModels';
import AppHeader from '../components/common/AppHeader';
import BudgetCard from '../components/budget/BudgetCard';
import Loader from '../components/common/Loader';

const BudgetPage: React.FC = () => {
  const navigate = useNavigate();

  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [families, setFamilies] = useState<Family[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedFamilyId, setSelectedFamilyId] = useState<number | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const userFamilies = await familyApi.getUserFamilies();
        setFamilies(userFamilies);

        if (userFamilies.length > 0) {
          const allBudgets: Budget[] = [];

          for (const family of userFamilies) {
            const familyBudgets = await budgetApi.getFamilyBudgets(family.id);
            allBudgets.push(...familyBudgets);
          }

          setBudgets(allBudgets);
        }
      } catch (err) {
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('Виникла помилка при завантаженні бюджетів');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleCreateBudget = () => {
    navigate('/budgets/create');
  };

  const filterBudgetsByFamily = (familyId: number | null) => {
    setSelectedFamilyId(familyId);
  };

  const filteredBudgets = selectedFamilyId
    ? budgets.filter(budget => budget.familyId === selectedFamilyId)
    : budgets;

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
              <h1 className="h2 mb-0">Мої бюджети</h1>
              <Button
                variant="primary"
                onClick={handleCreateBudget}
                disabled={families.length === 0}
              >
                <i className="bi bi-plus-lg me-2"></i>
                Створити бюджет
              </Button>
            </div>

            {error && (
              <Alert variant="danger" className="mb-4">
                {error}
              </Alert>
            )}

            {families.length > 0 && (
              <Card className="mb-4">
                <Card.Body>
                  <Form.Group>
                    <Form.Label>Фільтрувати за сім'єю:</Form.Label>
                    <Form.Select
                      value={selectedFamilyId?.toString() || ""}
                      onChange={(e) => filterBudgetsByFamily(e.target.value ? parseInt(e.target.value, 10) : null)}
                      className="w-auto"
                    >
                      <option value="">Всі сім'ї</option>
                      {families.map(family => (
                        <option key={family.id} value={family.id}>
                          {family.name}
                        </option>
                      ))}
                    </Form.Select>
                  </Form.Group>
                </Card.Body>
              </Card>
            )}

            {filteredBudgets.length > 0 ? (
              <Row xs={1} md={2} lg={3} className="g-4">
                {filteredBudgets.map(budget => (
                  <Col key={budget.id}>
                    <BudgetCard budget={budget} />
                  </Col>
                ))}
              </Row>
            ) : (
              <Card className="text-center p-5 bg-light border-0">
                <Card.Body>
                  <i className="bi bi-wallet2 fs-1 text-muted mb-3 d-block"></i>
                  <h3 className="h4 mb-3">Бюджети не знайдено</h3>
                  {families.length > 0 ? (
                    <>
                      <p className="text-muted mb-4">
                        Створіть свій перший бюджет, щоб почати контролювати свої фінанси
                      </p>
                      <Button variant="primary" onClick={handleCreateBudget}>
                        <i className="bi bi-plus-lg me-2"></i>
                        Створити бюджет
                      </Button>
                    </>
                  ) : (
                    <p className="text-muted mb-0">
                      Спочатку потрібно створити сім'ю, щоб мати можливість створювати бюджети
                    </p>
                  )}
                </Card.Body>
              </Card>
            )}
          </Card.Body>
        </Card>
      </Container>
    </>
  );
};

export default BudgetPage;