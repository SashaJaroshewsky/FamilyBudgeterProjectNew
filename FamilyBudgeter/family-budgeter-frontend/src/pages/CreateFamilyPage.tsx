import React, { useState } from 'react';
import { Container, Row, Col, Card, Form, Button, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { familyApi } from '../api/familyApi';
import { CreateFamily } from '../models/FamilyModels';
import AppHeader from '../components/common/AppHeader';

const CreateFamilyPage: React.FC = () => {
  const navigate = useNavigate();
  const [validated, setValidated] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const [formData, setFormData] = useState<CreateFamily>({
    name: ''
  });

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;

    if (!form.checkValidity()) {
      event.stopPropagation();
      setValidated(true);
      return;
    }

    try {
      setSubmitting(true);
      setError(null);
      
      const newFamily = await familyApi.createFamily({
        name: formData.name.trim()
      });

      // Перенаправляємо на сторінку створеної сім'ї
      navigate(`/families/${newFamily.id}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Помилка створення сім\'ї');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <>
      <AppHeader />
      <Container className="py-4">
        <Row className="justify-content-center">
          <Col md={8} lg={6}>
            <Card className="shadow-sm">
              <Card.Header className="bg-white py-3">
                <div className="text-center">
                  <i className="bi bi-people-fill fs-1 text-primary"></i>
                  <h1 className="h3 mt-3 mb-0">Створення нової сім'ї</h1>
                </div>
              </Card.Header>

              <Card.Body className="p-4">
                {error && (
                  <Alert variant="danger" dismissible onClose={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form noValidate validated={validated} onSubmit={handleSubmit}>
                  <Form.Group className="mb-4">
                    <Form.Label>Назва сім'ї</Form.Label>
                    <Form.Control
                      required
                      type="text"
                      placeholder="Введіть назву сім'ї"
                      value={formData.name}
                      onChange={(e) => setFormData({ name: e.target.value })}
                      disabled={submitting}
                      minLength={3}
                      maxLength={50}
                    />
                    <Form.Control.Feedback type="invalid">
                      Назва сім'ї повинна містити від 3 до 50 символів
                    </Form.Control.Feedback>
                    <Form.Text className="text-muted">
                      Назва буде відображатися для всіх учасників сім'ї
                    </Form.Text>
                  </Form.Group>

                  <div className="d-grid gap-2">
                    <Button
                      type="submit"
                      variant="primary"
                      size="lg"
                      disabled={submitting}
                    >
                      {submitting ? (
                        <>
                          <span className="spinner-border spinner-border-sm me-2" />
                          Створення...
                        </>
                      ) : (
                        <>
                          <i className="bi bi-plus-circle me-2"></i>
                          Створити сім'ю
                        </>
                      )}
                    </Button>
                    <Button
                      variant="outline-secondary"
                      onClick={() => navigate('/families')}
                      disabled={submitting}
                    >
                      Скасувати
                    </Button>
                  </div>
                </Form>
              </Card.Body>
            </Card>

            <div className="text-center mt-4">
              <p className="text-muted mb-0">
                Вже є код приєднання?{' '}
                <Button
                  variant="link"
                  className="p-0"
                  onClick={() => navigate('/families')}
                >
                  Приєднайтеся до існуючої сім'ї
                </Button>
              </p>
            </div>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default CreateFamilyPage;