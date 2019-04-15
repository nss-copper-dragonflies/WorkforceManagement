DROP TABLE IF EXISTS ComputerEmployee;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Department;

CREATE TABLE Department (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(55) NOT NULL,
    Budget  INTEGER NOT NULL
);
CREATE TABLE Employee (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL,
    DepartmentId INTEGER NOT NULL,
    IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);
CREATE TABLE Computer (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    PurchaseDate DATETIME NOT NULL,
    DecomissionDate DATETIME,
    Make VARCHAR(55) NOT NULL,
    Manufacturer VARCHAR(55) NOT NULL
);
CREATE TABLE ComputerEmployee (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    EmployeeId INTEGER NOT NULL,
    ComputerId INTEGER NOT NULL,
    AssignDate DATETIME NOT NULL,
    UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);
CREATE TABLE TrainingProgram (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    MaxAttendees INTEGER NOT NULL
);
CREATE TABLE EmployeeTraining (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    EmployeeId INTEGER NOT NULL,
    TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2019', '01 Jan 2024', 'MacBook Pro', 'Apple')
insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2018', '01 Jan 2023', 'Inspiron', 'Dell')
insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2017', '01 Jan 2022', 'MacBook Air', 'Apple')
insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2017', '01 Jan 2022', 'Mac Pro 2017', 'Apple')
insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2017', '01 Jan 2022', 'XPS Professional', 'Dell')


insert into Department ([Name], Budget) values ('Accounting', 400000)
insert into Department ([Name], Budget) values ('IT', 40000)
insert into Department ([Name], Budget) values ('Sales', 450000)

insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Hernando', 'Rivera', 1, 0)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Mary', 'Phillips', 2, 0)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Lorenzo', 'Lopez', 3, 1)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Terry', 'Cruz', 3, 1)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Orlando', 'Blume', 3, 1)

insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (1, 1, '01 Jan 2019', NULL)
insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (2, 2, '01 Jan 2018', NULL)
insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (3, 3, '01 Jan 2017', NULL)

insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Count Beans', '14 Feb 2019', '15 Feb 2019', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Spell "IT"', '14 Feb 2019', '15 Feb 2019', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Sell Beans', '14 Feb 2021', '15 Feb 2021', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Sell Cars', '14 Feb 2020', '15 Feb 2020', 12)

insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (1, 1)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (1, 3)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (1, 4)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (2, 1)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (2, 2)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (3, 3)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (3, 4)

ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];