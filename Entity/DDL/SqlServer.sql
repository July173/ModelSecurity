CREATE TABLE Regional (
    Id INT PRIMARY KEY IDENTITY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    Description VARCHAR(MAX),
    CodeRegional VARCHAR(100) UNIQUE NOT NULL,
    Active BIT NOT NULL
);
