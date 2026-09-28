# Evaluaciones, calificaciones y períodos

[Índice de documentación](README.md) · [Roles y permisos](Roles-y-permisos.md) · [Administración de períodos](Periodos.md)

## Modelo revisado y decisiones

- Se reutilizan `Evaluacion`, `NotaEvaluacion`, `PeriodoEvaluacion`, `Curso`, `CicloLectivo`, `DocenteMateriaCurso` y `Matricula`. No se crean entidades paralelas.
- El año se obtiene de `Curso.IdCicloLectivo`, y el período debe pertenecer al mismo ciclo. La pantalla permite elegir el año. No se agrega un año independiente que pueda contradecir al curso.
- Cada evaluación tiene título (200 caracteres), temario y descripción (4000 cada uno), fecha, curso, materia y período. Curso, materia y período quedan fijos después de crearla; se pueden editar los textos y la fecha.
- La entidad de períodos ya existía, con nombre y fechas configurables. El administrador configura Primer, Segundo y Tercer trimestre en esa entidad; no se infieren trimestres a partir de fechas ni se cambian nombres históricos. No se impone un máximo de tres registros porque la configuración anterior no tenía esa restricción.
- La nota sigue la escala existente de 1 a 10; se admiten hasta dos decimales. Un alumno pendiente no tiene registro en `NotasEvaluacion`. Nunca se convierte una celda vacía en cero.
- El índice único existente `(IdEvaluacion, IdAlumno)` evita duplicados. La nueva migración agrega una restricción SQL para el rango de la nota y un índice de consulta por curso/materia/período/activo.

## Roles y pertenencia

- Docente: consulta y administra evaluaciones únicamente de sus asignaciones activas de curso y materia en su escuela.
- Administrador: administra evaluaciones de los pares curso/materia que tengan asignación activa en su escuela. La relación disponible en el modelo es `DocenteMateriaCurso`; no se inventa una relación nueva entre curso y materia. Si falta una combinación, se configura primero en Asignaciones.
- Alumnos: la grilla y la escritura usan matrícula activa, curso, ciclo lectivo y escuela. No basta con recibir un identificador de alumno válido.
- Las escrituras exigen período y ciclo activos. La fecha de evaluación debe estar dentro del período.
- Se permite archivar una evaluación sin notas. Una evaluación con notas no puede archivarse, para no quitar resultados del promedio accidentalmente.
- Períodos: GET para Administrador/Docente; POST, PUT y DELETE solo Administrador. La baja es lógica. Se rechaza si existen evaluaciones activas, calificaciones o boletines; tampoco se permite achicar fechas dejando evaluaciones fuera del período.

## Calificaciones anteriores y boletines

`Calificaciones` almacena una nota por alumno/materia/período, no por evaluación. Los boletines y consultas anteriores dependen de esa tabla. La implementación previa de evaluaciones calculaba el promedio y sobrescribía esa nota sin distinguir su procedencia.

Se mantienen ambas funciones: `NotasEvaluacion` guarda cada resultado individual; `Calificaciones` recibe el promedio aritmético de las evaluaciones activas, redondeado a dos decimales. Se agrega `GeneradaPorEvaluaciones`: solo los promedios identificados así se actualizan automáticamente. Las calificaciones preexistentes quedan con `false`, sin inferir su origen. Una carga manual por el servicio anterior también las marca como manuales.

El usuario autorizó descartar calificaciones y boletines de prueba; esta entrega no necesita borrarlos para migrar y no contiene una purga automática. Cuando haya una nota previa, la respuesta y la interfaz indican qué alumnos conservan esa calificación del período. Las notas individuales nuevas se guardan de todos modos. Los boletines ya generados siguen siendo documentos almacenados y no se regeneran al guardar una evaluación.

## API

| Método | Ruta | Uso |
|---|---|---|
| GET | `/api/evaluaciones/opciones` | Curso/materia/año habilitados para el usuario |
| GET | `/api/evaluaciones/curso/{curso}/materia/{materia}/periodo/{periodo}` | Evaluaciones y notas guardadas |
| POST | `/api/evaluaciones` | Crear evaluación |
| PUT | `/api/evaluaciones/{id}` | Editar título, temario, descripción y fecha |
| DELETE | `/api/evaluaciones/{id}` | Archivar sin eliminar físicamente |
| GET | `/api/evaluaciones/{id}/alumnos` | Matrículas del curso y año, con nota nullable |
| PUT | `/api/evaluaciones/{id}/notas` | Guardar varias notas en una transacción |
| GET/POST | `/api/ciclos/{ciclo}/periodos` | Consultar/crear períodos |
| GET/PUT/DELETE | `/api/ciclos/{ciclo}/periodos/{id}` | Consultar/editar/dar de baja |

