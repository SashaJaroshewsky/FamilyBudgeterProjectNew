// src/pages/FamilyPage.tsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Alert, Badge } from 'react-bootstrap';
import { familyApi } from '../api/familyApi';
import { Family } from '../models/FamilyModels';
import AppHeader from '../components/common/AppHeader';
import FamilyCard from '../components/family/FamilyCard';
import Loader from '../components/common/Loader';
import JoinFamilyModal from '../components/family/JoinFamilyModal';

interface FamilyPageProps {
  isDetail?: boolean;
}

const FamilyPage: React.FC<FamilyPageProps> = ({ isDetail = false }) => {
  const { familyId } = useParams<{ familyId: string }>();
  const navigate = useNavigate();
  
  const [families, setFamilies] = useState<Family[]>([]);
  const [currentFamily, setCurrentFamily] = useState<Family | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [showJoinModal, setShowJoinModal] = useState(false);

  useEffect(() => {
    const fetchFamilies = async () => {
      try {
        setLoading(true);
        if (isDetail && familyId) {
          const familyDetails = await familyApi.getFamilyById(parseInt(familyId, 10));
          setCurrentFamily(familyDetails);
        } else {
          const userFamilies = await familyApi.getUserFamilies();
          setFamilies(userFamilies);
        }
      } catch (err) {
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('Виникла помилка при завантаженні даних про сім\'ї');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchFamilies();
  }, [isDetail, familyId]);

  const handleCreateFamily = () => {
    navigate('/families/create');
  };

  const handleJoinSuccess = async () => {
    // Оновлюємо список сімей після успішного приєднання
    try {
      const userFamilies = await familyApi.getUserFamilies();
      setFamilies(userFamilies);
    } catch (err) {
      setError('Помилка оновлення списку сімей');
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

  if (error) {
    return (
      <>
        <AppHeader />
        <Container className="py-4">
          <Alert variant="danger">{error}</Alert>
        </Container>
      </>
    );
  }

  // Detailed family view
  if (isDetail && currentFamily) {
    return (
      <>
        <AppHeader />
        <Container className="py-4">
          <Card className="shadow-sm mb-4">
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-4">
                <h1 className="h2 mb-0">{currentFamily.name}</h1>
                <div className="d-flex gap-2">
                  <Button 
                    variant="outline-primary"
                    onClick={() => navigate(`/families/${currentFamily.id}/edit`)}
                  >
                    <i className="bi bi-pencil me-2"></i>
                    Редагувати сім'ю
                  </Button>
                  <Button 
                    variant="primary"
                    onClick={() => navigate(`/budgets/create?familyId=${currentFamily.id}`)}
                  >
                    <i className="bi bi-plus-lg me-2"></i>
                    Створити бюджет
                  </Button>
                </div>
              </div>

              <Card className="mb-4">
                <Card.Header className="bg-white">
                  <h3 className="h5 mb-0">Інформація про сім'ю</h3>
                </Card.Header>
                <Card.Body>
                  <Row className="g-3">
                    <Col sm={6} md={4}>
                      <div className="small text-muted">Назва</div>
                      <div className="fw-medium">{currentFamily.name}</div>
                    </Col>
                    <Col sm={6} md={4}>
                      <div className="small text-muted">Код приєднання</div>
                      <div className="fw-medium">
                        {currentFamily.joinCode || 'Недоступний'}
                      </div>
                    </Col>
                    <Col sm={6} md={4}>
                      <div className="small text-muted">Кількість учасників</div>
                      <div className="fw-medium">{currentFamily.members.length}</div>
                    </Col>
                  </Row>
                </Card.Body>
              </Card>

              <Card>
                <Card.Header className="bg-white">
                  <h3 className="h5 mb-0">Учасники сім'ї</h3>
                </Card.Header>
                <Card.Body>
                  <Row xs={1} md={2} lg={3} className="g-4">
                    {currentFamily.members.map(member => (
                      <Col key={member.id}>
                        <Card className="h-100">
                          <Card.Body>
                            <div className="d-flex align-items-center">
                              <div className="me-3">
                                <div className="rounded-circle bg-primary bg-opacity-10 p-3">
                                  <i className="bi bi-person fs-4 text-primary"></i>
                                </div>
                              </div>
                              <div>
                                <h6 className="mb-1">{member.userName}</h6>
                                <div className="small text-muted">{member.userEmail}</div>
                                <Badge bg="secondary" className="mt-2">{member.role}</Badge>
                              </div>
                            </div>
                          </Card.Body>
                        </Card>
                      </Col>
                    ))}
                  </Row>
                </Card.Body>
              </Card>
            </Card.Body>
          </Card>
        </Container>
      </>
    );
  }

  // Families list view
  return (
    <>
      <AppHeader />
      <Container className="py-4">
        <Card className="shadow-sm">
          <Card.Body>
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h1 className="h2 mb-0">Мої сім'ї</h1>
              <div className="d-flex gap-2">
                <Button variant="outline-primary" onClick={() => setShowJoinModal(true)}>
                  <i className="bi bi-box-arrow-in-right me-2"></i>
                  Приєднатися до сім'ї
                </Button>
                <Button variant="primary" onClick={handleCreateFamily}>
                  <i className="bi bi-plus-lg me-2"></i>
                  Створити сім'ю
                </Button>
              </div>
            </div>

            {families.length > 0 ? (
              <Row xs={1} md={2} lg={3} className="g-4">
                {families.map(family => (
                  <Col key={family.id}>
                    <FamilyCard family={family} />
                  </Col>
                ))}
              </Row>
            ) : (
              <div className="text-center py-5">
                <i className="bi bi-people fs-1 text-muted"></i>
                <h3 className="h4 mt-4">У вас ще немає сімей</h3>
                <p className="text-muted mb-4">
                  Створіть свою першу сім'ю, щоб почати користуватися перевагами FamilyBudgeter
                </p>
                <Button variant="primary" onClick={handleCreateFamily}>
                  <i className="bi bi-plus-lg me-2"></i>
                  Створити сім'ю
                </Button>
              </div>
            )}
          </Card.Body>
        </Card>
      </Container>

      <JoinFamilyModal 
        show={showJoinModal}
        onHide={() => setShowJoinModal(false)}
        onSuccess={handleJoinSuccess}
      />
    </>
  );
};

export default FamilyPage;