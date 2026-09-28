# Reglas de negocio compartidas

[Índice](README.md)

## Identidad y relaciones

| Identificador | Significado |
|---|---|
| IdUsuario | Cuenta que inicia sesión; también remitente/destinatario de mensajes |
| IdDocente / IdTutor | Perfil vinculado a una cuenta; no es intercambiable con IdUsuario |
| IdAlumno | Alumno, sin cuenta de acceso propia en el flujo actual |
| IdCicloLectivo | Ciclo al que pertenecen cursos, períodos y matrículas |
| IdCurso | Curso concreto, con grado, división, turno y ciclo lectivo |
| IdPeriodoEvaluacion | Período configurable dentro de un ciclo |
| IdEvaluacion | Actividad evaluable de un curso/materia/período |

La etiqueta “6° A” no es la clave del curso. Dos cursos pueden compartir grado/división y diferir en turno o ciclo. Las tarjetas y operaciones agrupan por IdCurso.

La pertenencia de un alumno a un curso se representa con Matricula. La asignación del docente se representa con DocenteMateriaCurso. Las materias no contienen por sí solas una lista curricular de cursos.

## Unicidad declarada en el modelo

| Entidad | Índice único |
|---|---|
| Usuario | Escuela + nombre de usuario |
| Matricula | Alumno + ciclo lectivo |
| AlumnoTutor | Alumno + tutor |
| DocenteMateriaCurso | Docente + materia + curso |
| Asistencia | Alumno + curso + fecha |
| Calificacion | Alumno + materia + período |
| NotaEvaluacion | Evaluación + alumno |
| Boletin | Alumno + período |
| DetalleBoletin | Boletín + materia |

El índice de matrícula incluye también los registros dados de baja. Por eso se reutiliza una matrícula al reactivar al alumno en el mismo ciclo. Las validaciones de nombres, DNI o email pueden estar en servicios aunque no exista un índice único equivalente: no confundir ambas garantías.

## Estados y fechas

La mayoría de las bajas usan Activo=false y conservan registros. Las matrículas usan Estado y FechaBaja. Teléfonos es una excepción: se elimina físicamente el registro.

“Año actual” no es una configuración global uniforme: mensajería y calendario de pendientes usan el año del servidor; evaluaciones permite elegir entre los años de sus opciones habilitadas. No afirmar que toda la aplicación filtra siempre por un único ciclo activo.

Los períodos se configuran por nombre, inicio y fin. No existe numeración rígida de trimestre ni cierre automático de notas por una fecha en el modelo actual.

## Resultados académicos

- Asistencia: alumno/curso/día; nunca por materia.
- Nota de evaluación: alumno/evaluación, entre 1 y 10; sin registro significa pendiente.
- Calificación de período: alumno/materia/período; puede ser un promedio identificado como generado por evaluaciones.
- Boletín: copia almacenada de calificaciones al generarlo. Cambiar una nota no actualiza automáticamente boletines anteriores.
- Los conceptos del boletín no constituyen una regla implementada de promoción o aprobación de curso.

## Operaciones múltiples

La matrícula masiva devuelve resultados individuales y puede matricular a los válidos mientras informa conflictos de otros. El lote de notas rechaza toda la solicitud si hay una fila inválida. El envío múltiple de mensajes valida destinatarios y crea un mensaje por usuario; no declara una transacción conjunta para todas las inserciones.

## Fuentes

[Modelo e índices](../EducAR.API/Data/AppDbContext.cs), [modelos](../EducAR.API/Models), [matrículas](Matriculas-funcional.md), [evaluaciones](Evaluaciones-y-periodos.md), [asistencia](Asistencia.md).
