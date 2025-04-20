using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Отримання всіх категорій бюджету
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetBudgetCategoriesAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання категорії за ідентифікатором
        /// </summary>
        Task<CategoryDetailDto> GetCategoryByIdAsync(int categoryId, int userId);

        /// <summary>
        /// Створення нової категорії
        /// </summary>
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, int userId);

        /// <summary>
        /// Оновлення категорії
        /// </summary>
        Task<CategoryDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto categoryDto, int userId);

        /// <summary>
        /// Видалення категорії
        /// </summary>
        Task<bool> DeleteCategoryAsync(int categoryId, int userId);

        /// <summary>
        /// Отримання категорій за типом
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(int budgetId, CategoryType type, int userId);

        /// <summary>
        /// Отримання підсумків за категоріями
        /// </summary>
        Task<IEnumerable<CategorySummaryDto>> GetCategorySummariesAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);
    }
}
