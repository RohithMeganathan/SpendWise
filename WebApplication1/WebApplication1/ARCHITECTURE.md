# Income Expense Management API - Layered Architecture

## Project Architecture Overview

This project follows a **clean, layered architecture** with proper separation of concerns:

```
Controllers Layer     → HTTP Requests/Responses (Minimal Business Logic)
    ↓
Services Layer       → Business Logic, Validation, Data Transformation
    ↓
Repositories Layer   → Data Access, Database Operations
    ↓
Models Layer         → EF Core Models (Database Schema)
    ↓
Database             → SQL Server
```

---

## Folder Structure

```
WebApplication1/
├── Controllers/              # API Controllers (Thin layer - routing only)
│   ├── CategoriesController.cs
│   ├── TransactionsController.cs
│   └── DashboardController.cs
│
├── Services/                 # Business Logic Layer
│   ├── IServices.cs                 # Service interfaces
│   └── Services.cs                  # Service implementations
│       ├── CategoryService
│       ├── TransactionService
│       └── DashboardService
│
├── Repositories/             # Data Access Layer
│   ├── IRepository.cs               # Generic repository interface
│   ├── Repository.cs                # Generic repository implementation
│   ├── IEntityRepositories.cs       # Entity-specific repository interfaces
│   └── EntityRepositories.cs        # Entity-specific implementations
│       ├── TransactionTypeRepository
│       ├── CategoryRepository
│       └── TransactionRepository
│
├── Data/                     # Database Context
│   └── ApplicationDbContext.cs       # EF Core DbContext
│
├── Models/                   # Domain Models
│   ├── TransactionType.cs
│   ├── Category.cs
│   └── Transaction.cs
│
├── DTOs/                     # Data Transfer Objects
│   ├── CategoryDTOs.cs
│   ├── TransactionDTOs.cs
│   └── DashboardDTOs.cs
│
├── Database/                 # SQL Scripts
│   └── 01_InitialSchema.sql
│
├── Program.cs                # DI Configuration
├── appsettings.json         # Configuration
└── README.md
```

---

## Layer Responsibilities

### 1. **Controllers Layer**
**Location:** `Controllers/`

**Responsibilities:**
- Handle HTTP requests/responses
- Route requests to appropriate services
- Input validation delegation to DTOs
- Error handling and HTTP status codes
- Minimal business logic

**Example:**
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
{
    var categories = await _categoryService.GetAllCategoriesAsync();
    return Ok(categories);
}
```

**Benefits:**
- Controllers remain thin and focused on HTTP concerns
- Easy to test by mocking services
- Reusable services across multiple controllers

---

### 2. **Services Layer**
**Location:** `Services/`

**Responsibilities:**
- Implement business logic
- Data validation and transformation
- Orchestrate repository calls
- Logging and error handling
- DTO mapping
- Transaction coordination

**Example:**
```csharp
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    
    public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto)
    {
        // Validate transaction type
        var typeExists = await _transactionTypeRepository
            .AnyAsync(tt => tt.Id == dto.TransactionTypeId);
        
        if (!typeExists)
            throw new ArgumentException("Invalid transaction type");
        
        // Create and save
        var category = new Category { ... };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        
        return MapToDTO(category);
    }
}
```

**Key Services:**
- `ICategoryService` - Category management
- `ITransactionService` - Transaction management
- `IDashboardService` - Dashboard analytics

**Benefits:**
- Centralized business rules
- Easy to test in isolation
- Reusable across controllers
- Clear separation from data access

---

### 3. **Repositories Layer**
**Location:** `Repositories/`

**Responsibilities:**
- Abstract data access logic
- CRUD operations
- Database queries
- Entity framework interactions
- Data persistence

**Two-Part Design:**

#### **Generic Repository** (`IRepository<T>`, `Repository<T>`)
Provides common CRUD operations:
```csharp
public interface IRepository<T> where T : class
{
    // Read
    Task<T?> GetByIdAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    // Write
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(object id);
    
    // Commit
    Task<int> SaveChangesAsync();
}
```

#### **Entity-Specific Repositories**
Extend the generic repository with custom queries:
```csharp
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetByTransactionTypeAsync(byte typeId);
    Task<Category?> GetWithTransactionTypeAsync(byte id);
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetByTransactionTypeAsync(byte typeId)
    {
        return await _dbSet
            .Where(c => c.TransactionTypeId == typeId)
            .Include(c => c.TransactionType)
            .ToListAsync();
    }
}
```

**Benefits:**
- Consistent data access pattern
- Single responsibility per repository
- Easy to mock for testing
- Centralized database logic

---

### 4. **Data Transfer Objects (DTOs)**
**Location:** `DTOs/`

**Responsibilities:**
- Define API contracts
- Request/response shapes
- Input validation rules
- Isolate models from API consumers

**Example:**
```csharp
public class CreateCategoryDTO
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    [Required]
    public byte TransactionTypeId { get; set; }
}
```

**Benefits:**
- Protects database models from client changes
- Enables versioning of API contracts
- ASP.NET Core validates automatically
- Clear API documentation

---

### 5. **Models Layer**
**Location:** `Models/`

**Responsibilities:**
- Define database schema
- Entity relationships
- EF Core configuration

**Example:**
```csharp
public class Category
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte TransactionTypeId { get; set; }
    
    // Navigation properties
    public TransactionType? TransactionType { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}
