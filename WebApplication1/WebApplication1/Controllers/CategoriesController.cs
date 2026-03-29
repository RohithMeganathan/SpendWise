using Microsoft.AspNetCore.Mvc;
using IncomeExpenseManagementApp.DTOs;
using IncomeExpenseManagementApp.Services;

namespace IncomeExpenseManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new { message = "Error retrieving categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get categories by transaction type
        /// </summary>
        [HttpGet("by-type/{transactionTypeId}")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesByType(byte transactionTypeId)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesByTypeAsync(transactionTypeId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories by type");
                return StatusCode(500, new { message = "Error retrieving categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var categoryDTO = await _categoryService.CreateCategoryAsync(createCategoryDTO);
                return CreatedAtAction(nameof(GetCategories), new { id = categoryDTO.Id }, categoryDTO);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "Error creating category", error = ex.Message });
            }
        }
    }
}
