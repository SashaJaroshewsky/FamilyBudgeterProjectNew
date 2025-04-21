// src/components/auth/RegisterForm.tsx
import React, { useState } from 'react';
import { Form, Button, Row, Col } from 'react-bootstrap';
import { useAuth } from '../../context/AuthContext';
import { UserRegistration } from '../../models/AuthModels';

const RegisterForm: React.FC = () => {
  const { register, loading } = useAuth();
  const [formData, setFormData] = useState<UserRegistration>({
    email: '',
    password: '',
    firstName: '',
    lastName: ''
  });
  const [confirmPassword, setConfirmPassword] = useState('');
  const [validated, setValidated] = useState(false);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;
    
    if (form.checkValidity() === false || formData.password !== confirmPassword) {
      event.stopPropagation();
      setValidated(true);
      return;
    }

    await register(formData);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    if (name === 'confirmPassword') {
      setConfirmPassword(value);
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
  };

  return (
    <Form noValidate validated={validated} onSubmit={handleSubmit}>
      <h2 className="text-center mb-4">Реєстрація</h2>
      
      <Row className="g-3 mb-3">
        <Col md={6}>
          <Form.Group controlId="firstName">
            <Form.Label>Ім'я</Form.Label>
            <Form.Control
              required
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              disabled={loading}
              placeholder="Введіть ваше ім'я"
              minLength={2}
            />
            <Form.Control.Feedback type="invalid">
              Введіть коректне ім'я (мінімум 2 символи)
            </Form.Control.Feedback>
          </Form.Group>
        </Col>
        
        <Col md={6}>
          <Form.Group controlId="lastName">
            <Form.Label>Прізвище</Form.Label>
            <Form.Control
              required
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              disabled={loading}
              placeholder="Введіть ваше прізвище"
              minLength={2}
            />
            <Form.Control.Feedback type="invalid">
              Введіть коректне прізвище (мінімум 2 символи)
            </Form.Control.Feedback>
          </Form.Group>
        </Col>
      </Row>

      <Form.Group className="mb-3" controlId="email">
        <Form.Label>Електронна пошта</Form.Label>
        <Form.Control
          required
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          disabled={loading}
          placeholder="example@domain.com"
        />
        <Form.Control.Feedback type="invalid">
          Введіть коректну електронну адресу
        </Form.Control.Feedback>
      </Form.Group>

      <Form.Group className="mb-3" controlId="password">
        <Form.Label>Пароль</Form.Label>
        <Form.Control
          required
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          disabled={loading}
          placeholder="Введіть пароль"
          minLength={6}
        />
        <Form.Text className="text-muted">
          Пароль має містити щонайменше 6 символів
        </Form.Text>
        <Form.Control.Feedback type="invalid">
          Пароль має містити щонайменше 6 символів
        </Form.Control.Feedback>
      </Form.Group>

      <Form.Group className="mb-4" controlId="confirmPassword">
        <Form.Label>Підтвердження пароля</Form.Label>
        <Form.Control
          required
          type="password"
          name="confirmPassword"
          value={confirmPassword}
          onChange={handleChange}
          disabled={loading}
          placeholder="Повторіть пароль"
          isInvalid={validated && formData.password !== confirmPassword}
        />
        <Form.Control.Feedback type="invalid">
          Паролі не співпадають
        </Form.Control.Feedback>
      </Form.Group>

      <div className="d-grid">
        <Button
          variant="primary"
          type="submit"
          size="lg"
          disabled={loading}
        >
          {loading ? (
            <>
              <span className="spinner-border spinner-border-sm me-2" />
              Реєстрація...
            </>
          ) : (
            'Зареєструватися'
          )}
        </Button>
      </div>
    </Form>
  );
};

export default RegisterForm;