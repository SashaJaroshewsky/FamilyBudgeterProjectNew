import React from 'react';
import { Card, Badge, Button, ProgressBar } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { Category, CategoryType } from '../../models/CategoryModels';


interface CategoryItemProps {
  category: Category;
  amount?: number;
  limit?: number;
  percentOfLimit?: number;
  transactionsCount?: number;
  onDelete?: (id: number) => void;
}

const CategoryItem: React.FC<CategoryItemProps> = ({ 
  category,
  amount = 0,
  limit,
  percentOfLimit = 0,
  transactionsCount = 0,
  onDelete 
}) => {
  const navigate = useNavigate();

  const getCategoryIcon = () => {
    if (category.icon) return category.icon;
    
    return category.type === CategoryType.Income ? 
      'bi-arrow-down-circle' : 
      'bi-arrow-up-circle';
  };

  const getProgressVariant = () => {
    if (!percentOfLimit) return 'primary';
    if (percentOfLimit >= 100) return 'danger';
    if (percentOfLimit >= 80) return 'warning';
    return 'success';
  };

  return (
    <Card className="category-item shadow-sm">
      <Card.Body>
        <div className="d-flex justify-content-between align-items-start mb-3">
          <div className="d-flex align-items-center">
            <div className={`category-icon ${category.type.toLowerCase()}`}>
              <i className={`bi ${getCategoryIcon()}`}></i>
            </div>
            <div className="ms-3">
              <h5 className="mb-1">{category.name}</h5>
              <Badge bg={category.type === CategoryType.Income ? 'success' : 'danger'}>
                {category.type === CategoryType.Income ? 'Дохід' : 'Витрата'}
              </Badge>
            </div>
          </div>
          <div className="text-end">
            <div className="h5 mb-0">
              {amount.toFixed(2)}
            </div>
            <small className="text-muted">
              {transactionsCount} транзакцій
            </small>
          </div>
        </div>

        {limit && (
          <div className="mb-3">
            <div className="d-flex justify-content-between align-items-center mb-1">
              <small className="text-muted">Використано ліміту</small>
              <small className="text-muted">{percentOfLimit}%</small>
            </div>
            <ProgressBar 
              now={percentOfLimit} 
              variant={getProgressVariant()}
            />
            <small className="text-muted d-block mt-1">
              {amount.toFixed(2)} / {limit.toFixed(2)}
            </small>
          </div>
        )}

        <div className="d-flex gap-2 justify-content-end mt-3 pt-3 border-top">
          <Button 
            variant="outline-primary" 
            size="sm"
            onClick={() => navigate(`/categories/${category.id}`)}
          >
            <i className="bi bi-info-circle me-1"></i>
            Деталі
          </Button>
          <Button 
            variant="outline-secondary" 
            size="sm"
            onClick={() => navigate(`/categories/${category.id}/edit`)}
          >
            <i className="bi bi-pencil me-1"></i>
            Редагувати
          </Button>
          {onDelete && (
            <Button 
              variant="outline-danger" 
              size="sm"
              onClick={() => onDelete(category.id)}
            >
              <i className="bi bi-trash me-1"></i>
              Видалити
            </Button>
          )}
        </div>
      </Card.Body>
    </Card>
  );
};

export default CategoryItem;