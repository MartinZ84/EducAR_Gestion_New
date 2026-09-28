# Escuelas

[Índice](README.md)

## Funcionalidades y roles

La API ofrece listado paginado, consulta por ID, alta y edición de escuelas. Consulta para usuarios autenticados; escritura para Administrador. No hay DELETE en EscuelasController ni una pantalla propia de escuelas en el menú administrador revisado.

## Datos y reglas

Una escuela contiene nombre, dirección, teléfono, email y estado. El servicio rechaza nombres duplicados al crear y editar. La edición permite modificar Activo.

Usuarios, alumnos, ciclos, cursos y materias se relacionan con la escuela. El JWT transporta la escuela del usuario para los módulos que filtran por ella.

## Alcance particular

EscuelasController usa el ID de la ruta para consultar y editar; no lo reemplaza por el IdEscuela del JWT. Por lo tanto, el comportamiento implementado para un administrador es más amplio que “solo editar su propia escuela”. No existe un rol de administración global separado que justifique esa diferencia en el modelo de permisos actual.

## API y fuentes

/api/Escuelas: GET y POST; /api/Escuelas/{id}: GET y PUT.

[Controller](../EducAR.API/Controllers/EscuelasController.cs), [servicio](../EducAR.API/Services/EscuelaService.cs), [repositorio](../EducAR.API/Repositories/EscuelaRepository.cs).