```

---

## Dependency Injection Setup

**Program.cs Configuration:**
```csharp
// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repositories
builder.Services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Register Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

**Scoped Lifetime:** New instance per HTTP request (suitable for DbContext)

---

## Data Flow Example: Creating a Category

```
1. Client sends POST /api/categories with CreateCategoryDTO
                                    ↓
2. CategoriesController.CreateCategory()
   - Validates ModelState
   - Calls _categoryService.CreateCategoryAsync(dto)
                                    ↓
3. CategoryService.CreateCategoryAsync()
   - Validates transaction type exists via _transactionTypeRepository
   - Creates Category entity
   - Calls _categoryRepository.AddAsync(category)
   - Calls _categoryRepository.SaveChangesAsync()
   - Maps to CategoryDTO
   - Returns DTO
                                    ↓
4. CategoryRepository (specific implementation)
   - Inherits from Repository<Category>
   - Uses DbSet<Category> for database operations
                                    ↓
5. Repository<T> (generic implementation)
   - Executes DbSet.AddAsync(entity)
   - Executes _context.SaveChangesAsync()
                                    ↓
6. ApplicationDbContext
   - Communicates with SQL Server
   - Persists data
                                    ↓
7. Response returned to client
```

---

## Testing Benefits

### Unit Testing Services
```csharp
[Test]
public async Task CreateCategory_WithValidData_ReturnsDTO()
{
    // Arrange
    var mockCategoryRepo = new Mock<ICategoryRepository>();
    var mockTypeRepo = new Mock<ITransactionTypeRepository>();
    var service = new CategoryService(mockCategoryRepo.Object, mockTypeRepo.Object, logger);
    
    mockTypeRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<...>>()))
        .ReturnsAsync(true);
    
    // Act
    var result = await service.CreateCategoryAsync(dto);
    
    // Assert
    Assert.IsNotNull(result);
    mockCategoryRepo.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
}
```

### Benefits
- Easy to mock repositories
- Test business logic in isolation
- No database required
- Fast and reliable tests

---

## Key Design Patterns

### 1. **Repository Pattern**
- Abstracts data access layer
- Single interface for CRUD operations
- Easy to swap implementations

### 2. **Service Pattern**
- Encapsulates business logic
- Coordinates multiple repositories
- Handles cross-cutting concerns (logging, validation)

### 3. **Dependency Injection**
- Loose coupling between layers
- Easy to extend/maintain
- Testable code

### 4. **DTO Pattern**
- Separates API contracts from models
- Version independence
- Security through reduced data exposure

---

## Best Practices Implemented

✅ **Separation of Concerns** - Each layer has single responsibility  
✅ **Dependency Inversion** - Code depends on abstractions, not concrete types  
✅ **DRY (Don't Repeat Yourself)** - Generic repository for common CRUD  
✅ **Logging** - Every layer logs appropriately  
✅ **Error Handling** - Try-catch at service level with specific exceptions  
✅ **Validation** - DTOs and service-level validation  
✅ **Async/Await** - All database operations are async  
✅ **CORS** - Configured for frontend integration  

---

## API Endpoints Summary

### Categories
- `GET /api/categories` - Calls CategoryService.GetAllCategoriesAsync()
- `GET /api/categories/by-type/{id}` - Calls CategoryService.GetCategoriesByTypeAsync(id)
- `POST /api/categories` - Calls CategoryService.CreateCategoryAsync(dto)

### Transactions
- `GET /api/transactions` - Calls TransactionService.GetAllTransactionsAsync()
- `GET /api/transactions/{id}` - Calls TransactionService.GetTransactionByIdAsync(id)
- `POST /api/transactions` - Calls TransactionService.CreateTransactionAsync(dto)
- `PUT /api/transactions/{id}` - Calls TransactionService.UpdateTransactionAsync(id, dto)
- `DELETE /api/transactions/{id}` - Calls TransactionService.DeleteTransactionAsync(id)

### Dashboard
- `GET /api/dashboard/summary` - Calls DashboardService.GetDashboardSummaryAsync()
- `GET /api/dashboard/monthly-summary` - Calls DashboardService.GetMonthlySummaryAsync()

---

## Adding New Features

### To add a new entity (e.g., Budget):

1. **Create Model** (`Models/Budget.cs`)
2. **Add DbSet** to ApplicationDbContext
3. **Create DTO** (`DTOs/BudgetDTOs.cs`)
4. **Create Repository** (implement IBudgetRepository extending IRepository<Budget>)
5. **Create Service** (implement IBudgetService)
6. **Create Controller** (using IBudgetService)
7. **Register in Program.cs** (DI configuration)

---

## Conclusion

This layered architecture ensures:
- **Maintainability** - Clear separation of concerns
- **Testability** - Easy to unit test each layer
- **Scalability** - Easy to add new features
- **Flexibility** - Easy to swap implementations
- **Professional** - Industry-standard architecture

