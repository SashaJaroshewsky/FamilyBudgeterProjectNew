using FamilyBudgeter.API.BLL.DTOs.TransactionDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("budget/{budgetId}")]
        public async Task<IActionResult> GetBudgetTransactions(int budgetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transactions = await _transactionService.GetBudgetTransactionsAsync(budgetId, userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transaction = await _transactionService.CreateTransactionAsync(transactionDto, userId);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionDto transactionDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transaction = await _transactionService.UpdateTransactionAsync(id, transactionDto, userId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _transactionService.DeleteTransactionAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetTransactionsByCategory(int categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transactions = await _transactionService.GetTransactionsByCategoryAsync(categoryId, userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transactions = await _transactionService.GetTransactionsByDateRangeAsync(budgetId, startDate, endDate, userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("my-transactions")]
        public async Task<IActionResult> GetUserTransactions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var transactions = await _transactionService.GetUserTransactionsAsync(userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterTransactions([FromBody] TransactionFilterDto filterDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var transactions = await _transactionService.FilterTransactionsAsync(filterDto, userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/receipt")]
        public async Task<IActionResult> UploadReceiptImage(int id, IFormFile file)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Файл не надано або він порожній" });
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _transactionService.UploadReceiptImageAsync(id, stream, file.FileName, userId);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
