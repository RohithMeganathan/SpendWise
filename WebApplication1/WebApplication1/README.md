# Income Expense Management API

## Project Architecture

### Folder Structure
```
WebApplication1/
├── Database/              # SQL Scripts for database initialization
├── Data/                  # Entity Framework Core DbContext
├── Models/                # EF Core Models (TransactionType, Category, Transaction)
├── Controllers/           # API Controllers
│   ├── CategoriesController.cs
│   ├── TransactionsController.cs
│   └── DashboardController.cs
├── Properties/            # App settings and launch profiles
├── appsettings.json       # Configuration with ConnectionStrings
└── Program.cs             # Application startup configuration
```

## Database Schema

### TransactionType Table
- `Id` (TINYINT, PK)
- `Name` (NVARCHAR(10))
  - Values: 'Income', 'Expense'

### Category Table
- `Id` (TINYINT, PK, Identity)
- `Name` (NVARCHAR(50))
- `TransactionTypeId` (FK to TransactionType)

### Transactions Table
- `Id` (BIGINT, PK, Identity)
- `Name` (NVARCHAR(50))
- `Description` (NVARCHAR(500), nullable)
- `CategoryId` (FK to Category)
- `Amount` (DECIMAL(10,2))
- `TransactionDate` (DATETIME2, default: GETUTCDATE())

## API Endpoints

### Categories
- **GET** `/api/categories` - Get all categories
- **GET** `/api/categories/by-type/{transactionTypeId}` - Get categories by transaction type (1=Income, 2=Expense)
- **POST** `/api/categories` - Create a new category
  ```json
  {
    "name": "Salary",
    "transactionTypeId": 1
  }
  ```

### Transactions
- **GET** `/api/transactions` - Get all transactions (with optional filters)
  - Query params: `categoryId`, `startDate`, `endDate`
- **GET** `/api/transactions/{id}` - Get a specific transaction
- **POST** `/api/transactions` - Create a new transaction
  ```json
  {
    "name": "Monthly Salary",
    "description": "Regular monthly income",
    "categoryId": 1,
    "amount": 5000,
    "transactionDate": "2026-03-27T00:00:00Z"
  }
  ```
- **PUT** `/api/transactions/{id}` - Update a transaction
  ```json
  {
    "name": "Updated Name",
    "description": "Updated Description",
    "categoryId": 2,
    "amount": 5500
  }
  ```
- **DELETE** `/api/transactions/{id}` - Delete a transaction

### Dashboard
- **GET** `/api/dashboard/summary` - Get dashboard summary
  - Query params: `startDate`, `endDate` (defaults to last 30 days)
  - Returns: Total income, total expense, balance, and breakdown by category
  
- **GET** `/api/dashboard/monthly-summary` - Get monthly summary for past 12 months
  - Returns: Income, expense, and transaction count by month

## Setup Instructions

### 1. Database Setup
Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=IncomeExpenseManagementDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

### 2. Create Database
Run the SQL script from `Database/01_InitialSchema.sql` to create tables and seed initial data.

### 3. Install Dependencies
The required NuGet packages are already configured in the .csproj:
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.OpenApi

### 4. Run the Application
```bash
dotnet run
```

The application will start on `https://localhost:5001` or `http://localhost:5000` depending on your configuration.

## Key Features

1. **EF Core Integration** - Using Entity Framework Core for data access
2. **Proper Data Validation** - Input validation on all DTOs
3. **Error Handling** - Try-catch blocks with detailed logging
4. **Logging** - ILogger implementation on all controllers
5. **DTOs** - Data Transfer Objects for API contracts
6. **CORS Support** - Enabled for frontend integration
7. **Transaction Filtering** - Filter by date range, category
8. **Dashboard Analytics** - Summary data with category breakdowns

## Technologies Used
- .NET 9.0
- Entity Framework Core 9.0
- SQL Server
- ASP.NET Core Web API
