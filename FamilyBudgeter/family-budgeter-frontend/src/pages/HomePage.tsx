// src/pages/HomePage.tsx
import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Alert } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';
import { familyApi } from '../api/familyApi';
import { budgetApi } from '../api/budgetApi';
import { notificationApi } from '../api/notificationApi';
import { Family } from '../models/FamilyModels';
import { Budget } from '../models/BudgetModels';
import { Notification } from '../models/NotificationModels';
import Loader from '../components/common/Loader';
import AppHeader from '../components/common/AppHeader';
import FamilyCard from '../components/family/FamilyCard';
import BudgetCard from '../components/budget/BudgetCard';
import NotificationList from '../components/notification/NotificationList';

const HomePage: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  
  const [loading, setLoading] = useState<boolean>(true);
  const [families, setFamilies] = useState<Family[]>([]);
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        // Отримуємо сім'ї користувача
        const userFamilies = await familyApi.getUserFamilies();
        setFamilies(userFamilies);
        
        // Якщо є сім'ї, отримуємо їх бюджети
        if (userFamilies.length > 0) {
          const userBudgets = [];
          for (const family of userFamilies) {
            const familyBudgets = await budgetApi.getFamilyBudgets(family.id);
            userBudgets.push(...familyBudgets);
          }
          setBudgets(userBudgets);
        }
        
        // Отримуємо непрочитані сповіщення користувача
        const userNotifications = await notificationApi.getUnreadNotifications();
        setNotifications(userNotifications);
        
      } catch (err) {
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('Виникла помилка при завантаженні даних');
        }
      } finally {
        setLoading(false);
      }
    };
    
    fetchData();
  }, []);

  const handleCreateFamily = () => {
    navigate('/families/create');
  };

  const handleCreateBudget = () => {
    navigate('/budgets/create');
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
    <div className="min-vh-100 bg-light">
      <AppHeader />
      
      <Container className="py-4">
        <Card className="shadow-sm mb-4">
          <Card.Body>
            <h1 className="display-6">Вітаємо, {user?.firstName}!</h1>
            <p className="lead text-muted">Ласкаво просимо до FamilyBudgeter</p>
          </Card.Body>
        </Card>
        
        {error && (
          <Alert variant="danger" className="mb-4">
            {error}
          </Alert>
        )}
        
        <Row className="mb-4">
          <Col>
            <Card className="shadow-sm h-100">
              <Card.Header className="d-flex justify-content-between align-items-center bg-white">
                <h2 className="h5 mb-0">Мої сім'ї</h2>
                <Button variant="primary" size="sm" onClick={handleCreateFamily}>
                  <i className="bi bi-plus-lg me-2"></i>Створити сім'ю
                </Button>
              </Card.Header>
              <Card.Body>
                {families.length > 0 ? (
                  <Row xs={1} md={2} lg={3} className="g-4">
                    {families.map(family => (
                      <Col key={family.id}>
                        <FamilyCard family={family} />
                      </Col>
                    ))}
                  </Row>
                ) : (
                  <div className="text-center py-4">
                    <i className="bi bi-people fs-1 text-muted"></i>
                    <p className="mt-3 mb-3">У вас ще немає сімей</p>
                    <Button variant="outline-primary" onClick={handleCreateFamily}>
                      Створити сім'ю
                    </Button>
                  </div>
                )}
              </Card.Body>
            </Card>
          </Col>
        </Row>

        <Row className="mb-4">
          <Col>
            <Card className="shadow-sm h-100">
              <Card.Header className="d-flex justify-content-between align-items-center bg-white">
                <h2 className="h5 mb-0">Мої бюджети</h2>
                <Button 
                  variant="primary" 
                  size="sm"
                  onClick={handleCreateBudget}
                  disabled={families.length === 0}
                >
                  <i className="bi bi-wallet2 me-2"></i>Створити бюджет
                </Button>
              </Card.Header>
              <Card.Body>
                {budgets.length > 0 ? (
                  <Row xs={1} md={2} lg={3} className="g-4">
                    {budgets.map(budget => (
                      <Col key={budget.id}>
                        <BudgetCard budget={budget} />
                      </Col>
                    ))}
                  </Row>
                ) : (
                  <div className="text-center py-4">
                    <i className="bi bi-cash-stack fs-1 text-muted"></i>
                    <p className="mt-3 mb-2">У вас ще немає бюджетів</p>
                    {families.length > 0 ? (
                      <Button variant="outline-primary" onClick={handleCreateBudget}>
                        Створити бюджет
                      </Button>
                    ) : (
                      <p className="text-muted">Спочатку створіть сім'ю</p>
                    )}
                  </div>
                )}
              </Card.Body>
            </Card>
          </Col>
        </Row>

        <Row>
          <Col>
            <Card className="shadow-sm">
              <Card.Header className="d-flex justify-content-between align-items-center bg-white">
                <h2 className="h5 mb-0">Останні сповіщення</h2>
                <Button 
                  variant="link" 
                  onClick={() => navigate('/notifications')}
                  className="text-decoration-none"
                >
                  Переглянути всі
                </Button>
              </Card.Header>
              <Card.Body>
                {notifications.length > 0 ? (
                  <NotificationList notifications={notifications} limit={5} />
                ) : (
                  <div className="text-center py-4">
                    <i className="bi bi-bell fs-1 text-muted"></i>
                    <p className="mt-3">У вас немає непрочитаних сповіщень</p>
                  </div>
                )}
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </div>
  );
};

export default HomePage;