# Portal del tutor

[Índice](README.md)

## Funcionalidades

Menú propio con Asistencia, Calificaciones y Mensajes. La entrada inicial abre la consulta de asistencia. El tutor no tiene acceso al CRUD general de alumnos, a la carga de asistencia ni a la edición de notas.

Asistencia muestra inicialmente los últimos siete días de todos los alumnos vinculados y permite filtrar. Calificaciones permite elegir alumno y consultar calificaciones del período y notas individuales de evaluaciones.

## Reglas de consulta

- Los endpoints de /api/tutor exigen rol Tutor.
- Los alumnos disponibles provienen de relaciones AlumnoTutor activas asociadas al IdUsuario autenticado.
- Se exige usuario tutor activo y coherencia de escuela del tutor y del alumno.
- Consultar un alumno sin vínculo devuelve NotFound; no basta conocer su ID.
- Las asistencias consultadas deben estar activas y pertenecer a cursos de la escuela.
- Las calificaciones de período deben estar activas y pertenecer a materias de la escuela.
- Las notas individuales corresponden a evaluaciones activas de cursos de la escuela.
- Estas consultas devuelven historial; no tienen el filtro estricto de año actual usado para destinatarios de mensajes.
- Las notas pendientes no son ceros: el endpoint de notas devuelve registros existentes.
- No existe en el portal actual una descarga o consulta específica de boletines completos.

## API y fuentes

/api/tutor/mis-alumnos; /alumnos/{idAlumno}/asistencias; /alumnos/{idAlumno}/calificaciones; /alumnos/{idAlumno}/notas: GET.

También existe /api/AlumnoTutor/mis-alumnos en el controlador de relaciones; no confundir ambos contratos.

[Controller del portal](../EducAR.API/Controllers/TutorConsultasController.cs), [consulta React](../../EducAr_Web/src/pages/Tutor/ConsultaAlumnoPage.tsx), [menú](../../EducAr_Web/src/pages/Tutor/TutorLayout.tsx), [mensajería](Mensajeria.md).
