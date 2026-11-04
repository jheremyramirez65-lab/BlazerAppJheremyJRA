USE master
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'BDBIBLIOTECA')
BEGIN
    ALTER DATABASE BDBIBLIOTECA SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE BDBIBLIOTECA
END
GO


CREATE DATABASE BDBIBLIOTECA
GO


USE BDBIBLIOTECA
GO


--------------------------------------------------
-- TABLA SEXO
--------------------------------------------------

CREATE TABLE Sexo
(
    IIDSEXO INT IDENTITY(1,1) PRIMARY KEY,
    NOMBRE VARCHAR(100),
    BHABILITADO INT
)
GO


--------------------------------------------------
-- TABLA PAIS
--------------------------------------------------

CREATE TABLE Pais
(
    IIDPAIS INT IDENTITY(1,1) PRIMARY KEY,
    NOMBRE VARCHAR(100),
    BHABILITADO INT
)
GO


--------------------------------------------------
-- TABLA AUTOR
--------------------------------------------------

CREATE TABLE Autor
(
    IIDAUTOR INT IDENTITY(1,1) PRIMARY KEY,

    NOMBRE VARCHAR(200),
    APPATERNO VARCHAR(200),
    APMATERNO VARCHAR(200),

    IIDSEXO INT,
    IIDPAIS INT,

    DESCRIPCION VARCHAR(500),

    BHABILITADO INT,


    CONSTRAINT FK_Autor_Sexo
    FOREIGN KEY(IIDSEXO)
    REFERENCES Sexo(IIDSEXO),


    CONSTRAINT FK_Autor_Pais
    FOREIGN KEY(IIDPAIS)
    REFERENCES Pais(IIDPAIS)
)
GO



--------------------------------------------------
-- TABLA TIPO LIBRO
--------------------------------------------------

CREATE TABLE TipoLibro
(
    idTipoLibro INT IDENTITY(1,1) PRIMARY KEY,

    nombreTipoLibro VARCHAR(100),

    descripcion VARCHAR(300),

    BHABILITADO INT
)
GO



--------------------------------------------------
-- TABLA LIBRO
--------------------------------------------------

CREATE TABLE Libro
(
    idLibro INT IDENTITY(1,1) PRIMARY KEY,


    codigo VARCHAR(50),

    titulo VARCHAR(200),

    resumen VARCHAR(MAX),


    autor VARCHAR(200),

    editorial VARCHAR(200),


    anio INT,

    numeroPaginas INT,


    categoria VARCHAR(100),



    imagen VARBINARY(MAX),

    archivo VARBINARY(MAX),

    nombrearchivo VARCHAR(200),



    idTipoLibro INT,



    CONSTRAINT FK_Libro_TipoLibro
    FOREIGN KEY(idTipoLibro)
    REFERENCES TipoLibro(idTipoLibro)

)
GO



--------------------------------------------------
-- TABLA TIPO USUARIO
--------------------------------------------------

CREATE TABLE TipoUsuario
(
    IIDTIPOUSUARIO INT IDENTITY(1,1) PRIMARY KEY,

    NOMBRE VARCHAR(100),

    BHABILITADO INT
)
GO



--------------------------------------------------
-- TABLA USUARIO
--------------------------------------------------

CREATE TABLE Usuario
(
    IIDUSUARIO INT IDENTITY(1,1) PRIMARY KEY,


    NOMBRE VARCHAR(100),

    CORREO VARCHAR(150),

    PASSWORD VARCHAR(200),


    IIDTIPOUSUARIO INT,


    BHABILITADO INT,


    CONSTRAINT FK_Usuario_TipoUsuario
    FOREIGN KEY(IIDTIPOUSUARIO)
    REFERENCES TipoUsuario(IIDTIPOUSUARIO)

)
GO



--------------------------------------------------
-- TABLA RESERVA ESTADO
--------------------------------------------------

CREATE TABLE ReservaEstado
(
    IIDRESERVAESTADO INT IDENTITY(1,1) PRIMARY KEY,

    NOMBRE VARCHAR(100),

    BHABILITADO INT
)
GO



--------------------------------------------------
-- TABLA RESERVA
--------------------------------------------------

CREATE TABLE Reserva
(
    IIDRESERVA INT IDENTITY(1,1) PRIMARY KEY,


    IIDUSUARIO INT,


    IIDRESERVAESTADO INT,


    FECHA DATETIME DEFAULT GETDATE(),



    CONSTRAINT FK_Reserva_Usuario
    FOREIGN KEY(IIDUSUARIO)
    REFERENCES Usuario(IIDUSUARIO),


    CONSTRAINT FK_Reserva_Estado
    FOREIGN KEY(IIDRESERVAESTADO)
    REFERENCES ReservaEstado(IIDRESERVAESTADO)

)
GO



--------------------------------------------------
-- DETALLE RESERVA
--------------------------------------------------

