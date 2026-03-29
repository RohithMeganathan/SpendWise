using Microsoft.AspNetCore.Mvc;
using IncomeExpenseManagementApp.DTOs;
using IncomeExpenseManagementApp.Services;

namespace IncomeExpenseManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all transactions with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactions(
            [FromQuery] byte? categoryId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync(categoryId, startDate, endDate);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions");
                return StatusCode(500, new { message = "Error retrieving transactions", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDTO>> GetTransaction(long id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);

                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction");
                return StatusCode(500, new { message = "Error retrieving transaction", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction([FromBody] CreateTransactionDTO createTransactionDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var transaction = await _transactionService.CreateTransactionAsync(createTransactionDTO);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating transaction");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return StatusCode(500, new { message = "Error creating transaction", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing transaction
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction(long id, [FromBody] UpdateTransactionDTO updateTransactionDTO)
        {
            try
            {
                var transaction = await _transactionService.UpdateTransactionAsync(id, updateTransactionDTO);

                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating transaction");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction");
                return StatusCode(500, new { message = "Error updating transaction", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a transaction
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(long id)
        {
            try
            {
                var result = await _transactionService.DeleteTransactionAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction");
                return StatusCode(500, new { message = "Error deleting transaction", error = ex.Message });
            }
        }
    }
}
