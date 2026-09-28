# Autenticación y usuarios

[Índice](README.md)

## Funcionalidades y roles

Login con nombre de usuario, contraseña y escuela. El administrador dispone de listado paginado, búsqueda, alta, edición, detalle y baja de usuarios. Todo usuario autenticado puede consultar su perfil y cambiar su propia contraseña por API. El catálogo de roles es de consulta; no hay CRUD de roles expuesto.

En React, AuthContext conserva usuario y token en localStorage. Axios agrega Bearer a las peticiones. Ante 401 elimina la sesión y redirige a login. Cerrar sesión limpia los datos locales; no existe un endpoint de revocación de token.

## Reglas

- El login busca una cuenta activa de la escuela indicada y valida el hash con BCrypt.
- Un login correcto actualiza UltimoAcceso y FechaAct.
- El JWT incluye IdUsuario, nombre de usuario, rol, escuela y nombre completo. Su vencimiento se calcula con Jwt:ExpirationHours.
- Al crear usuarios, la escuela se toma del administrador autenticado, no del valor enviado por el navegador.
- El servicio valida duplicados de nombre de usuario, DNI y email dentro de la escuela al crear. Al editar valida DNI y email.
- La baja es lógica.
- Cambiar contraseña requiere contraseña actual correcta, nueva y confirmación iguales, y al menos seis caracteres para la nueva.
- La modificación del rol de una cuenta genérica no crea automáticamente el perfil Docente o Tutor: esos perfiles tienen sus propios flujos.

## API

Familias /api/Auth/login, /api/Usuarios y /api/Roles. Usuarios agrega /perfil y PATCH /cambiar-contrasena. [Métodos y permisos exactos](API-endpoints.md).

## Límites actuales

El campo Expiracion devuelto por AuthService usa ocho horas, mientras el vencimiento del JWT usa configuración. Pueden diferir. No se encontró refresh token, recuperación de contraseña por email ni revocación central al cerrar sesión. La cuenta inactiva no puede iniciar otra sesión, pero no hay una comprobación global por petición que revoque automáticamente todos sus JWT ya emitidos.

Perfil y cambio de contraseña existen en API; no tienen una página dedicada en los menús revisados.

## Fuentes

[AuthService](../EducAR.API/Services/AuthService.cs), [AuthRepository](../EducAR.API/Repositories/AuthRepository.cs), [UsuarioService](../EducAR.API/Services/UsuarioService.cs), [UsuariosController](../EducAR.API/Controllers/UsuariosController.cs), [JWT](../EducAR.API/Helpers/JwtHelper.cs), [Usuarios React](../../EducAr_Web/src/pages/Admin/Usuarios/UsuariosPage.tsx).
