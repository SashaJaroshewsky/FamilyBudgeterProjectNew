export enum NotificationType {
    LimitWarning = 'LimitWarning',
    RegularPayment = 'RegularPayment',
    LargeExpense = 'LargeExpense',
    GoalAchievement = 'GoalAchievement',
    FamilyInvitation = 'FamilyInvitation'
}

export interface Notification {
    id: number;
    title: string;
    message: string;
    isRead: boolean;
    type: NotificationType;
    createdAt: Date;
    userId: number;
    familyId?: number;
    familyName?: string;
}

export interface CreateNotification {
    title: string;
    message: string;
    type: NotificationType;
    userId: number;
    familyId?: number;
}

export interface NotificationFilter {
    type?: NotificationType;
    isRead?: boolean;
    familyId?: number;
    startDate?: Date;
    endDate?: Date;
}