import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Container, Card, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { budgetApi } from '../api/budgetApi';
import { familyApi } from '../api/familyApi';
import { CreateBudget, BudgetType } from '../models/BudgetModels';
import { Family } from '../models/FamilyModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';

const CreateBudgetPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedFamilyId = searchParams.get('familyId');

  const [formData, setFormData] = useState<CreateBudget>({
    name: '',
    currency: 'UAH',
    type: BudgetType.Monthly,
    familyId: preselectedFamilyId ? parseInt(preselectedFamilyId, 10) : 0
  });

  const [families, setFamilies] = useState<Family[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [validated, setValidated] = useState(false);

  useEffect(() => {
    const fetchFamilies = async () => {
      try {
        const userFamilies = await familyApi.getUserFamilies();
        setFamilies(userFamilies);
        
        if (!preselectedFamilyId && userFamilies.length > 0) {
          setFormData(prev => ({ ...prev, familyId: userFamilies[0].id }));
        }
      } catch (err) {
        setError('Помилка завантаження списку сімей');
      } finally {
        setLoading(false);
      }
    };

    fetchFamilies();
  }, [preselectedFamilyId]);

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
      
      const newBudget = await budgetApi.createBudget(formData);
      navigate(`/budgets/${newBudget.id}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Помилка створення бюджету');
    } finally {
      setSubmitting(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'familyId' ? parseInt(value, 10) : value
    }));
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
                <h1 className="h3 mb-0">Створення нового бюджету</h1>
              </Card.Header>

              <Card.Body>
                {error && (
                  <Alert variant="danger" className="mb-4" dismissible onClose={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form noValidate validated={validated} onSubmit={handleSubmit}>
                  <Form.Group className="mb-3">
                    <Form.Label>Назва бюджету</Form.Label>
                    <Form.Control
                      required
                      type="text"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      placeholder="Введіть назву бюджету"
                      minLength={3}
                      maxLength={50}
                    />
                    <Form.Control.Feedback type="invalid">
                      Назва бюджету обов'язкова (від 3 до 50 символів)
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Тип бюджету</Form.Label>
                    <Form.Select
                      required
                      name="type"
                      value={formData.type}
                      onChange={handleInputChange}
                    >
                      <option value={BudgetType.Monthly}>Місячний</option>
                      <option value={BudgetType.Yearly}>Річний</option>
                      <option value={BudgetType.Special}>Спеціальний</option>
                    </Form.Select>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Валюта</Form.Label>
                    <Form.Select
                      required
                      name="currency"
                      value={formData.currency}
                      onChange={handleInputChange}
                    >
                      <option value="UAH">Гривня (UAH)</option>
                      <option value="USD">Долар США (USD)</option>
                      <option value="EUR">Євро (EUR)</option>
                    </Form.Select>
                  </Form.Group>

                  <Form.Group className="mb-4">
                    <Form.Label>Сім'я</Form.Label>
                    <Form.Select
                      required
                      name="familyId"
                      value={formData.familyId}
                      onChange={handleInputChange}
                      disabled={preselectedFamilyId !== null}
                    >
                      <option value="">Виберіть сім'ю</option>
                      {families.map(family => (
                        <option key={family.id} value={family.id}>
                          {family.name}
                        </option>
                      ))}
                    </Form.Select>
                    <Form.Control.Feedback type="invalid">
                      Виберіть сім'ю для бюджету
                    </Form.Control.Feedback>
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
                        'Створити бюджет'
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

export default CreateBudgetPage;