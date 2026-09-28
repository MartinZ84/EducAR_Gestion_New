# Matrículas

[Índice](README.md)

## Funcionalidades y roles

Administrador: matricular individualmente o por lote, dar de baja y consultar. Docente: consultar por curso/alumno/ID y candidatos; no modificar. Pantalla administrativa: /admin/matriculas.

La pantalla permite elegir ciclo y curso, buscar alumnos por nombre/apellido/DNI, filtrar por año de registro y seleccionar varios para asignar. El año de registro corresponde al alta del alumno, no necesariamente al ciclo a matricular.

## Reglas

- Alumno activo de la escuela, curso activo de la escuela y ciclo del curso activo.
- El ciclo de la matrícula se obtiene del curso.
- Solo existe una matrícula por alumno y ciclo, incluso si fue dada de baja.
- Si está activa en el mismo curso, el alta individual rechaza el duplicado.
- Si está activa en otro curso del mismo ciclo, se informa conflicto: no se cambia de curso silenciosamente.
- Si está inactiva, se reutiliza la matrícula, se actualiza el curso, se limpia FechaBaja y se registra nuevamente la fecha de matrícula.
- Dar de baja conserva la fila y registra el estado/fecha de baja.
- No hay promoción automática de alumnos al siguiente año.

## Lote y consulta

El body de /asignar-masivo contiene idCurso e idsAlumnos. Los IDs se consideran sin duplicados. El resultado distingue Matriculado, Reactivado, YaMatriculado, Conflicto y NoEncontrado.

El proceso persiste los válidos en una transacción y devuelve observaciones para los demás. Un conflicto individual no obliga a rechazar todos los alumnos, a diferencia del guardado de notas.

La consulta de candidatos es paginada, ordenada por apellido, nombre, DNI y fecha de nacimiento; muestra matrícula/curso/estado del ciclo elegido.

## API y fuentes

/api/Matriculas: POST; /asignar-masivo: POST; /alumnos-disponibles, /curso/{idCurso}, /alumno/{idAlumno}, /{id}: GET; /{id}: DELETE.

[Servicio](../EducAR.API/Services/MatriculaService.cs), [repositorio](../EducAR.API/Repositories/MatriculaRepository.cs), [modelo](../EducAR.API/Models/Matricula.cs), [pantalla](../../EducAr_Web/src/pages/Admin/Matriculas/MatriculasPage.tsx).

[Documento histórico](MATRICULAS.md): describe la introducción de la matrícula y nombres anteriores. La recomendación antigua de recrear una base no es un requisito del flujo funcional actual.
