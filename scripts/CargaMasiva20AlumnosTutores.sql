SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @IdEscuela int = (SELECT TOP (1) IdEscuela FROM Escuelas ORDER BY IdEscuela);
DECLARE @IdRolTutor int = (SELECT TOP (1) IdRol FROM Roles WHERE Nombre = N'Tutor');
DECLARE @Ahora datetime2 = SYSDATETIME();
DECLARE @HashContrasena nvarchar(max) = N'$2a$11$fvFi0fuB1RVHvYJeu4lsLelTZyVr9giIRqXObby.Tqn.RwNtYGUV.';

IF @IdEscuela IS NULL THROW 50001, 'No existe una escuela para cargar los datos.', 1;
IF @IdRolTutor IS NULL THROW 50002, 'No existe el rol Tutor.', 1;

DECLARE @Datos TABLE (
    Numero int PRIMARY KEY, DniAlumno int, NombreAlumno nvarchar(100), ApellidoAlumno nvarchar(100),
    FechaNacimiento date, DniTutor int, NombreTutor nvarchar(100), ApellidoTutor nvarchar(100), Parentesco nvarchar(50)
);

INSERT INTO @Datos VALUES
(1, 51010001,N'Sofía',N'Gómez','2015-03-12',31010001,N'Carolina',N'Gómez',N'Madre'),
(2, 51010002,N'Mateo',N'Rodríguez','2015-05-24',31010002,N'Pablo',N'Rodríguez',N'Padre'),
(3, 51010003,N'Valentina',N'Fernández','2014-11-08',31010003,N'Laura',N'Fernández',N'Madre'),
(4, 51010004,N'Benjamín',N'López','2015-01-19',31010004,N'Martín',N'López',N'Padre'),
(5, 51010005,N'Emma',N'Martínez','2014-08-03',31010005,N'Natalia',N'Martínez',N'Madre'),
(6, 51010006,N'Joaquín',N'Pérez','2015-06-17',31010006,N'Andrés',N'Pérez',N'Padre'),
(7, 51010007,N'Olivia',N'Sánchez','2014-12-29',31010007,N'Mariana',N'Sánchez',N'Madre'),
(8, 51010008,N'Thiago',N'Romero','2015-02-11',31010008,N'Diego',N'Romero',N'Padre'),
(9, 51010009,N'Catalina',N'Díaz','2014-09-15',31010009,N'Gabriela',N'Díaz',N'Madre'),
(10,51010010,N'Felipe',N'Torres','2015-04-06',31010010,N'Sergio',N'Torres',N'Padre'),
(11,51010011,N'Isabella',N'Álvarez','2014-07-22',31010011,N'Verónica',N'Álvarez',N'Madre'),
(12,51010012,N'Bautista',N'Ruiz','2015-10-13',31010012,N'Fernando',N'Ruiz',N'Padre'),
(13,51010013,N'Mía',N'Acosta','2014-06-30',31010013,N'Paula',N'Acosta',N'Madre'),
(14,51010014,N'Santino',N'Benítez','2015-09-04',31010014,N'Gustavo',N'Benítez',N'Padre'),
(15,51010015,N'Josefina',N'Medina','2014-10-26',31010015,N'Patricia',N'Medina',N'Madre'),
(16,51010016,N'Franco',N'Herrera','2015-07-09',31010016,N'Ricardo',N'Herrera',N'Padre'),
(17,51010017,N'Antonella',N'Castro','2014-04-18',31010017,N'Mónica',N'Castro',N'Madre'),
(18,51010018,N'Ignacio',N'Ortiz','2015-12-01',31010018,N'Javier',N'Ortiz',N'Padre'),
(19,51010019,N'Malena',N'Silva','2014-05-14',31010019,N'Claudia',N'Silva',N'Madre'),
(20,51010020,N'Lucas',N'Ramos','2015-08-21',31010020,N'Marcelo',N'Ramos',N'Padre');

INSERT INTO Alumnos (IdEscuela,Dni,Nombre,Apellido,FechaNacimiento,Activo,FechaCrea,FechaAct)
SELECT @IdEscuela,DniAlumno,NombreAlumno,ApellidoAlumno,FechaNacimiento,1,@Ahora,@Ahora
FROM @Datos d WHERE NOT EXISTS (SELECT 1 FROM Alumnos a WHERE a.IdEscuela=@IdEscuela AND a.Dni=d.DniAlumno);

INSERT INTO Usuarios (IdRol,IdEscuela,Dni,Nombre,Apellido,Email,NombreUsuario,HashContrasena,Activo,FechaCrea,FechaAct)
SELECT @IdRolTutor,@IdEscuela,DniTutor,NombreTutor,ApellidoTutor,
       CONCAT(N'tutor.masivo',RIGHT(CONCAT('00',Numero),2),N'@educar.test'),
       CONCAT(N'tutor.masivo',RIGHT(CONCAT('00',Numero),2)),@HashContrasena,1,@Ahora,@Ahora
FROM @Datos d WHERE NOT EXISTS (SELECT 1 FROM Usuarios u WHERE u.IdEscuela=@IdEscuela AND u.Dni=d.DniTutor);

INSERT INTO Tutores (IdUsuario,EsResponsable,FechaCrea,FechaAct)
SELECT u.IdUsuario,1,@Ahora,@Ahora FROM @Datos d
JOIN Usuarios u ON u.IdEscuela=@IdEscuela AND u.Dni=d.DniTutor
WHERE NOT EXISTS (SELECT 1 FROM Tutores t WHERE t.IdUsuario=u.IdUsuario);

INSERT INTO AlumnoTutores (IdAlumno,IdTutor,Parentesco,EsResponsablePrinc,Activo,FechaCrea,FechaAct)
SELECT a.IdAlumno,t.IdTutor,d.Parentesco,1,1,@Ahora,@Ahora FROM @Datos d
JOIN Alumnos a ON a.IdEscuela=@IdEscuela AND a.Dni=d.DniAlumno
JOIN Usuarios u ON u.IdEscuela=@IdEscuela AND u.Dni=d.DniTutor
JOIN Tutores t ON t.IdUsuario=u.IdUsuario
WHERE NOT EXISTS (SELECT 1 FROM AlumnoTutores at WHERE at.IdAlumno=a.IdAlumno AND at.IdTutor=t.IdTutor);

UPDATE at SET Activo=1, Parentesco=d.Parentesco, EsResponsablePrinc=1, FechaAct=@Ahora
FROM AlumnoTutores at JOIN Alumnos a ON a.IdAlumno=at.IdAlumno
JOIN @Datos d ON d.DniAlumno=a.Dni
WHERE a.IdEscuela=@IdEscuela AND at.Activo=0;

COMMIT TRANSACTION;

SELECT COUNT(*) AS AlumnosCargados FROM Alumnos a JOIN @Datos d ON d.DniAlumno=a.Dni WHERE a.IdEscuela=@IdEscuela;
SELECT COUNT(*) AS TutoresCargados FROM Tutores t JOIN Usuarios u ON u.IdUsuario=t.IdUsuario JOIN @Datos d ON d.DniTutor=u.Dni WHERE u.IdEscuela=@IdEscuela;
SELECT COUNT(*) AS RelacionesCargadas FROM AlumnoTutores at JOIN Alumnos a ON a.IdAlumno=at.IdAlumno JOIN @Datos d ON d.DniAlumno=a.Dni WHERE a.IdEscuela=@IdEscuela AND at.Activo=1;
