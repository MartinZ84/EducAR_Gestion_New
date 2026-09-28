# Organización académica: ciclos, cursos y materias

[Índice](README.md)

## Roles y pantallas

Administrador: altas, consultas, ediciones y bajas en /admin/ciclos, /admin/cursos y /admin/materias. Docente: consultas de estas entidades por API. Tutor: sin acceso a estos catálogos generales.

## Ciclos lectivos

Campos principales: año, inicio, fin, escuela y estado. Se rechaza repetir un año dentro de la escuela y una fecha final igual o anterior a la inicial. Se pueden editar año/fechas/estado; la baja es lógica. La ficha muestra cursos y cantidades de alumnos matriculados.

No existe una regla global que garantice exactamente un ciclo activo. Cambiar las fechas del ciclo no equivale a validar automáticamente todas sus relaciones dependientes.

## Cursos

El curso tiene grado, división, turno, ciclo y escuela. La creación exige ciclo activo de la escuela. El servicio comprueba duplicados por grado/división/turno/ciclo; editar mantiene el ciclo original.

La baja mediante DELETE se rechaza si hay alumnos matriculados activos, asistencias registradas o boletines generados. El nombre del método de repositorio TieneCalificaciones no debe confundirse con su mensaje/regla de boletines.

La edición también permite modificar Activo y no invoca las mismas comprobaciones del método Eliminar. Por eso esas restricciones se describen para DELETE, no como garantía universal de toda desactivación.

La ficha muestra los alumnos con matrícula activa y alumno activo. El contador del listado usa matrículas activas y puede diferir si hay alumnos inactivos.

## Materias

Nombre, descripción, estado y escuela. Alta y edición rechazan nombres duplicados dentro de la escuela según el servicio. La baja es lógica. El detalle permite consultar las relaciones académicas cargadas por el módulo.

Una materia no queda disponible automáticamente en todos los cursos: las combinaciones de trabajo docente provienen de DocenteMateriaCurso.

## API y fuentes

/api/CiclosLectivos, /api/Cursos, /api/Materias. Incluyen consultas de detalle; Cursos incorpora /ciclo/{idCicloLectivo}.

[CicloLectivoService](../EducAR.API/Services/CicloLectivoService.cs), [CursoService](../EducAR.API/Services/CursoService.cs), [MateriaService](../EducAR.API/Services/MateriaService.cs), [CursoRepository](../EducAR.API/Repositories/CursoRepository.cs), [pantallas administrador](../../EducAr_Web/src/pages/Admin).
