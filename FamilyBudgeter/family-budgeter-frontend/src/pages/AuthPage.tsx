// src/pages/AuthPage.tsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Container, Row, Col, Card, Alert, Button } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';
import LoginForm from '../components/auth/LoginForm';
import RegisterForm from '../components/auth/RegisterForm';

interface AuthPageProps {
  type: 'login' | 'register';
}

const AuthPage: React.FC<AuthPageProps> = ({ type }) => {
  const [pageType, setPageType] = useState<'login' | 'register'>(type);
  const { isAuthenticated, loading, error, clearError } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (isAuthenticated) {
      const { from } = location.state as { from?: { pathname: string } } || {};
      navigate(from?.pathname || '/', { replace: true });
    }
  }, [isAuthenticated, navigate, location]);

  useEffect(() => {
    clearError();
  }, [pageType, clearError]);

  const togglePageType = () => {
    setPageType(prevType => prevType === 'login' ? 'register' : 'login');
  };

  return (
    <Container fluid className="vh-100 bg-light">
      <Row className="h-100 align-items-center justify-content-center">
        <Col xs={12} sm={10} md={8} lg={6} xl={5}>
          <Card className="shadow-lg border-0">
            <Card.Body className="p-5">
              <div className="text-center mb-4">
                <i className="bi bi-wallet2 text-primary display-1"></i>
                <h1 className="h3 mb-3 fw-normal">FamilyBudgeter</h1>
                <p className="text-muted">
                  Планування та контроль сімейних фінансів
                </p>
              </div>

              {error && (
                <Alert 
                  variant="danger" 
                  dismissible 
                  onClose={clearError}
                  className="mb-4"
                >
                  {error}
                </Alert>
              )}

              {pageType === 'login' ? (
                <>
                  <LoginForm />
                  <div className="text-center mt-4">
                    <p className="text-muted mb-0">
                      Немає облікового запису?{' '}
                      <Button
                        variant="link"
                        className="p-0 align-baseline"
                        onClick={togglePageType}
                      >
                        Зареєструватися
                      </Button>
                    </p>
                  </div>
                </>
              ) : (
                <>
                  <RegisterForm />
                  <div className="text-center mt-4">
                    <p className="text-muted mb-0">
                      Вже маєте обліковий запис?{' '}
                      <Button
                        variant="link"
                        className="p-0 align-baseline"
                        onClick={togglePageType}
                      >
                        Увійти
                      </Button>
                    </p>
                  </div>
                </>
              )}
            </Card.Body>
          </Card>

          <div className="text-center mt-4">
            <small className="text-muted">
              © {new Date().getFullYear()} FamilyBudgeter. Всі права захищені.
            </small>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default AuthPage;