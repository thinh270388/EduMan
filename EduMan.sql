﻿CREATE DATABASE EduMan
GO

USE EduMan
GO

CREATE TABLE dbo.Level(
	Id INT PRIMARY KEY IDENTITY,
	LevelName NVARCHAR(50) NOT NULL
)
GO

CREATE TABLE dbo.Grade(
	Id INT PRIMARY KEY IDENTITY,
	GradeName NVARCHAR(50) NOT NULL,
	LevelId INT FOREIGN KEY REFERENCES dbo.Level(Id) NOT NULL
)
GO

CREATE TABLE dbo.Class(
	Id INT PRIMARY KEY IDENTITY,
	ClassName NVARCHAR(50) NOT NULL,
	GradeId INT FOREIGN KEY REFERENCES dbo.Grade(Id) NOT NULL
)
GO

CREATE TABLE dbo.Teacher(
	Id INT PRIMARY KEY IDENTITY,
	Code VARCHAR(20) NOT NULL,
	FullName NVARCHAR(50) NOT NULL,
	Phone NVARCHAR(50) NULL,
	Email NVARCHAR(100) NULL
)
GO

CREATE TABLE dbo.ClassDistribute(
	Id INT PRIMARY KEY IDENTITY,
	ClassId INT FOREIGN KEY REFERENCES dbo.Class(Id) NOT NULL,
	TeacherId INT FOREIGN KEY REFERENCES dbo.Teacher(Id) NOT NULL,
	AssignDate DATE NOT NULL,
	OnYear VARCHAR(20) NOT NULL,
	Note NVARCHAR(512) NULL
)
GO

CREATE TABLE dbo.Student(
	Id INT PRIMARY KEY IDENTITY,
	Code VARCHAR(20) NOT NULL,
	FullName NVARCHAR(50) NOT NULL,
	Birthday DATE NULL,
	Gender BIT NULL,
	Phone NVARCHAR(50) NULL,
	Email NVARCHAR(50) NULL,
	AddressCurrent NVARCHAR(100) NULL,
	ContactInfo NVARCHAR(100) NULL,
	SequenceNumber INT NULL,
	Note NVARCHAR(250) NULL
)
GO

CREATE TABLE dbo.StudentDistribute(
	Id INT PRIMARY KEY IDENTITY,
	ClassDistributeId INT FOREIGN KEY REFERENCES dbo.ClassDistribute(Id) NOT NULL,
	StudentId INT FOREIGN KEY REFERENCES dbo.Student(Id) NOT NULL,
	AssignDate DATE NOT NULL,
	Note NVARCHAR(512) NULL
)
GO

CREATE TABLE dbo.DisciplineGroup(
	Id INT PRIMARY KEY IDENTITY,
	DisciplineGroupName NVARCHAR(250) NOT NULL
)
GO

CREATE TABLE dbo.DisciplineType(
	Id INT PRIMARY KEY IDENTITY,
	DisciplineTypeName NVARCHAR(50) NOT NULL
)
GO

CREATE TABLE dbo.Discipline(
	Id INT PRIMARY KEY IDENTITY,
	DisciplineName NVARCHAR(250) NOT NULL,
	DisciplineGroupId INT FOREIGN KEY REFERENCES dbo.DisciplineGroup(Id) NOT NULL,
	ApplyFor INT NOT NULL,
	PlusPoint INT DEFAULT 0 NOT NULL,
	MinusPoint INT DEFAULT 0 NOT NULL,
	Display BIT DEFAULT 1 NOT NULL,
	DisciplineTypeId INT FOREIGN KEY REFERENCES dbo.DisciplineType(Id) NOT NULL,
	SequenceNumber INT DEFAULT 0 NULL,
	Note NVARCHAR(250) NULL
)
GO

CREATE TABLE dbo.StartWeek(
	Id INT PRIMARY KEY IDENTITY,
	OnYear VARCHAR(20) NOT NULL,
	StartDate DATE NOT NULL,
	Used BIT NOT NULL
)
GO

