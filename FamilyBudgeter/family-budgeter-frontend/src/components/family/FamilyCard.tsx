// src/components/family/FamilyCard.tsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Badge, Button, Row, Col } from 'react-bootstrap';
import { Family, FamilyRole } from '../../models/FamilyModels';

interface FamilyCardProps {
  family: Family;
}

const FamilyCard: React.FC<FamilyCardProps> = ({ family }) => {
  const navigate = useNavigate();
  
  const isAdmin = family.members.some(member => 
    member.role === FamilyRole.Administrator
  );
  
  const adminsCount = family.members.filter(m => m.role === FamilyRole.Administrator).length;
  const fullMembersCount = family.members.filter(m => m.role === FamilyRole.FullMember).length;
  const limitedMembersCount = family.members.filter(m => m.role === FamilyRole.LimitedMember).length;

  const handleClick = () => {
    navigate(`/families/${family.id}`);
  };

  return (
    <Card className="h-100 shadow-sm hover-shadow" style={{ cursor: 'pointer' }} onClick={handleClick}>
      <Card.Header className="bg-white">
        <div className="d-flex justify-content-between align-items-center">
          <h5 className="mb-0">{family.name}</h5>
          {isAdmin && (
            <Badge bg="primary">
              <i className="bi bi-shield-check me-1"></i>
              Адміністратор
            </Badge>
          )}
        </div>
      </Card.Header>
      
      <Card.Body>
        <div className="mb-3">
          <h6 className="text-muted mb-3">Учасники: {family.members.length}</h6>
          <Row className="g-2">
            <Col xs={12}>
              <div className="d-flex justify-content-between align-items-center border-bottom pb-2">
                <small className="text-muted">Адміністратори</small>
                <Badge bg="secondary">{adminsCount}</Badge>
              </div>
            </Col>
            <Col xs={12}>
              <div className="d-flex justify-content-between align-items-center border-bottom pb-2">
                <small className="text-muted">Повні учасники</small>
                <Badge bg="secondary">{fullMembersCount}</Badge>
              </div>
            </Col>
            <Col xs={12}>
              <div className="d-flex justify-content-between align-items-center pb-2">
                <small className="text-muted">Обмежені учасники</small>
                <Badge bg="secondary">{limitedMembersCount}</Badge>
              </div>
            </Col>
          </Row>
        </div>
        
        {family.joinCode && (
          <div className="mt-3 p-2 bg-light rounded">
            <small className="text-muted d-block mb-1">Код для приєднання:</small>
            <code className="fs-6">{family.joinCode}</code>
          </div>
        )}
      </Card.Body>
      
      <Card.Footer className="bg-white border-top">
        <div className="d-flex gap-2 justify-content-end">
          <Button 
            variant="outline-primary" 
            size="sm"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/families/${family.id}`);
            }}
          >
            <i className="bi bi-info-circle me-1"></i>
            Деталі
          </Button>
          
          {isAdmin && (
            <Button 
              variant="outline-secondary"
              size="sm"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/families/${family.id}/edit`);
              }}
            >
              <i className="bi bi-pencil me-1"></i>
              Редагувати
            </Button>
          )}
        </div>
      </Card.Footer>
    </Card>
  );
};

export default FamilyCard;