CREATE TABLE DetalleReserva
(
    IIDDETALLERESERVA INT IDENTITY(1,1) PRIMARY KEY,


    IIDRESERVA INT,

    idLibro INT,


    CANTIDAD INT,


    CONSTRAINT FK_Detalle_Reserva
    FOREIGN KEY(IIDRESERVA)
    REFERENCES Reserva(IIDRESERVA),


    CONSTRAINT FK_Detalle_Libro
    FOREIGN KEY(idLibro)
    REFERENCES Libro(idLibro)

)
GO



--------------------------------------------------
-- HISTORIAL RESERVA
--------------------------------------------------

CREATE TABLE ReservaHistorial
(
    IIDHISTORIAL INT IDENTITY(1,1) PRIMARY KEY,

    IIDRESERVA INT,

    DESCRIPCION VARCHAR(300),

    FECHA DATETIME DEFAULT GETDATE(),


    CONSTRAINT FK_Historial_Reserva
    FOREIGN KEY(IIDRESERVA)
    REFERENCES Reserva(IIDRESERVA)

)
GO



--------------------------------------------------
-- TABLAS MENU / PERMISOS
--------------------------------------------------

CREATE TABLE Pagina
(
    IIDPAGINA INT IDENTITY(1,1) PRIMARY KEY,

    NOMBRE VARCHAR(100),

    URL VARCHAR(200)
)
GO



CREATE TABLE Button
(
    IIDBUTTON INT IDENTITY(1,1) PRIMARY KEY,

    NOMBRE VARCHAR(100)
)
GO



CREATE TABLE PaginaTipoUsuario
(
    IIDPAGINATIPOUSUARIO INT IDENTITY(1,1) PRIMARY KEY,


    IIDPAGINA INT,

    IIDTIPOUSUARIO INT,


    FOREIGN KEY(IIDPAGINA)
    REFERENCES Pagina(IIDPAGINA),


    FOREIGN KEY(IIDTIPOUSUARIO)
    REFERENCES TipoUsuario(IIDTIPOUSUARIO)

)
GO



CREATE TABLE PaginaTipoUsuButton
(
    IIDPAGINATIPOUSUBUTTON INT IDENTITY(1,1) PRIMARY KEY,


    IIDPAGINATIPOUSUARIO INT,

    IIDBUTTON INT,


    FOREIGN KEY(IIDPAGINATIPOUSUARIO)
    REFERENCES PaginaTipoUsuario(IIDPAGINATIPOUSUARIO),


    FOREIGN KEY(IIDBUTTON)
    REFERENCES Button(IIDBUTTON)

)
GO



--------------------------------------------------
-- DATOS DE PRUEBA
--------------------------------------------------


INSERT INTO Sexo VALUES
('Masculino',1),
('Femenino',1)
GO


INSERT INTO Pais VALUES
('Bolivia',1),
('Peru',1),
('Argentina',1)
GO



INSERT INTO TipoLibro(nombreTipoLibro,descripcion,BHABILITADO)
VALUES
('Novela','Libros de novela',1),
('Cuento','Libros infantiles',1),
('Tecnico','Libros educativos',1)
GO



INSERT INTO Libro
(
codigo,
titulo,
resumen,
autor,
editorial,
anio,
numeroPaginas,
categoria,
idTipoLibro
)
VALUES

(
'L001',
'Caperucita Roja',
'Cuento infantil',
'Charles Perrault',
'Editorial ABC',
2020,
50,
'Cuento',
2
),

(
'L002',
'Programacion C#',
'Libro de programacion',
'Microsoft',
'Microsoft Press',
2024,
400,
'Tecnologia',
3
)
GO



INSERT INTO TipoUsuario
VALUES
('Administrador',1),
('Usuario',1)
GO



INSERT INTO Usuario
(
NOMBRE,
CORREO,
PASSWORD,
IIDTIPOUSUARIO,
BHABILITADO
)
VALUES
(
'Admin',
'admin@gmail.com',
'123456',
1,
1
)
GO



PRINT 'BASE DE DATOS BDBIBLIOTECA CREADA CORRECTAMENTE'
GO




SELECT idLibro, titulo, Imagen
FROM Libro
WHERE idLibro = 1


SELECT *
FROM Autor


ALTER TABLE Autor
ADD IMAGEN VARBINARY(MAX) NULL
GO


SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Autor'


INSERT INTO Autor 
(NOMBRE, APPATERNO, APMATERNO, IIDSEXO, IIDPAIS, DESCRIPCION, BHABILITADO) 
VALUES 
('Gabriel', 'Garcia', 'Marquez', 1, 1, 'Escritor colombiano', 1)
GO


SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS TipoDato
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Autor'
GO

SELECT *
FROM Autor
WHERE BHABILITADO = 1

SELECT *
FROM Autor
WHERE BHABILITADO = 0