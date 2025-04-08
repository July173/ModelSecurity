

CREATE TABLE Rol (
    Id SERIAL PRIMARY KEY,
    TypeRol VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL
);

CREATE TABLE Verification (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP,
    DeleteDate TIMESTAMP,
    UpdaDate TIMESTAMP
);

CREATE TABLE Person (
    Id SERIAL PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    Name VARCHAR(255) NOT NULL,
    FirstName VARCHAR(255),
    SecondName VARCHAR(255),
    FirstLastName VARCHAR(255),
    SecondLastName VARCHAR(255),
    PhoneNumber VARCHAR(50),
    Email VARCHAR(255) UNIQUE,
    TypeIdentification VARCHAR(50) NOT NULL,
    NumberIdentification INT NOT NULL UNIQUE,
    Signig BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Regional (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    Description TEXT,
    CodeRegional VARCHAR(100) UNIQUE NOT NULL,
    Active BOOLEAN NOT NULL
);

CREATE TABLE "User" (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PersonId INT NOT NULL REFERENCES Person(Id),
    Active BOOLEAN NOT NULL
);

CREATE TABLE UserRol (
    Id SERIAL PRIMARY KEY,
    UserId INT NOT NULL REFERENCES "User"(Id),
    RolId INT NOT NULL REFERENCES Rol(Id)
);
CREATE TABLE Center (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    CodeCenter VARCHAR(50) NOT NULL,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    RegionalId INT NOT NULL REFERENCES Regional(Id)
);

CREATE TABLE Sede (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    CodeSede VARCHAR(50) UNIQUE NOT NULL,
    Address VARCHAR(255),
    PhoneSede VARCHAR(50),
    EmailContact VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    CenterId INT NOT NULL REFERENCES Center(Id)
);

CREATE TABLE UserSede (
    Id SERIAL PRIMARY KEY,
    StatusProcedure VARCHAR(255),
    UserId INT NOT NULL REFERENCES "User"(Id),
    SedeId INT NOT NULL REFERENCES Sede(Id)
);

CREATE TABLE Aprendiz (
    Id SERIAL PRIMARY KEY,
    PreviousProgram VARCHAR(255),
    Active BOOLEAN NOT NULL,
    UserId INT NOT NULL REFERENCES "User"(Id)
);

CREATE TABLE Instructor (
    Id SERIAL PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    UserId INT NOT NULL REFERENCES "User"(Id)
);

CREATE TABLE Program (
    Id SERIAL PRIMARY KEY,
    CodeProgram NUMERIC NOT NULL UNIQUE,
    Name VARCHAR(255) NOT NULL,
    TypeProgram VARCHAR(255) NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    Active BOOLEAN NOT NULL,
    Description TEXT
);
CREATE TABLE Process (
    Id SERIAL PRIMARY KEY,
    StartAprendiz VARCHAR(255) NOT NULL,
    Observation TEXT,
    TypeProcess VARCHAR(255) NOT NULL,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE AprendizProgram (
    Id SERIAL PRIMARY KEY,
    AprendizId INT NOT NULL REFERENCES Aprendiz(Id),
    ProgramId INT NOT NULL REFERENCES Program(Id)
);

CREATE TABLE InstructorProgram (
    Id SERIAL PRIMARY KEY,
    InstructorId INT NOT NULL REFERENCES Instructor(Id),
    ProgramId INT NOT NULL REFERENCES Program(Id)
);

CREATE TABLE Module (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Form (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Cuestion TEXT,
    TypeCuestion VARCHAR(255),
    Answer TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE FormModule (
    Id SERIAL PRIMARY KEY,
    StatusProcedure VARCHAR(255) NOT NULL,
    FormId INT NOT NULL REFERENCES Form(Id),
    ModuleId INT NOT NULL REFERENCES Module(Id)
);
CREATE TABLE RolForm (
    Id SERIAL PRIMARY KEY,
    Permission VARCHAR(255),
    RolId INT NOT NULL REFERENCES Rol(Id),
    FormId INT NOT NULL REFERENCES Form(Id)
);

CREATE TABLE TypeModality (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL
);

CREATE TABLE "State" (
    Id SERIAL PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    Description VARCHAR(255),
    TypeState VARCHAR(255)
);

CREATE TABLE RegisterySofia (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Document VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Enterprise (
    Id SERIAL PRIMARY KEY,
    Observation TEXT,
    NameEnterprise VARCHAR(255) NOT NULL,
    PhoneEnterprise VARCHAR(50),
    Locate VARCHAR(255),
    NitEnterprise VARCHAR(50) NOT NULL UNIQUE,
    EmailEnterprise VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Concept (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);
CREATE TABLE ChangeLog (
    Id SERIAL PRIMARY KEY,
    TableName VARCHAR(255) NOT NULL,
    IdTable INT NOT NULL,
    OldValues TEXT,
    NewValues TEXT,
    Action VARCHAR(50) NOT NULL,
    Active BOOLEAN NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    UpdateDate TIMESTAMP
);

CREATE TABLE AprendizProcessInstructor (
    Id SERIAL PRIMARY KEY,
    TypeModalityId INT NOT NULL REFERENCES TypeModality(Id),
    RegisterySofiaId INT NOT NULL REFERENCES RegisterySofia(Id),
    ConceptId INT NOT NULL REFERENCES Concept(Id),
    EnterpriseId INT NOT NULL REFERENCES Enterprise(Id),
    ProcessId INT NOT NULL REFERENCES Process(Id),
    AprendizId INT NOT NULL REFERENCES Aprendiz(Id),
    InstructorId INT NOT NULL REFERENCES Instructor(Id),
    StateId INT NOT NULL REFERENCES "State"(Id),
    VerificationId INT NOT NULL REFERENCES Verification(Id)
);