using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("budget/{budgetId}")]
        public async Task<IActionResult> GetBudgetCategories(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var categories = await _categoryService.GetBudgetCategoriesAsync(budgetId, userId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var category = await _categoryService.GetCategoryByIdAsync(id, userId);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var category = await _categoryService.CreateCategoryAsync(categoryDto, userId);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var category = await _categoryService.UpdateCategoryAsync(id, categoryDto, userId);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _categoryService.DeleteCategoryAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/type/{type}")]
        public async Task<IActionResult> GetCategoriesByType(int budgetId, CategoryType type)
        {
            try
            {
                var userId = GetCurrentUserId();
                var categories = await _categoryService.GetCategoriesByTypeAsync(budgetId, type, userId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/summary")]
        public async Task<IActionResult> GetCategorySummaries(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var summaries = await _categoryService.GetCategorySummariesAsync(budgetId, startDate, endDate, userId);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
