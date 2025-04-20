using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class BudgetLimitController : BaseApiController
    {
        private readonly IBudgetLimitService _budgetLimitService;

        public BudgetLimitController(IBudgetLimitService budgetLimitService)
        {
            _budgetLimitService = budgetLimitService;
        }

        [HttpGet("budget/{budgetId}")]
        public async Task<IActionResult> GetBudgetLimits(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var limits = await _budgetLimitService.GetBudgetLimitsAsync(budgetId, userId);
                return Ok(limits);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudgetLimit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var limit = await _budgetLimitService.GetBudgetLimitByIdAsync(id, userId);
                return Ok(limit);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudgetLimit([FromBody] CreateBudgetLimitDto limitDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var limit = await _budgetLimitService.CreateBudgetLimitAsync(limitDto, userId);
                return CreatedAtAction(nameof(GetBudgetLimit), new { id = limit.Id }, limit);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudgetLimit(int id, [FromBody] UpdateBudgetLimitDto limitDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var limit = await _budgetLimitService.UpdateBudgetLimitAsync(id, limitDto, userId);
                return Ok(limit);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudgetLimit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _budgetLimitService.DeleteBudgetLimitAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/active")]
        public async Task<IActionResult> GetActiveBudgetLimits(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var limits = await _budgetLimitService.GetActiveBudgetLimitsAsync(budgetId, userId);
                return Ok(limits);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetLimitsByCategory(int categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var limits = await _budgetLimitService.GetLimitsByCategoryAsync(categoryId, userId);
                return Ok(limits);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("category/{categoryId}/exceeded")]
        public async Task<IActionResult> IsLimitExceeded(int categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isExceeded = await _budgetLimitService.IsLimitExceededAsync(categoryId, userId);
                return Ok(new { isExceeded });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/status")]
        public async Task<IActionResult> GetLimitStatuses(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var statuses = await _budgetLimitService.GetLimitStatusesAsync(budgetId, userId);
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
