// src/components/common/AppHeader.tsx
import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { notificationApi } from '../../api/notificationApi';
import NotificationBadge from '../notification/NotificationBadge';
import { Navbar, Container, Nav, NavDropdown, Badge } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

const AppHeader: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [unreadCount, setUnreadCount] = useState<number>(0);

  useEffect(() => {
    const fetchUnreadCount = async () => {
      try {
        const notifications = await notificationApi.getUnreadNotifications();
        setUnreadCount(notifications.length);
      } catch (error) {
        console.error('Помилка отримання сповіщень:', error);
      }
    };

    fetchUnreadCount();
    const interval = setInterval(fetchUnreadCount, 5 * 60 * 1000);
    return () => clearInterval(interval);
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Navbar bg="light" expand="lg" className="shadow-sm">
      <Container>
        <Navbar.Brand as={Link} to="/">
          FamilyBudgeter
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/">Головна</Nav.Link>
            <Nav.Link as={Link} to="/families">Сім'ї</Nav.Link>
            <Nav.Link as={Link} to="/budgets">Бюджети</Nav.Link>
            <Nav.Link as={Link} to="/transactions">Транзакції</Nav.Link>
            <Nav.Link as={Link} to="/categories">Категорії</Nav.Link> {/* Додана нова кнопка */}
            <Nav.Link as={Link} to="/goals">Цілі</Nav.Link>
            <Nav.Link as={Link} to="/analysis">Аналіз</Nav.Link>
          </Nav>
          
          <Nav>
            <Nav.Link as={Link} to="/notifications" className="position-relative">
              <i className="bi bi-bell"></i>
              {unreadCount > 0 && (
                <Badge bg="danger" pill className="position-absolute top-0 start-100 translate-middle">
                  {unreadCount}
                </Badge>
              )}
            </Nav.Link>
            
            <NavDropdown 
              title={user?.firstName || 'Користувач'} 
              id="basic-nav-dropdown"
              align="end"
            >
              <NavDropdown.Item as={Link} to="/profile">Мій профіль</NavDropdown.Item>
              <NavDropdown.Item as={Link} to="/settings">Налаштування</NavDropdown.Item>
              <NavDropdown.Divider />
              <NavDropdown.Item onClick={handleLogout}>Вийти</NavDropdown.Item>
            </NavDropdown>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default AppHeader;