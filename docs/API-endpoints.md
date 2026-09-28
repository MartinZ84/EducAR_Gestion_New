# Inventario de endpoints

[Indice](README.md)

Extraido de los atributos activos de los controladores el 28/09/2026. Las rutas se muestran con el prefijo /api. Los parametros de query y los DTO completos deben consultarse en el metodo enlazado.

La columna de acceso resume Authorize. Las comprobaciones adicionales de escuela, asignacion, matricula y vinculos se describen en las fichas de cada modulo. Autenticado significa que no hay una restriccion de rol declarada para esa accion.

## AlumnoTutor

[Controlador](../EducAR.API/Controllers/AlumnoTutorController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/AlumnoTutor/alumno/{idAlumno} | Administrador, Docente | ObtenerPorAlumno |
| GET | /api/AlumnoTutor/tutor/{idTutor} | Administrador, Docente | ObtenerPorTutor |
| POST | /api/AlumnoTutor | Administrador | Asociar |
| DELETE | /api/AlumnoTutor/{id} | Administrador | Desasociar |
| GET | /api/AlumnoTutor/mis-alumnos | Tutor | ObtenerMisAlumnos |

## Alumnos

[Controlador](../EducAR.API/Controllers/AlumnosController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Alumnos | Administrador, Docente | ObtenerTodos |
| GET | /api/Alumnos/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/Alumnos/{id}/detalle | Administrador, Docente | ObtenerDetalle |
| POST | /api/Alumnos | Administrador | Crear |
| PUT | /api/Alumnos/{id} | Administrador | Actualizar |
| DELETE | /api/Alumnos/{id} | Administrador | Eliminar |
| POST | /api/Alumnos/{id}/tutores | Administrador | AsociarTutor |
| DELETE | /api/Alumnos/{id}/tutores/{idTutor} | Administrador | QuitarTutor |

## Asistencia

[Controlador](../EducAR.API/Controllers/AsistenciaController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Asistencia/calendario/{idCurso} | Administrador, Docente | ObtenerCalendario |
| GET | /api/Asistencia/pendientes | Administrador, Docente | ObtenerPendientes |
| GET | /api/Asistencia/curso/{idCurso} | Administrador, Docente | ObtenerPorCursoYFecha |
| GET | /api/Asistencia/alumno/{idAlumno}/curso/{idCurso} | Administrador, Docente | ObtenerResumenAlumno |
| POST | /api/Asistencia | Administrador, Docente | Registrar |

## Auth

[Controlador](../EducAR.API/Controllers/AuthController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| POST | /api/Auth/login | Sin autenticacion | Login |

## Boletines

[Controlador](../EducAR.API/Controllers/BoletinesController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Boletines/curso/{idCurso}/periodo/{idPeriodo} | Administrador, Docente | ObtenerPorCursoYPeriodo |
| GET | /api/Boletines/alumno/{idAlumno}/curso/{idCurso}/periodo/{idPeriodo} | Administrador, Docente | ObtenerPorAlumno |
| POST | /api/Boletines/generar | Administrador, Docente | Generar |
| PATCH | /api/Boletines/{id}/observacion | Administrador, Docente | ActualizarObservacion |

## Calificaciones

[Controlador](../EducAR.API/Controllers/CalificacionesController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Calificaciones/curso/{idCurso}/materia/{idMateria}/periodo/{idPeriodo} | Administrador | ObtenerPorCursoMateriaYPeriodo |
| GET | /api/Calificaciones/alumno/{idAlumno}/periodo/{idPeriodo} | Administrador | ObtenerPorAlumno |

## CiclosLectivos

[Controlador](../EducAR.API/Controllers/CiclosLectivosController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/CiclosLectivos | Administrador, Docente | ObtenerTodos |
| GET | /api/CiclosLectivos/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/CiclosLectivos/{id}/detalle | Administrador, Docente | ObtenerDetalle |
| POST | /api/CiclosLectivos | Administrador | Crear |
| PUT | /api/CiclosLectivos/{id} | Administrador | Actualizar |
| DELETE | /api/CiclosLectivos/{id} | Administrador | Eliminar |

## Cursos

[Controlador](../EducAR.API/Controllers/CursosController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Cursos | Administrador, Docente | ObtenerTodos |
| GET | /api/Cursos/ciclo/{idCicloLectivo} | Administrador, Docente | ObtenerPorCicloLectivo |
| GET | /api/Cursos/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/Cursos/{id}/detalle | Administrador, Docente | ObtenerDetalle |
| POST | /api/Cursos | Administrador | Crear |
| PUT | /api/Cursos/{id} | Administrador | Actualizar |
| DELETE | /api/Cursos/{id} | Administrador | Eliminar |

## DocenteMateriaCurso

[Controlador](../EducAR.API/Controllers/DocenteMateriaCursoController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/DocenteMateriaCurso/docente/{idDocente} | Administrador, Docente | ObtenerPorDocente |
| GET | /api/DocenteMateriaCurso/curso/{idCurso} | Administrador, Docente | ObtenerPorCurso |
| GET | /api/DocenteMateriaCurso/curso/{idCurso}/materia/{idMateria} | Administrador, Docente | ObtenerPorCursoYMateria |
| POST | /api/DocenteMateriaCurso | Administrador | Asignar |
| DELETE | /api/DocenteMateriaCurso/{id} | Administrador | Desasignar |
| GET | /api/DocenteMateriaCurso/mis-cursos | Docente | ObtenerMisCursos |

## Docentes

[Controlador](../EducAR.API/Controllers/DocentesController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Docentes | Administrador | ObtenerTodos |
| GET | /api/Docentes/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/Docentes/{id}/detalle | Administrador, Docente | ObtenerDetalle |
| POST | /api/Docentes | Administrador | Crear |
| PUT | /api/Docentes/{id} | Administrador | Actualizar |
| DELETE | /api/Docentes/{id} | Administrador | Eliminar |

## Escuelas

[Controlador](../EducAR.API/Controllers/EscuelasController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Escuelas | Autenticado | ObtenerTodas |
| GET | /api/Escuelas/{id} | Autenticado | ObtenerPorId |
| POST | /api/Escuelas | Administrador | Crear |
| PUT | /api/Escuelas/{id} | Administrador | Actualizar |

## Evaluaciones

[Controlador](../EducAR.API/Controllers/EvaluacionesController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/evaluaciones/opciones | Administrador, Docente | Opciones |
| GET | /api/evaluaciones/curso/{idCurso}/materia/{idMateria}/periodo/{idPeriodo} | Administrador, Docente | Obtener |
| POST | /api/evaluaciones | Administrador, Docente | Crear |
| PUT | /api/evaluaciones/{idEvaluacion} | Administrador, Docente | Editar |
| DELETE | /api/evaluaciones/{idEvaluacion} | Administrador, Docente | Archivar |
| GET | /api/evaluaciones/{idEvaluacion}/alumnos | Administrador, Docente | ObtenerAlumnos |
| PUT | /api/evaluaciones/{idEvaluacion}/notas | Administrador, Docente | GuardarNotas |

## Materias

[Controlador](../EducAR.API/Controllers/MateriasController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Materias | Administrador, Docente | ObtenerTodas |
| GET | /api/Materias/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/Materias/{id}/detalle | Administrador, Docente | ObtenerDetalle |
| POST | /api/Materias | Administrador | Crear |
| PUT | /api/Materias/{id} | Administrador | Actualizar |
| DELETE | /api/Materias/{id} | Administrador | Eliminar |

## Matriculas

[Controlador](../EducAR.API/Controllers/MatriculasController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Matriculas/curso/{idCurso} | Administrador, Docente | ObtenerPorCurso |
| GET | /api/Matriculas/alumno/{idAlumno} | Administrador, Docente | ObtenerPorAlumno |
| GET | /api/Matriculas/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/Matriculas/alumnos-disponibles | Administrador, Docente | ObtenerAlumnosDisponibles |
| POST | /api/Matriculas | Administrador | Crear |
| POST | /api/Matriculas/asignar-masivo | Administrador | AsignarMasivo |
| DELETE | /api/Matriculas/{id} | Administrador | DarDeBaja |

## Mensajes

[Controlador](../EducAR.API/Controllers/MensajesController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Mensajes/recibidos | Autenticado | ObtenerRecibidos |
| GET | /api/Mensajes/enviados | Autenticado | ObtenerEnviados |
| GET | /api/Mensajes/noleidos | Autenticado | ContarNoLeidos |
| GET | /api/Mensajes/destinatarios | Docente, Tutor | ObtenerDestinatarios |
| GET | /api/Mensajes/{id} | Autenticado | ObtenerPorId |
| POST | /api/Mensajes | Docente, Tutor | Enviar |
| PATCH | /api/Mensajes/{id}/leido | Autenticado | MarcarLeido |

## PeriodosEvaluacion

[Controlador](../EducAR.API/Controllers/PeriodosEvaluacionController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/ciclos/{idCicloLectivo}/periodos | Administrador, Docente | ObtenerTodos |
| GET | /api/ciclos/{idCicloLectivo}/periodos/{id} | Administrador, Docente | ObtenerPorId |
| POST | /api/ciclos/{idCicloLectivo}/periodos | Administrador | Crear |
| PUT | /api/ciclos/{idCicloLectivo}/periodos/{id} | Administrador | Actualizar |
| DELETE | /api/ciclos/{idCicloLectivo}/periodos/{id} | Administrador | Eliminar |

## Roles

[Controlador](../EducAR.API/Controllers/RolesController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Roles | Autenticado | ObtenerTodos |

## Telefonos

[Controlador](../EducAR.API/Controllers/TelefonosController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Telefonos/alumno/{idAlumno} | Administrador, Docente | ObtenerPorAlumno |
| POST | /api/Telefonos | Administrador | Crear |
| DELETE | /api/Telefonos/{id} | Administrador | Eliminar |
| PUT | /api/Telefonos/{id} | Administrador | Actualizar |

## TutorConsultas

[Controlador](../EducAR.API/Controllers/TutorConsultasController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/tutor/mis-alumnos | Tutor | ObtenerMisAlumnos |
| GET | /api/tutor/alumnos/{idAlumno}/asistencias | Tutor | ObtenerAsistencias |
| GET | /api/tutor/alumnos/{idAlumno}/calificaciones | Tutor | ObtenerCalificaciones |
| GET | /api/tutor/alumnos/{idAlumno}/notas | Tutor | ObtenerNotas |

## Tutores

[Controlador](../EducAR.API/Controllers/TutoresController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Tutores | Administrador, Docente | ObtenerTodos |
| GET | /api/Tutores/{id} | Administrador, Docente | ObtenerPorId |
| GET | /api/Tutores/{id}/detalle | Administrador, Docente | ObtenerDetalle |
| POST | /api/Tutores | Administrador | Crear |
| PUT | /api/Tutores/{id} | Administrador | Actualizar |
| DELETE | /api/Tutores/{id} | Administrador | Eliminar |

## Usuarios

[Controlador](../EducAR.API/Controllers/UsuariosController.cs)

| Metodo | Ruta | Acceso declarado | Accion |
|---|---|---|---|
| GET | /api/Usuarios | Administrador | ObtenerTodos |
| GET | /api/Usuarios/{id} | Administrador | ObtenerPorId |
| GET | /api/Usuarios/{id}/detalle | Administrador | ObtenerDetalle |
| POST | /api/Usuarios | Administrador | Crear |
| PUT | /api/Usuarios/{id} | Administrador | Actualizar |
| DELETE | /api/Usuarios/{id} | Administrador | Eliminar |
| PATCH | /api/Usuarios/cambiar-contrasena | Autenticado | CambiarContrasena |
| GET | /api/Usuarios/perfil | Autenticado | ObtenerPerfil |

Total: 109 acciones HTTP en 21 controladores.
