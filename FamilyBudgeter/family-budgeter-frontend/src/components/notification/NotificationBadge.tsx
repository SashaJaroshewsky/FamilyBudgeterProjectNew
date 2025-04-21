// src/components/notification/NotificationBadge.tsx
import React from 'react';

interface NotificationBadgeProps {
  count: number;
}

const NotificationBadge: React.FC<NotificationBadgeProps> = ({ count }) => {
  // Якщо кількість більше 99, показуємо "99+"
  const displayCount = count > 99 ? '99+' : count.toString();
  
  return (
    <span className="notification-badge">
      {displayCount}
    </span>
  );
};

export default NotificationBadge;