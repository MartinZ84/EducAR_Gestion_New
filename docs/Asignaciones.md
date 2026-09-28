# Asignaciones y Mis cursos

[Índice](README.md)

## Funcionalidades

El administrador puede vincular alumno/tutor y docente/materia/curso desde /admin/asignaciones. Este documento describe la segunda relación; la primera está en [Tutores y vínculos](Tutores-y-vinculos.md).

El docente ve Mis cursos y las mismas tarjetas en su dashboard. Cada tarjeta agrupa por IdCurso, muestra grado/división, turno y ciclo, y permite desplegar sus materias. Asistencia navega solo con IdCurso; Notas navega con IdCurso e IdMateria.

## Reglas y roles

- Solo Administrador crea y desactiva asignaciones.
- Docente y Administrador pueden consultar asignaciones por docente, curso o curso/materia. Mis cursos exige rol Docente y deriva el docente del usuario del token.
- Al asignar se valida curso activo de la escuela, docente activo de la escuela y materia activa de la escuela.
- La combinación docente/materia/curso es única. La asociación activa duplicada se rechaza y una inactiva se reactiva.
- Distintos docentes pueden tener asignada la misma combinación curso/materia: el índice único incluye al docente.
- Dar de baja una asignación no elimina el docente, el curso, la materia ni las notas existentes.
- La agrupación visual evita una tarjeta por materia y conserva separados turnos/ciclos por usar IdCurso.

## Filtros y límites

ObtenerMisCursos consulta las asignaciones activas del docente; no filtra explícitamente por año actual ni por todos los estados relacionados. Aunque el subtítulo diga “este ciclo”, no se debe asumir ese filtro como regla del servidor. Las opciones de Evaluaciones tienen filtros propios adicionales.

## API y fuentes

/api/DocenteMateriaCurso: POST; /{id}: DELETE; /docente/{idDocente}, /curso/{idCurso}, /curso/{idCurso}/materia/{idMateria}, /mis-cursos: GET.

[Servicio](../EducAR.API/Services/DocenteMateriaCursoService.cs), [repositorio](../EducAR.API/Repositories/DocenteMateriaCursoRepository.cs), [asignaciones React](../../EducAr_Web/src/pages/Admin/Asignaciones/AsignacionesPage.tsx), [tarjetas compartidas](../../EducAr_Web/src/components/CursosDocenteCards.tsx).
