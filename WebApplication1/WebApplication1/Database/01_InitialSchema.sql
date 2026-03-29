-- Create TransactionType table
CREATE TABLE TransactionType (
    Id TINYINT PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL
);

-- Insert transaction types
INSERT INTO TransactionType (Id, Name)
VALUES (1, 'Income'), (2, 'Expense');

-- Create Category table
CREATE TABLE Category (
    Id TINYINT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    TransactionTypeId TINYINT NOT NULL,
    FOREIGN KEY (TransactionTypeId) REFERENCES TransactionType(Id)
);

-- Create Transactions table
CREATE TABLE Transactions (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    CategoryId TINYINT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    TransactionDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);

-- Create indexes for better query performance
CREATE INDEX IX_Category_TransactionTypeId ON Category(TransactionTypeId);
CREATE INDEX IX_Transactions_CategoryId ON Transactions(CategoryId);
CREATE INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate);