Ejemplo de lote: `{"notas":[{"idAlumno":1,"valor":8.5},{"idAlumno":2,"valor":null}]}`.

Un alumno omitido no se modifica. `null` elimina explícitamente su nota y lo deja pendiente. React envía solo las filas modificadas. La respuesta contiene `resultados` por alumno (`registrada`, `actualizada`, `pendiente`, `sin cambios`) y `calificacionesConservadas`. Si alguna fila es inválida, se rechaza todo el lote con errores por `idAlumno`. La grilla conserva lo escrito. Los errores de conexión tampoco limpian las celdas.

## Archivos principales

Backend:

- `EducAR.API/Controllers/EvaluacionesController.cs`, `DTOs/Evaluaciones/EvaluacionDtos.cs`, `Services/EvaluacionService.cs`, `Services/Interfaces/IEvaluacionService.cs` y registro en `Program.cs`.
- `Models/Evaluacion.cs`, `Models/Calificacion.cs`, `Data/AppDbContext.cs`, `Services/CalificacionService.cs`.
- `Controllers/PeriodosEvaluacionController.cs`, `Services/PeriodoEvaluacionService.cs`, `Repositories/PeriodoEvaluacionRepository.cs` y su interfaz.
- Migración `20260928160829_CompletarEvaluaciones`, su diseñador y snapshot; `scripts/CompletarEvaluaciones.sql`.

Frontend (repositorio EducAr_Web):

- `src/pages/Docente/Calificaciones/EvaluacionesPage.tsx`, `src/api/evaluacionesApi.ts`.
- `src/pages/Periodos/PeriodosPage.tsx`, `src/api/periodosEvaluacionApi.ts`.
- `src/pages/Admin/AdminLayout.tsx`, `src/pages/Docente/DocenteLayout.tsx`.

## Instalación y verificación

Desde EducAR_Gestion_New, con la API detenida:

```powershell
dotnet ef database update --project EducAR.API
dotnet test EducAR_Tests -c Codex
```

Como alternativa para una base que ya tiene `20260915215612_AddEvaluacionesYNotas`, ejecutar `scripts/CompletarEvaluaciones.sql` en la base seleccionada. Es idempotente respecto de la nueva migración. El script no crea una base ni selecciona un servidor. Reiniciar la API después de migrar.

La prueba opcional de SQL Server usa una base aleatoria `EducAR_Evaluaciones_Test_*` en LocalDB. Construye el esquema desde el modelo de `AddEvaluacionesYNotas`, registra ese historial y aplica la nueva migración. Elimina solamente esa base al finalizar:

```powershell
$env:EDUCAR_TEST_SQLSERVER = '1'
dotnet test EducAR_Tests -c Codex --filter FullyQualifiedName~EvaluacionSqlServerTests
Remove-Item Env:EDUCAR_TEST_SQLSERVER
```

En EducAr_Web: `npm run typecheck` y `npm run build`.

Resultado de verificación: 126 pruebas del backend aprobadas; una prueba adicional de SQL Server aprobada al habilitarla explícitamente; TypeScript y compilación de producción del frontend correctos. La prueba SQL cubre actualización de esquema, creación, grilla de alumnos, lote de notas, edición, consulta posterior e índices/restricciones reales. La compilación de Vite conserva una advertencia de tamaño del bundle. No se realizó una prueba visual en navegador ni se aplicó la migración a la base de la aplicación.

Limitación preexistente detectada: ejecutar todo el historial de migraciones sobre una base vacía falla al referenciar `TelefonosContacto`, cuya creación no está incluida en el punto esperado del historial. No se alteró ese historial en este módulo. Una instalación nueva necesita resolver primero esa base inicial; la actualización desde el esquema anterior sí se probó en SQL Server.

Prueba manual de interfaz: configurar tres períodos como administrador, abrir Calificaciones, seleccionar año/curso/materia/período, crear evaluación con todos los campos, ingresar dos notas dejando otra pendiente, guardar una vez, reabrir, modificar una nota y guardar. Probar una nota fuera de rango y verificar que los valores permanezcan. Como docente, Períodos debe mostrar solo consulta. Como administrador, deben estar disponibles crear, editar, dar de baja y reactivar.
