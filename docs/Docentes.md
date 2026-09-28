# Docentes

[Índice](README.md)

## Funcionalidades y roles

Administrador: listado paginado con filtros, alta, edición, detalle y baja. Docente: la API permite consulta por ID y detalle, pero no el listado general. La pantalla de administración está en /admin/docentes.

El detalle muestra identidad, contacto, estado y cursos/materias asignados que cumplen los filtros activos del servicio.

## Reglas

- Un docente tiene un perfil Docente y una cuenta Usuario; los datos personales y credenciales se guardan en Usuario.
- Al crear se validan nombre de usuario, DNI y email en la escuela. La cuenta se crea con el rol docente y contraseña hasheada.
- El alta de cuenta y perfil utiliza una transacción. El código contempla reutilización de una cuenta/perfil inactivos cuando corresponde.
- Editar valida DNI y email, actualiza nombre/apellido/contacto y sincroniza el estado de cuenta y perfil.
- Dar de baja marca tanto Docente.Activo como Usuario.Activo en falso. No borra físicamente sus notas, asistencias ni asignaciones históricas.
- Crear el docente no lo asigna automáticamente a materias ni cursos. Esa operación está en [Asignaciones](Asignaciones.md).
- IdUsuario e IdDocente son identificadores diferentes.

## API y verificación funcional

/api/Docentes: GET/POST; /{id}: GET/PUT/DELETE; /{id}/detalle: GET. Para probar el flujo, crear docente, asignar curso/materia, iniciar sesión y revisar Mis cursos. Intentar un DNI/email ya utilizado debe devolver error.

## Fuentes

[Servicio](../EducAR.API/Services/DocenteService.cs), [repositorio](../EducAR.API/Repositories/DocenteRepository.cs), [controller](../EducAR.API/Controllers/DocentesController.cs), [pantalla](../../EducAr_Web/src/pages/Admin/Docentes/DocentesPage.tsx).
