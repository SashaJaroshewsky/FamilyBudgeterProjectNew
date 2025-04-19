namespace FamilyBudgeter.API.Domain.Enums
{
    public enum NotificationType
    {
        LimitWarning = 1,       // Попередження про наближення до ліміту
        RegularPayment = 2,     // Нагадування про регулярний платіж
        LargeExpense = 3,       // Повідомлення про значну витрату
        GoalAchievement = 4,    // Досягнення фінансової цілі
        FamilyInvitation = 5    // Запрошення до сім'ї
    }
}
