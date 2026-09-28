# Alumnos y contactos

[Índice](README.md)

## Funcionalidades y roles

Administrador: alta, edición, baja, búsqueda paginada, ficha detallada, asociación de tutores y gestión de teléfonos. Docente: consultas de alumnos y teléfonos por API. Tutor: consultas académicas de sus alumnos por endpoints propios, no el CRUD general.

La ficha React organiza datos, matrícula, tutores, asistencias, calificaciones y boletines. La pertenencia al curso se administra por matrícula, no editando directamente un curso en el alumno.

## Reglas

- El alumno pertenece a una escuela.
- La fecha de nacimiento no puede ser vacía ni futura. El nombre de la propiedad actual es FechaNacimiento; algunos DTO de matrícula la exponen como FecNac.
- El DNI no se puede repetir para otro alumno de la escuela según la validación del servicio.
- La baja cambia Activo; no elimina físicamente el historial.
- Asociar un tutor exige que exista en la escuela. Una relación inactiva puede reactivarse.
- Quitar un tutor no está permitido si dejaría al alumno sin ninguno. Esto no implica que exista una restricción SQL que obligue a tener tutor en el instante inicial del alta.
- Los flujos de asociación tienen diferencias sobre responsable principal; ver [Tutores y vínculos](Tutores-y-vinculos.md).

## Teléfonos

Se registran número, descripción, tipo y marca de principal. Consulta para Administrador/Docente; creación, edición y eliminación solo Administrador.

El servicio comprueba la escuela del alumno. La edición no permite mover un teléfono a otro alumno por modificar el ID del body. La eliminación es física. El código permite marcar EsPrincipal, pero no se encontró una validación que garantice un único teléfono principal por alumno.

## API y fuentes

/api/Alumnos y /api/Telefonos; los vínculos también tienen rutas /api/Alumnos/{id}/tutores y /api/AlumnoTutor. [Inventario](API-endpoints.md).

[AlumnoService](../EducAR.API/Services/AlumnoService.cs), [AlumnoRepository](../EducAR.API/Repositories/AlumnoRepository.cs), [TelefonoService](../EducAR.API/Services/TelefonoService.cs), [ficha React](../../EducAr_Web/src/components/modals/AlumnoDetalleModal.tsx), [pantalla](../../EducAr_Web/src/pages/Admin/Alumnos/AlumnosPage.tsx).
