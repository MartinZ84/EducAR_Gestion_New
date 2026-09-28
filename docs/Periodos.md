# Períodos de evaluación

[Índice](README.md)

## Funcionalidades y roles

Administrador: crear, consultar, editar, dar de baja y reactivar desde /admin/periodos. Docente: consultar desde /docente/periodos, sin acciones de escritura. La API exige Administrador para POST, PUT y DELETE, incluso si se invoca sin usar la pantalla.

## Reglas

- Se reutiliza PeriodoEvaluacion, ligado a un CicloLectivo.
- Nombre obligatorio de hasta 100 caracteres; inicio, fin y estado.
- El ciclo debe pertenecer a la escuela. Para crear debe estar activo.
- Fin posterior al inicio; ambas fechas dentro del ciclo.
- No se permite repetir el nombre en el mismo ciclo según la comprobación del servicio.
- Los nombres son configurables. React sugiere Primer, Segundo y Tercer trimestre. No se crea automáticamente esa terna ni se impone un máximo de tres.
- No hay validación explícita de solapamientos entre períodos distintos.
- Dar de baja usa Activo=false; reactivar usa PUT con Activo=true.
- Se bloquea la baja si hay evaluaciones activas, calificaciones o boletines asociados. La misma comprobación se aplica a PUT cuando desactiva.
- No se permite modificar las fechas dejando evaluaciones activas fuera del período.
- Crear y editar no tienen idéntico control de estado del ciclo: la comprobación explícita de ciclo activo está en el alta.
- No hay un cierre automático por haber llegado a FechaFin. Fecha de evaluación y estado activo son controles separados.

## Flujo

Elegir ciclo, crear el período con sus fechas, editarlo cuando corresponda y seleccionarlo al crear evaluaciones. Si no hay períodos activos, Calificaciones informa que debe revisarse la configuración.

## API y fuentes

GET/POST /api/ciclos/{idCicloLectivo}/periodos; GET/PUT/DELETE /{id} dentro de esa ruta.

[Controller](../EducAR.API/Controllers/PeriodosEvaluacionController.cs), [servicio](../EducAR.API/Services/PeriodoEvaluacionService.cs), [repositorio](../EducAR.API/Repositories/PeriodoEvaluacionRepository.cs), [pantalla](../../EducAr_Web/src/pages/Periodos/PeriodosPage.tsx), [relación con evaluaciones](Evaluaciones-y-periodos.md).
