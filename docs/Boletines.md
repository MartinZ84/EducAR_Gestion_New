# Boletines

[Índice](README.md)

## Funcionalidades y roles

Administrador y Docente pueden consultar, generar boletines y modificar observaciones. El docente debe tener una asignación activa al curso. La pantalla está disponible desde los menús de ambos roles.

La pantalla permite elegir curso/trimestre, consultar los promedios actuales, generar un boletín por alumno y abrir su PDF. La tabla, el documento guardado y el PDF usan el mismo DTO calculado por la API.

## Reglas de generación

- El curso debe pertenecer a la escuela y el período configurado al ciclo lectivo del curso.
- Se incluyen todos los alumnos y alumnas activos con matrícula activa en ese curso/ciclo, también si todavía no tienen notas.
- Las materias provienen de las asignaciones activas `DocenteMateriaCurso` del curso.
- El promedio de una materia usa las notas existentes en `NotasEvaluacion` de evaluaciones activas del mismo alumno, curso, materia y período. Una evaluación sin fila de nota no aporta cero ni entra en el promedio.
- Cada promedio por materia se redondea a dos decimales con `MidpointRounding.AwayFromZero`, regla ya usada al actualizar las calificaciones derivadas de evaluaciones.
- Una materia sin notas se conserva con calificación y concepto nulos. Se excluye del promedio general; si ninguna materia tiene nota, el promedio general también es nulo.
- El promedio general conserva la regla anterior: media aritmética de las materias con nota, redondeada a dos decimales con `AwayFromZero`.
- Los conceptos se asignan a partir del promedio por materia ya redondeado: menor que 7 Insuficiente; desde 7 y menor que 8 Bueno; desde 8 y menor que 9 Muy bueno; desde 9 y menor que 9,5 Excelente; desde 9,5 hasta 10 Sobresaliente.
- El índice único existente por alumno/período evita duplicados. El período pertenece a un solo ciclo; la matrícula impide dos cursos activos por alumno/ciclo.
- Al regenerar, se recalculan y reemplazan los detalles del mismo boletín. Se conserva su identificador, fecha de creación y observación general; se actualiza `FechaAct`.
- La observación general se modifica separadamente. Guardar una observación no elimina los detalles.
- El GET calcula una vista previa actual y marca `RequiereRegeneracion` si difiere de la última versión persistida. El POST individual guarda exactamente ese cálculo y devuelve el DTO usado para abrir el PDF.

## Conceptos

| Nota | Concepto |
|---|---|
| Menor que 7 | Insuficiente |
| Desde 7 y menor que 8 | Bueno |
| Desde 8 y menor que 9 | Muy bueno |
| Desde 9 y menor que 9,5 | Excelente |
| Desde 9,5 hasta 10 | Sobresaliente |

Estas etiquetas no implementan promoción, repitencia ni cierre de trimestre. El sistema no tiene una regla de cierre que congele notas.

## Decisiones académicas pendientes

- No hay ponderaciones por evaluación definidas; se usa media aritmética simple.
- No hay un estado de cierre trimestral. Las evaluaciones pueden modificarse según los permisos actuales y el boletín debe regenerarse para actualizar su copia guardada.
- Las materias se determinan por asignaciones activas actuales. No existe historial temporal de asignaciones que permita reconstruir una materia que se desactivó después del trimestre.
- Se reutilizan períodos configurables como trimestres; el modelo no exige exactamente tres ni impide repetir nombres en un mismo ciclo.

## API y fuentes

`GET /api/Boletines/curso/{idCurso}/periodo/{idPeriodo}` calcula la tabla. `POST /api/Boletines/alumno/{idAlumno}/curso/{idCurso}/periodo/{idPeriodo}/generar` persiste o regenera un boletín y devuelve su DTO. Se conserva `POST /api/Boletines/generar` para generación masiva. `PATCH /api/Boletines/{id}/observacion` modifica la observación.

[Controller](../EducAR.API/Controllers/BoletinesController.cs), [servicio](../EducAR.API/Services/BoletinService.cs), [repositorio](../EducAR.API/Repositories/BoletinRepository.cs), [pantalla](../../EducAr_Web/src/pages/Docente/Boletines/BoletinesPage.tsx), [PDF](../../EducAr_Web/src/utils/boletinPdf.ts), [notas y promedios](Evaluaciones-y-periodos.md).
