# Limitaciones y diferencias observadas

[Índice](README.md)

Esta lista registra comportamientos concretos del código revisado el 28/09/2026. No significa que se hayan corregido en esta tarea documental.

| Área | Comportamiento observado | Implicación |
|---|---|---|
| Dashboard administrador | Dos tarjetas usan textos fijos: 94% y 2 alumnos | No presentarlas como estadísticas reales |
| Escuelas | Consulta/edición no se limita al IdEscuela del token | El alcance es diferente al de otros módulos |
| Consultas docentes | Algunos catálogos permiten consulta general dentro de la escuela | No afirmar que todo lo visible por API se limita a cursos asignados |
| Mis cursos | La consulta filtra asignación activa, sin año actual explícito | El subtítulo “este ciclo” no garantiza el filtrado |
| JWT | Expiración de respuesta fija en ocho horas; JWT configurable | Puede existir diferencia entre ambos valores |
| Sesión | Logout local, sin refresh/revocación central | No describirlo como invalidación inmediata del token en el servidor |
| Cuenta genérica | Cambiar rol no crea perfiles Docente/Tutor | Un rol y un perfil relacionado no son lo mismo |
| Alumno–tutor | Dos servicios para asociar; control de principal no idéntico | No garantizar unicidad funcional del principal en ambos caminos |
| Teléfonos | No se fuerza un único EsPrincipal | Varias marcas principales son posibles |
| Cursos | DELETE controla dependencias; PUT permite editar Activo sin el mismo control | La baja no tiene validación uniforme |
| Períodos | Nombres configurables, sin máximo de tres ni rechazo de solapamientos | Los trimestres deben configurarse; no se infieren |
| Asistencia | Calendario no laborable no equivale a prohibición de escritura | Puede existir registro en día clasificado no laborable |
| Asistencia | Feriados codificados, algunos específicos de 2026 | No se consulta automáticamente un calendario oficial actualizado |
| Notificaciones | Algunos errores de contador se convierten en cero | Cero no permite distinguir “sin pendientes” de error |
| Mensajes múltiples | Persistencia por destinatario sin transacción global | Un fallo puede producir envío parcial |
| Calificaciones | Servicio manual conservado, controller general solo expone GET | No hay una ruta pública nueva de carga manual por ese controller |
| Boletines | Se generan bajo demanda | No se actualizan automáticamente al editar evaluaciones |
| Migraciones | Historial antiguo falla desde base vacía por TelefonosContacto | La actualización probada no equivale a instalación limpia |
| Reglas escolares | No hay promoción, repitencia ni cierre automático implementados | No inferirlos de los conceptos del boletín |

## Referencias

Los documentos de cada módulo enlazan los servicios y pantallas correspondientes. La limitación del historial SQL y la prueba de actualización están detalladas en [Evaluaciones y períodos](Evaluaciones-y-periodos.md).

Una revisión documental no verifica datos reales, configuración de producción, aplicación de migraciones ni controles de seguridad en ejecución. Los permisos aquí descritos provienen de atributos y comprobaciones presentes en el código.