CREATE TABLE dbo.Weekly(
	Id INT PRIMARY KEY IDENTITY,
	StartWeekId INT FOREIGN KEY REFERENCES dbo.StartWeek(Id) NOT NULL,
	WeeklyName NVARCHAR(20) DEFAULT N'Tuần ' NOT NULL,
	FromDate DATE NOT NULL,
	ToDate DATE NOT NULL,
	NumberOfLession INT DEFAULT 30 NOT NULL,
	InitialPoint INT DEFAULT 140 NOT NULL,
	Coefficient INT DEFAULT 1 NOT NULL,
	OnDutyClass NVARCHAR(50) NULL,
	Sumarizing NVARCHAR(4000) NULL,
	Planning NVARCHAR(4000) NULL,
)
GO

CREATE TABLE dbo.StudentDiscipline(
	Id INT PRIMARY KEY IDENTITY,
	StudentId INT FOREIGN KEY REFERENCES dbo.Student(Id) NOT NULL,
	DisciplineId INT FOREIGN KEY REFERENCES dbo.Discipline(Id) NOT NULL,
	WeeklyId INT FOREIGN KEY REFERENCES dbo.Weekly(Id) NOT NULL,
	OnDate DATE NOT NULL,
	Times INT NOT NULL,
)
GO

CREATE TABLE dbo.ClassDiscipline(
	Id INT PRIMARY KEY IDENTITY,
	ClassDistributeId INT FOREIGN KEY REFERENCES dbo.ClassDistribute(Id) NOT NULL,
	DisciplineId INT FOREIGN KEY REFERENCES dbo.Discipline(Id) NOT NULL,
	WeeklyId INT FOREIGN KEY REFERENCES dbo.Weekly(Id) NOT NULL,
	OnDate DATE NOT NULL,
	Times INT NOT NULL
)
GO

CREATE TABLE dbo.DroppedOut(
	Id INT PRIMARY KEY IDENTITY,
	StudentId INT FOREIGN KEY REFERENCES dbo.Student(Id) NOT NULL,
	Semaster NVARCHAR(20) NOT NULL,
	OnDate DATE NOT NULL,
	Reason NVARCHAR(100) NULL,
	DecisionNumber NVARCHAR(50) NULL,
	DecisionDate DATE NULL,
	Note NVARCHAR(250) NULL
)
GO

CREATE TABLE dbo.GroupUser(
	Id INT PRIMARY KEY IDENTITY,
	GroupUserName NVARCHAR(50) NOT NULL
)
GO

CREATE TABLE dbo.UserInfo(
	Id INT PRIMARY KEY IDENTITY,
	UserName VARCHAR(20) NOT NULL,
	UserPassword NVARCHAR(250) NOT NULL,
	FullName NVARCHAR(50) NULL,
	GroupUserId INT FOREIGN KEY REFERENCES dbo.GroupUser(Id) NOT NULL,
	Active BIT DEFAULT 1 NOT NULL
)
GO

CREATE TABLE dbo.Funct(
	Id INT PRIMARY KEY IDENTITY,
	FunctName NVARCHAR(50) NOT NULL
)
GO

CREATE TABLE dbo.RoleAssign(
	Id INT PRIMARY KEY IDENTITY,
	GroupUserId INT FOREIGN KEY REFERENCES dbo.GroupUser(Id) NOT NULL,
	FunctId INT FOREIGN KEY REFERENCES dbo.Funct(Id) NOT NULL
)
GO

INSERT INTO dbo.GroupUser (GroupUserName) VALUES (N'Quản trị')
GO
INSERT INTO dbo.GroupUser (GroupUserName) VALUES (N'Tổ quản lý')
GO
INSERT INTO dbo.GroupUser (GroupUserName) VALUES (N'Giáo viên')
GO
INSERT INTO dbo.GroupUser (GroupUserName) VALUES (N'Học sinh')
GO

INSERT INTO dbo.UserInfo (UserName, UserPassword, FullName, GroupUserId, Active) VALUES('admin', N'VbUPmf2DCAOG4OQw1tw2uA==', N'Administrator', 1, 1)
GO
INSERT INTO dbo.UserInfo (UserName, UserPassword, FullName, GroupUserId, Active) VALUES('0775426999', N'0KZ+uJRgpohWbSrNjdz7U4A+s1Tg83lfCu0yI+fnUvc=', N'Lê Phúc Nhã Thịnh', 2, 1)
GO

