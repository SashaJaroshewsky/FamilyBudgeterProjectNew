import React from 'react';
import { Card, Badge, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { Transaction, TransactionType } from '../../models/TransactionModels';
import { useCategories } from '../../hooks/useCategories';
import { CategoryType } from '../../models/CategoryModels';

interface TransactionItemProps {
  transaction: Transaction;
  onDelete?: (id: number) => void;
}

const TransactionItem: React.FC<TransactionItemProps> = ({ transaction, onDelete }) => {
  const navigate = useNavigate();
  const { categories } = useCategories(transaction.budgetId);
  const category = categories.get(transaction.categoryId);
  const transactionType = category?.type === CategoryType.Income 
    ? TransactionType.Income 
    : TransactionType.Expense;

  const formatDate = (date: Date): string => {
    return new Date(date).toLocaleDateString('uk-UA', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <Card className="transaction-item mb-3 shadow-sm">
      <Card.Body>
        <div className="d-flex justify-content-between align-items-start">
          <div className="d-flex align-items-center">
            <div className={`transaction-icon ${
              transactionType === TransactionType.Income ? 'income' : 'expense'
            }`}>
              <i className={`bi ${category?.icon || 
                (transactionType === TransactionType.Income ? 'bi-arrow-down' : 'bi-arrow-up')
              }`}></i>
            </div>
            <div className="ms-3">
              <h5 className="mb-1">{transaction.description}</h5>
              <div className="text-muted small">
                <span className="me-3">
                  <i className="bi bi-calendar3 me-1"></i>
                  {formatDate(transaction.date)}
                </span>
                <span className="me-3">
                  <i className="bi bi-folder me-1"></i>
                  {category?.name || transaction.categoryName}
                </span>
                <span>
                  <i className="bi bi-wallet2 me-1"></i>
                  {transaction.budgetName}
                </span>
              </div>
            </div>
          </div>
          
          <div className="text-end">
            <div className="mb-2">
              <span className={`h5 mb-0 ${
                transactionType === TransactionType.Income ? 'text-success' : 'text-danger'
              }`}>
                {transactionType === TransactionType.Income ? '+' : '-'}
                {transaction.amount.toFixed(2)} {transaction.currency}
              </span>
            </div>
            <Badge bg={transactionType === TransactionType.Income ? 'success' : 'danger'}>
              {transactionType === TransactionType.Income ? 'Дохід' : 'Витрата'}
            </Badge>
          </div>
        </div>

        {transaction.receiptImageUrl && (
          <div className="mt-3">
            <Button 
              variant="outline-secondary" 
              size="sm"
              onClick={() => window.open(transaction.receiptImageUrl, '_blank')}
            >
              <i className="bi bi-receipt me-2"></i>
              Переглянути чек
            </Button>
          </div>
        )}

        <div className="mt-3 pt-3 border-top d-flex justify-content-between align-items-center">
          <div className="text-muted small">
            Створено: {transaction.createdByUserName}
          </div>
          <div className="d-flex gap-2">
            <Button 
              variant="outline-primary" 
              size="sm"
              onClick={() => navigate(`/transactions/${transaction.id}`)}
            >
              <i className="bi bi-pencil me-1"></i>
              Редагувати
            </Button>
            {onDelete && (
              <Button 
                variant="outline-danger" 
                size="sm"
                onClick={() => onDelete(transaction.id)}
              >
                <i className="bi bi-trash me-1"></i>
                Видалити
              </Button>
            )}
          </div>
        </div>
      </Card.Body>
    </Card>
  );
};

export default TransactionItem;