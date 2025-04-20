using FamilyBudgeter.API.BLL.DTOs.BudgetDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class BudgetController : BaseApiController
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("family/{familyId}")]
        public async Task<IActionResult> GetFamilyBudgets(int familyId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var budgets = await _budgetService.GetFamilyBudgetsAsync(familyId, userId);
                return Ok(budgets);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudget(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var budget = await _budgetService.GetBudgetByIdAsync(id, userId);
                return Ok(budget);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto budgetDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var budget = await _budgetService.CreateBudgetAsync(budgetDto, userId);
                return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetDto budgetDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var budget = await _budgetService.UpdateBudgetAsync(id, budgetDto, userId);
                return Ok(budget);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _budgetService.DeleteBudgetAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("family/{familyId}/type/{type}")]
        public async Task<IActionResult> GetBudgetsByType(int familyId, BudgetType type)
        {
            try
            {
                var userId = GetCurrentUserId();
                var budgets = await _budgetService.GetBudgetsByTypeAsync(familyId, type, userId);
                return Ok(budgets);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetBudgetSummary(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _budgetService.GetBudgetSummaryAsync(id, startDate, endDate, userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
