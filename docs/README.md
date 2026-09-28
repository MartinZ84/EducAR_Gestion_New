# Documentación funcional de Educar

Revisión del código local: 28/09/2026. Describe el comportamiento implementado en EducAR_Gestion_New (API .NET y modelo SQL Server) y EducAr_Web (React). No certifica qué migraciones están aplicadas en una instalación ni sustituye una prueba de ejecución.

## Documentos

| Documento | Contenido |
|---|---|
| [Roles y permisos](Roles-y-permisos.md) | Matriz de permisos efectivos, acceso por rol y alcance de las consultas |
| [Reglas de negocio](Reglas-de-negocio.md) | Reglas compartidas, identificadores, estados y relaciones |
| [Autenticación y usuarios](Autenticacion-y-usuarios.md) | Login, sesión, usuarios, perfil, contraseña y catálogo de roles |
| [Escuelas](Escuelas.md) | Consulta, alta y edición de instituciones |
| [Docentes](Docentes.md) | Registro, edición, baja y detalle del docente |
| [Alumnos y contactos](Alumnos-y-contactos.md) | Datos personales, tutores, teléfonos y ficha del alumno |
| [Tutores y vínculos](Tutores-y-vinculos.md) | Usuarios tutores, parentesco y responsable principal |
| [Organización académica](Organizacion-academica.md) | Ciclos lectivos, cursos y materias |
| [Asignaciones](Asignaciones.md) | Docente–materia–curso y tarjetas de Mis cursos |
| [Matrículas](Matriculas-funcional.md) | Matrícula individual, masiva, baja y conflictos |
| [Períodos](Periodos.md) | Configuración de trimestres, CRUD administrador y consulta docente |
| [Evaluaciones y períodos](Evaluaciones-y-periodos.md) | Evaluaciones, grilla de notas, promedio, migración y pruebas de implementación |
| [Asistencia](Asistencia.md) | Registro diario por curso, calendario, pendientes y avisos |
| [Boletines](Boletines.md) | Generación, consulta, conceptos y observación |
| [Mensajería](Mensajeria.md) | Destinatarios relacionados, envíos múltiples y lectura |
| [Portal del tutor](Portal-del-tutor.md) | Consulta de alumnos vinculados, asistencia y notas |
| [Paneles y navegación](Paneles-y-navegacion.md) | Menús por rol, tarjetas y notificaciones |
| [Inventario de endpoints](API-endpoints.md) | Métodos y rutas declarados en los controladores |
| [Limitaciones actuales](Limitaciones-actuales.md) | Diferencias comprobadas y aspectos que no deben darse por implementados |

## Cómo leer y mantener esta documentación

Cada ficha incluye funcionalidades, permisos, reglas y referencias al código. El menú visible y la autorización del servidor se documentan por separado: que una pantalla no esté en un menú no significa que el endpoint esté prohibido.

El inventario de API refleja atributos de rutas y autorización; las comprobaciones de escuela, asignación y vínculos están en los servicios o controladores y se explican en las fichas.

Para cambiar una funcionalidad, actualizar su ficha, la matriz de roles si cambia un permiso y el inventario si cambian las rutas. Los documentos no fijan políticas futuras como si ya existieran.

[Documento histórico de matrículas](MATRICULAS.md): conserva el contexto de aquella migración. Para el funcionamiento actual, usar Matriculas-funcional.md. Las cifras de pruebas en Evaluaciones-y-periodos.md corresponden a esa entrega; esta revisión documental no vuelve a ejecutar esas pruebas.
