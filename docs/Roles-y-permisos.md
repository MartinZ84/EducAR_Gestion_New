# Roles y permisos

[Índice](README.md)

## Roles implementados

El JWT lleva un rol: Administrador, Docente o Tutor. No hay un rol Alumno ni un Superadministrador diferenciado en los controladores revisados. Cada usuario está asociado a una escuela.

| Funcionalidad | Administrador | Docente | Tutor |
|---|---|---|---|
| Perfil propio y cambio de contraseña | Sí | Sí | Sí |
| Usuarios: listado, detalle, alta, edición y baja | Sí | No | No |
| Catálogo de roles | Consulta | Consulta | Consulta |
| Escuelas | Consulta, alta y edición por API | Consulta por API | Consulta por API |
| Docentes | Listado, detalle y administración | Consulta por ID y detalle; no listado general | No |
| Alumnos y sus teléfonos | Consulta y administración | Consulta | Solo información vinculada mediante portal tutor |
| Tutores | Consulta y administración | Consulta general, por ID y detalle | Sin acceso al CRUD |
| Ciclos, cursos y materias | Consulta y administración | Consulta | No |
| Períodos | Alta, consulta, edición, baja y reactivación | Consulta | No |
| Matrículas | Consulta, alta individual/masiva y baja | Consulta | No |
| Asignación docente–materia–curso | Consulta, asignación y baja | Consulta; Mis cursos propios | No |
| Vínculos alumno–tutor | Consulta, asociación y baja | Consulta | Consulta de sus alumnos |
| Evaluaciones y notas | Administra combinaciones habilitadas en su escuela | Administra sus asignaciones | Consulta sus alumnos mediante portal |
| Calificaciones del período: controlador general | Consulta | No; utiliza evaluaciones | No; utiliza portal |
| Asistencia | Consulta y registro | Consulta y registro de cursos asignados | Consulta vinculada |
| Boletines | Consulta, generación y observación por API | Consulta, generación y observación de cursos asignados | Sin endpoint de boletines en el portal actual |
| Mensajes: enviar y buscar destinatarios | No | Sí, tutores vinculados | Sí, docentes vinculados |
| Mensajes propios: bandejas, detalle, contador y marcar leído | Autenticado, aunque no hay menú | Sí | Sí |

## Alcance de los permisos

- El servidor obtiene normalmente IdEscuela e identidad desde el JWT.
- Las consultas generales de alumnos, tutores, cursos, materias y ciclos habilitadas al docente no equivalen a consultas limitadas a sus asignaciones. La restricción específica existe en operaciones como evaluaciones, asistencia y boletines.
- Un administrador no necesita ser docente para administrar evaluaciones, pero los pares curso/materia disponibles provienen de asignaciones activas.
- El tutor no elige libremente cualquier alumno: se exige relación activa con su usuario y coherencia de escuela en el portal.
- Escuelas es una excepción al alcance habitual: sus endpoints no restringen los IDs al IdEscuela del token. Esto describe el código actual, no una política institucional recomendada.
- Las acciones de escritura de períodos están protegidas por Administrador en el servidor, además de ocultarse al docente en React.
- Ocultar botones no reemplaza las validaciones de la API.

## Fuentes

[Controladores](../EducAR.API/Controllers), [JWT](../EducAR.API/Helpers/JwtHelper.cs), [contexto React](../../EducAr_Web/src/context/AuthContext.tsx), [inventario por endpoint](API-endpoints.md).
