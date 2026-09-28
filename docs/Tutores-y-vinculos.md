# Tutores y vínculos con alumnos

[Índice](README.md)

## Funcionalidades y roles

Administrador: alta, edición, baja de tutores y asociación/desasociación con alumnos. Administrador y Docente: consultas generales, detalle y relaciones por API. Tutor: consulta de sus alumnos asociados y acceso al portal.

La cuenta Usuario mantiene identidad, DNI, email, credenciales y estado. Tutor mantiene su perfil; AlumnoTutor relaciona cada alumno con un tutor, parentesco y condición de responsable principal.

## Reglas

- Alta: validación de nombre de usuario, DNI y email en la escuela, contraseña hasheada y cuenta de rol Tutor.
- La baja del tutor desactiva su usuario. Sus vínculos no se borran físicamente en ese procedimiento.
- Alumno y tutor deben pertenecer a la escuela de la operación.
- El índice único alumno/tutor impide duplicar el mismo vínculo. Una asociación inactiva se reutiliza.
- El servicio AlumnoTutorService impide crear otro responsable principal si ya existe uno.
- Los dos flujos de desasociación revisados exigen conservar al menos un tutor activo.
- EsResponsable del perfil Tutor y EsResponsablePrinc del vínculo no son el mismo campo: el segundo es específico de un alumno.

## Diferencia entre endpoints de asociación

Existen /api/AlumnoTutor y /api/Alumnos/{id}/tutores. No se deben documentar como si aplicaran exactamente las mismas reglas: la validación explícita de otro responsable principal está en AlumnoTutorService. El alta desde AlumnoService comprueba relación/escuela y reactivación, pero no replica esa comprobación.

## API y fuentes

/api/Tutores, /api/AlumnoTutor/alumno/{idAlumno}, /tutor/{idTutor}, /mis-alumnos; POST y DELETE de vínculos según [inventario](API-endpoints.md).

[TutorService](../EducAR.API/Services/TutorService.cs), [TutorRepository](../EducAR.API/Repositories/TutorRepository.cs), [AlumnoTutorService](../EducAR.API/Services/AlumnoTutorService.cs), [AlumnoService](../EducAR.API/Services/AlumnoService.cs), [pantalla](../../EducAr_Web/src/pages/Admin/Tutores/TutoresPage.tsx).
