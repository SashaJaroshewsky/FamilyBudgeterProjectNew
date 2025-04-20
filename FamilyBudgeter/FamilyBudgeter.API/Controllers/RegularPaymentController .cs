using FamilyBudgeter.API.BLL.DTOs.RegularPaymentDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class RegularPaymentController : BaseApiController
    {
        private readonly IRegularPaymentService _regularPaymentService;

        public RegularPaymentController(IRegularPaymentService regularPaymentService)
        {
            _regularPaymentService = regularPaymentService;
        }

        [HttpGet("budget/{budgetId}")]
        public async Task<IActionResult> GetBudgetRegularPayments(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payments = await _regularPaymentService.GetBudgetRegularPaymentsAsync(budgetId, userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegularPayment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payment = await _regularPaymentService.GetRegularPaymentByIdAsync(id, userId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegularPayment([FromBody] CreateRegularPaymentDto paymentDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payment = await _regularPaymentService.CreateRegularPaymentAsync(paymentDto, userId);
                return CreatedAtAction(nameof(GetRegularPayment), new { id = payment.Id }, payment);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegularPayment(int id, [FromBody] UpdateRegularPaymentDto paymentDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payment = await _regularPaymentService.UpdateRegularPaymentAsync(id, paymentDto, userId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegularPayment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _regularPaymentService.DeleteRegularPaymentAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetRegularPaymentsByCategory(int categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payments = await _regularPaymentService.GetRegularPaymentsByCategoryAsync(categoryId, userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/frequency/{frequency}")]
        public async Task<IActionResult> GetRegularPaymentsByFrequency(int budgetId, PaymentFrequency frequency)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payments = await _regularPaymentService.GetRegularPaymentsByFrequencyAsync(budgetId, frequency, userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/upcoming")]
        public async Task<IActionResult> GetUpcomingRegularPayments(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payments = await _regularPaymentService.GetUpcomingRegularPaymentsAsync(budgetId, startDate, endDate, userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("process-due-payments")]
        [Authorize(Roles = "Administrator")] // Тільки адміністратори системи можуть запускати обробку платежів
        public async Task<IActionResult> ProcessDueRegularPayments()
        {
            try
            {
                var processedCount = await _regularPaymentService.ProcessDueRegularPaymentsAsync();
                return Ok(new { processedCount });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
