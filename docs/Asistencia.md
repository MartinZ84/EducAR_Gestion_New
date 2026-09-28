# Asistencia

[Índice](README.md)

## Funcionalidades y roles

Administrador y Docente pueden consultar y registrar asistencia. El docente debe estar asignado al curso. El tutor consulta asistencias de alumnos vinculados mediante su portal.

React permite elegir curso y fecha, ver alumnos, marcar presente/ausente, marcar todos, guardar y consultar un calendario. Hay una única asistencia diaria por curso y división, independiente de la materia.

## Reglas de registro

- La clave única es alumno + curso + fecha. El servicio normaliza la fecha sin hora.
- El curso debe existir en la escuela y estar activo.
- El usuario docente debe tener perfil docente y asignación al curso.
- El administrador registra mediante un docente asignado al curso con usuario activo; si no existe, la API rechaza la operación. No hay un campo de autor administrador separado en Asistencia.
- Se debe enviar una vez a todos los alumnos devueltos por la matrícula del curso, sin IDs ajenos ni duplicados. No es una carga parcial de alumnos.
- Si ya existe asistencia se actualizan los registros y se agregan los faltantes. Los alumnos cargan presentes por defecto en la interfaz cuando aún no tienen registro.
- El resumen cuenta registros presentes/ausentes; no interpreta ausencia de registro como una falta del alumno.

## Estado del día y notificaciones

El dashboard docente considera tomada la asistencia de un curso cuando hay al menos un registro de un alumno, presente o ausente. Si falta en algún curso muestra pendiente en rojo; si todos tienen registros, tomada en verde. Una consulta fallida no se convierte en “tomada”.

En la página Asistencia aparecen arriba de los selectores los cursos pendientes en rojo y los tomados en verde para la fecha elegida. Se recalculan al cambiar fecha y después de guardar. Un docente con varias materias del mismo curso no genera varios estados.

## Calendario y pendientes históricos

El calendario recorre inicio/fin del ciclo. Clasifica días en NoLaborable, Cargada, Pendiente y Futura. Evalúa fines de semana/feriados antes de registros existentes. Los feriados están codificados en AsistenciaController; hay fechas turísticas específicas de 2026.

El contador superior suma días pendientes de los cursos activos del ciclo activo del año actual. No es el mismo indicador que la tarjeta de asistencia de hoy.

El registro no tiene una validación equivalente que prohíba guardar fines de semana, feriados o fechas futuras. El calendario es clasificación, no una regla universal de bloqueo.

## API y fuentes

/api/Asistencia: POST; /curso/{idCurso}?fecha=...; /alumno/{idAlumno}/curso/{idCurso}; /calendario/{idCurso}; /pendientes: GET.

[Controller](../EducAR.API/Controllers/AsistenciaController.cs), [servicio](../EducAR.API/Services/AsistenciaService.cs), [pantalla](../../EducAr_Web/src/pages/Docente/Asistencia/AsistenciaPage.tsx), [estado por curso](../../EducAr_Web/src/hooks/useEstadoAsistencia.ts), [notificación histórica](../../EducAr_Web/src/components/AsistenciaNotification.tsx).
