import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Form, Button, Alert, ProgressBar } from 'react-bootstrap';
import { useNavigate, useParams } from 'react-router-dom';
import { financialGoalApi } from '../api/financialGoalApi';
import { FinancialGoal, UpdateFinancialGoalAmount } from '../models/FinancialGoalModels';
import AppHeader from '../components/common/AppHeader';
import Loader from '../components/common/Loader';

const UpdateGoalAmountPage: React.FC = () => {
  const navigate = useNavigate();
  const { goalId } = useParams<{ goalId: string }>();
  
  const [goal, setGoal] = useState<FinancialGoal | null>(null);
  const [amount, setAmount] = useState<number>(0);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [validated, setValidated] = useState(false);

  useEffect(() => {
    const loadGoal = async () => {
      try {
        if (!goalId) return;
        const goalData = await financialGoalApi.getGoalById(parseInt(goalId, 10));
        setGoal(goalData);
        setAmount(0); // Reset amount when goal changes
      } catch (err) {
        setError('Помилка завантаження даних про ціль');
      } finally {
        setLoading(false);
      }
    };

    loadGoal();
  }, [goalId]);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;

    if (!form.checkValidity() || !goalId || !goal) {
      event.stopPropagation();
      setValidated(true);
      return;
    }

    if (amount <= 0) {
      setError('Сума поповнення повинна бути більше 0');
      return;
    }

    if (goal.currentAmount + amount > goal.targetAmount) {
      setError('Сума поповнення призведе до перевищення цільової суми');
      return;
    }

    try {
      setSubmitting(true);
      setError(null);

      const updateData: UpdateFinancialGoalAmount = {
        currentAmount: goal.currentAmount + amount
      };

      await financialGoalApi.updateGoalAmount(parseInt(goalId, 10), updateData);
      navigate(-1);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Помилка оновлення суми');
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

  if (!goal) {
    return (
      <>
        <AppHeader />
        <Container className="py-4">
          <Alert variant="danger">
            Ціль не знайдена
          </Alert>
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
                <h1 className="h3 mb-0">Поповнення фінансової цілі</h1>
              </Card.Header>

              <Card.Body>
                {/* Goal Info */}
                <div className="mb-4">
                  <h5>{goal.name}</h5>
                  <ProgressBar
                    now={goal.percentComplete}
                    label={`${goal.percentComplete.toFixed(0)}%`}
                    variant={goal.percentComplete === 100 ? 'success' : 'primary'}
                    className="mb-2"
                  />
                  <div className="d-flex justify-content-between text-muted small">
                    <span>Поточна сума: {goal.currentAmount.toLocaleString()} грн</span>
                    <span>Ціль: {goal.targetAmount.toLocaleString()} грн</span>
                  </div>
                  <div className="text-muted small mt-1">
                    {goal.daysRemaining > 0 ? (
                      <span>Залишилось {goal.daysRemaining} днів</span>
                    ) : (
                      <span className="text-danger">Термін закінчився</span>
                    )}
                  </div>
                </div>

                {error && (
                  <Alert variant="danger" dismissible onClose={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form noValidate validated={validated} onSubmit={handleSubmit}>
                  <Form.Group className="mb-4">
                    <Form.Label>Сума поповнення</Form.Label>
                    <Form.Control
                      required
                      type="number"
                      value={amount}
                      onChange={(e) => setAmount(parseFloat(e.target.value) || 0)}
                      disabled={submitting}
                      min={0.01}
                      step={0.01}
                      max={goal.targetAmount - goal.currentAmount}
                    />
                    <Form.Text className="text-muted">
                      Максимальна сума поповнення: {(goal.targetAmount - goal.currentAmount).toLocaleString()} грн
                    </Form.Text>
                  </Form.Group>

                  <div className="d-flex gap-2 justify-content-end">
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
                          Збереження...
                        </>
                      ) : (
                        'Поповнити'
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

export default UpdateGoalAmountPage;