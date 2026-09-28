# Paneles, navegación y notificaciones

[Índice](README.md)

## Administrador

Menú: Dashboard, Docentes, Tutores, Alumnos, Materias, Cursos, Ciclos Lectivos, Usuarios, Asignaciones, Matrículas, Asistencia, Evaluaciones y notas, Períodos.

El dashboard carga cantidades de docentes, alumnos, tutores y cursos desde el total de los listados paginados. Las tarjetas “94% promedio hoy” y “2 alumnos sin calificar” son textos fijos en el código actual: no representan métricas calculadas ni una alerta real.

Dispone de notificación de pendientes de asistencia con acceso al calendario.

## Docente

Menú: Dashboard, Mis Cursos, Asistencia, Calificaciones, Períodos, Boletines y Mensajes.

Dashboard: cantidad de cursos distintos, asistencia del día, acceso a notas y mensajes no leídos. Las tarjetas de cursos usan el mismo componente que Mis cursos, con materias desplegables. La asistencia se abre por curso; las notas por curso/materia.

La tarjeta del día es roja si hay cursos pendientes y verde cuando todos tienen registros. También contempla carga, ausencia de cursos y error de consulta. Es diferente del contador de días pendientes históricos de la barra superior.

## Tutor

Menú: Asistencia, Calificaciones y Mensajes. El dashboard de gestión docente/administrador no se reutiliza para el tutor. La ruta inicial corresponde a asistencia.

## Comportamiento compartido

Los layouts tienen menú lateral adaptable a móvil, identificación del usuario y cierre de sesión. Las pantallas muestran errores de API mediante avisos. Las rutas visibles se organizan por rol, pero la protección efectiva depende además de la API.

El contador de asistencia se actualiza al cambiar de ruta. El de mensajes se consulta periódicamente y por eventos locales. Los errores del contador superior se traducen actualmente en cero; eso no garantiza que no haya pendientes.

## Fuentes

[AdminLayout](../../EducAr_Web/src/pages/Admin/AdminLayout.tsx), [dashboard administrador](../../EducAr_Web/src/pages/Admin/Dashboard/AdminDashboard.tsx), [DocenteLayout](../../EducAr_Web/src/pages/Docente/DocenteLayout.tsx), [dashboard docente](../../EducAr_Web/src/pages/Docente/Dashboard/DocenteDashboard.tsx), [TutorLayout](../../EducAr_Web/src/pages/Tutor/TutorLayout.tsx), [AsistenciaNotification](../../EducAr_Web/src/components/AsistenciaNotification.tsx), [MensajesNotification](../../EducAr_Web/src/components/MensajesNotification.tsx).
