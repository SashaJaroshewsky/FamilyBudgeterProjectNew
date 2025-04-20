using FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class FinancialGoalController : BaseApiController
    {
        private readonly IFinancialGoalService _financialGoalService;

        public FinancialGoalController(IFinancialGoalService financialGoalService)
        {
            _financialGoalService = financialGoalService;
        }

        [HttpGet("budget/{budgetId}")]
        public async Task<IActionResult> GetBudgetGoals(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _financialGoalService.GetBudgetGoalsAsync(budgetId, userId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoal(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _financialGoalService.GetGoalByIdAsync(id, userId);
                return Ok(goal);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] CreateFinancialGoalDto goalDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _financialGoalService.CreateGoalAsync(goalDto, userId);
                return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] UpdateFinancialGoalDto goalDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _financialGoalService.UpdateGoalAsync(id, goalDto, userId);
                return Ok(goal);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _financialGoalService.DeleteGoalAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPatch("{id}/amount")]
        public async Task<IActionResult> UpdateGoalAmount(int id, [FromBody] UpdateFinancialGoalAmountDto amountDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _financialGoalService.UpdateGoalAmountAsync(id, amountDto, userId);
                return Ok(goal);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/status/{status}")]
        public async Task<IActionResult> GetGoalsByStatus(int budgetId, FinancialGoalStatus status)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _financialGoalService.GetGoalsByStatusAsync(budgetId, status, userId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/check-status")]
        public async Task<IActionResult> CheckAndUpdateGoalStatus(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var statusChanged = await _financialGoalService.CheckAndUpdateGoalStatusAsync(id, userId);
                return Ok(new { statusChanged });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
