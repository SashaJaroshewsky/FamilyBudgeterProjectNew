// src/components/notification/NotificationList.tsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Notification, NotificationType } from '../../models/NotificationModels';
import { notificationApi } from '../../api/notificationApi';

interface NotificationListProps {
  notifications: Notification[];
  limit?: number;
  onMarkAsRead?: (id: number) => void;
}

const NotificationList: React.FC<NotificationListProps> = ({ 
  notifications, 
  limit,
  onMarkAsRead 
}) => {
  const navigate = useNavigate();
  
  // Якщо заданий ліміт, обмежуємо кількість сповіщень
  const displayedNotifications = limit ? notifications.slice(0, limit) : notifications;
  
  // Функція для отримання іконки в залежності від типу сповіщення
  const getNotificationIcon = (type: NotificationType): string => {
    switch (type) {
      case NotificationType.LimitWarning:
        return 'icon-warning';
      case NotificationType.RegularPayment:
        return 'icon-calendar';
      case NotificationType.LargeExpense:
        return 'icon-alert';
      case NotificationType.GoalAchievement:
        return 'icon-trophy';
      case NotificationType.FamilyInvitation:
        return 'icon-users';
      default:
        return 'icon-bell';
    }
  };
  
  // Функція для форматування дати сповіщення
  const formatDate = (date: Date): string => {
    const now = new Date();
    const notificationDate = new Date(date);
    
    // Якщо сповіщення створено сьогодні, показуємо час
    if (
      now.getDate() === notificationDate.getDate() &&
      now.getMonth() === notificationDate.getMonth() &&
      now.getFullYear() === notificationDate.getFullYear()
    ) {
      return `Сьогодні, ${notificationDate.getHours().toString().padStart(2, '0')}:${notificationDate.getMinutes().toString().padStart(2, '0')}`;
    }
    
    // Якщо сповіщення створено вчора, показуємо "Вчора"
    const yesterday = new Date(now);
    yesterday.setDate(now.getDate() - 1);
    if (
      yesterday.getDate() === notificationDate.getDate() &&
      yesterday.getMonth() === notificationDate.getMonth() &&
      yesterday.getFullYear() === notificationDate.getFullYear()
    ) {
      return `Вчора, ${notificationDate.getHours().toString().padStart(2, '0')}:${notificationDate.getMinutes().toString().padStart(2, '0')}`;
    }
    
    // В інших випадках показуємо повну дату
    return `${notificationDate.getDate().toString().padStart(2, '0')}.${(notificationDate.getMonth() + 1).toString().padStart(2, '0')}.${notificationDate.getFullYear()}, ${notificationDate.getHours().toString().padStart(2, '0')}:${notificationDate.getMinutes().toString().padStart(2, '0')}`;
  };
  
  // Обробник кліку на сповіщення
  const handleNotificationClick = async (notification: Notification) => {
    // Якщо сповіщення не прочитане, позначаємо його як прочитане
    if (!notification.isRead) {
      try {
        await notificationApi.markAsRead(notification.id);
        if (onMarkAsRead) {
          onMarkAsRead(notification.id);
        }
      } catch (error) {
        console.error('Помилка при позначенні сповіщення як прочитаного:', error);
      }
    }
    
    // Перенаправлення в залежності від типу сповіщення
    switch (notification.type) {
      case NotificationType.LimitWarning:
        // Перенаправлення на сторінку бюджету
        navigate(`/budgets/${notification.familyId}`);
        break;
      case NotificationType.RegularPayment:
        // Перенаправлення на сторінку транзакцій
        navigate('/transactions');
        break;
      case NotificationType.LargeExpense:
        // Перенаправлення на сторінку транзакцій
        navigate('/transactions');
        break;
      case NotificationType.GoalAchievement:
        // Перенаправлення на сторінку фінансових цілей
        navigate('/goals');
        break;
      case NotificationType.FamilyInvitation:
        // Перенаправлення на сторінку сім'ї
        navigate(`/families/${notification.familyId}`);
        break;
      default:
        // За замовчуванням перенаправляємо на сторінку сповіщень
        navigate('/notifications');
        break;
    }
  };

  if (displayedNotifications.length === 0) {
    return (
      <div className="notification-list-empty">
        Немає сповіщень
      </div>
    );
  }

  return (
    <div className="notification-list">
      {displayedNotifications.map(notification => (
        <div 
          key={notification.id} 
          className={`notification-item ${notification.isRead ? 'read' : 'unread'}`}
          onClick={() => handleNotificationClick(notification)}
        >
          <div className="notification-icon">
            <i className={getNotificationIcon(notification.type)}></i>
          </div>
          
          <div className="notification-content">
            <div className="notification-header">
              <h4 className="notification-title">{notification.title}</h4>
              <span className="notification-date">{formatDate(notification.createdAt)}</span>
            </div>
            
            <div className="notification-message">
              {notification.message}
            </div>
            
            {notification.familyName && (
              <div className="notification-family">
                <span className="family-label">Сім'я:</span>
                <span className="family-name">{notification.familyName}</span>
              </div>
            )}
          </div>
        </div>
      ))}
      
      {limit && notifications.length > limit && (
        <div className="view-all-link">
          <button 
            className="btn btn-link" 
            onClick={() => navigate('/notifications')}
          >
            Переглянути всі сповіщення ({notifications.length})
          </button>
        </div>
      )}
    </div>
  );
};

export default NotificationList;