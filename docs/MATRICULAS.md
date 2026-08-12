# Implementación de Matrículas

## Objetivo

Se reemplaza la relación técnica `AlumnoCurso` por la entidad de negocio `Matricula`.

Una matrícula representa la inscripción de un alumno a un curso dentro de un ciclo lectivo.

## Modelo

```text
Alumno
  |
  +-- Matricula -- Curso -- CicloLectivo
```

`Matricula` contiene:

- `IdMatricula`
- `IdEscuela`
- `IdAlumno`
- `IdCurso`
- `IdCicloLectivo`
- `FechaMatricula`
- `FechaBaja`
- `Estado` (`Activa`, `Baja`, `Finalizada`)
- `FechaCrea`
- `FechaAct`

La combinación `IdAlumno + IdCicloLectivo` es única. Esto garantiza que un alumno no tenga dos matrículas activas simultáneas en el mismo ciclo lectivo.

## Alumno

Se agrega:

```csharp
DateOnly FecNac
```

El valor se persiste como SQL Server `date` y es obligatorio.

## Asignación masiva

Endpoint:

```http
POST /api/Matriculas/asignar-masivo
```

Body:

```json
{
  "idCurso": 12,
  "idsAlumnos": [15, 18, 21, 27]
}
```

La operación se ejecuta dentro de una transacción y:

- crea matrículas nuevas;
- reactiva matrículas dadas de baja;
- informa alumnos que ya estaban matriculados en el curso;
- informa conflictos cuando el alumno ya está matriculado en otro curso del mismo ciclo;
- informa alumnos inexistentes, inactivos o pertenecientes a otra escuela.

## Listado para la pantalla de matriculación

Endpoint:

```http
GET /api/Matriculas/alumnos-disponibles
```

Parámetros:

```text
anioRegistro
idCicloLectivo
Pagina
Cantidad
nombre
apellido
dni
```

Los alumnos se ordenan en el servidor por:

1. Apellido
2. Nombre
3. DNI
4. Fecha de nacimiento

La consulta es paginada, por lo que el frontend no necesita cargar todos los alumnos de la escuela.

## Otros endpoints

```text
GET    /api/Matriculas/{id}
GET    /api/Matriculas/curso/{idCurso}
GET    /api/Matriculas/alumno/{idAlumno}
POST   /api/Matriculas
POST   /api/Matriculas/asignar-masivo
DELETE /api/Matriculas/{id}
```

## Base de datos durante desarrollo

Los datos de `AlumnoCurso` dejan de utilizarse. La migración `AddMatriculaAndFecNac` elimina la tabla `AlumnoCursos`, agrega `FecNac` y crea `Matriculas`.

Como el proyecto se encuentra en desarrollo y los datos no son necesarios, se recomienda recrear la base antes de aplicar la migración:

```powershell
dotnet ef database drop --force
dotnet ef database update
```

Después se pueden generar nuevamente los datos de desarrollo.
