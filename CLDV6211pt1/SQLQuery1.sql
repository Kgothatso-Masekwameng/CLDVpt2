USE RegistrationDB

-- TABLE CREATION
CREATE TABLE Student (
	StudentID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name] VARCHAR (250) NOT NULL,
	Surname VARCHAR (250) NOT NULL,
	email VARCHAR (250) NOT NULL

);

--TABLE INSERTION
INSERT INTO Student ([Name], Surname, Email)
VALUES ('Juliana', 'Adisa', 'adeola.adisa@gmail.com')
--VALUES ('Adeola', 'Adisa', 'adeola.adis@gmail.com')

--TABLE MANIPULATTION
SELECT *
FROM Student
