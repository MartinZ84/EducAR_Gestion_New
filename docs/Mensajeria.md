# Mensajería

[Índice](README.md)

## Funcionalidades y roles

Administrador, Docente y Tutor tienen bandejas de recibidos/enviados, consulta de detalle, composición, selección de uno o varios destinatarios y contador de mensajes sin leer. La pantalla de composición se comparte entre los roles.

La búsqueda de destinatarios y el envío están habilitados para los tres roles. Las bandejas, detalle, contador y marca de leído exigen autenticación y operan sobre la identidad del usuario.

## Reglas de destinatarios

- Docente puede escribir a tutores de alumnos matriculados en sus cursos.
- Tutor puede escribir a docentes asignados a los cursos de sus alumnos vinculados.
- Administrador puede escribir a otros administradores, docentes y tutores activos de su misma escuela. El selector permite buscar y agrupa las opciones por rol.
- Se exige ciclo activo del año actual, curso activo, matrícula activa, alumno activo, vínculo/asignación activos y usuarios destinatarios activos según la consulta.
- Para Docente y Tutor, el envío verifica que los destinatarios estén vinculados según las reglas anteriores. Para Administrador, verifica que pertenezcan a su escuela y a uno de los tres roles habilitados.
- No se permite enviarse a sí mismo ni enviar una solicitud sin destinatarios.
- Los IDs repetidos se deduplican. “Seleccionar todos” no evita las validaciones de servidor.
- El modelo admite idUsuarioDestinat individual y una lista de destinatarios para compatibilidad.

## Lectura y persistencia

Se crea un registro por destinatario. Cada copia tiene su propio Leido. Abrir un mensaje como destinatario lo marca leído; el remitente puede consultarlo pero no marcarlo como leído por el otro.

Solo remitente o destinatario acceden al detalle. Las bandejas muestran mensajes activos y se ordenan por fecha descendente. No hay endpoint de edición o eliminación de mensajes en el controller revisado.

Las inserciones múltiples se realizan individualmente; no hay una transacción envolviendo todo el envío. Un fallo de persistencia a mitad del proceso podría dejar parte enviada.

## Notificaciones

MensajesNotification consulta cada 15 segundos y escucha el evento local mensajes-actualizados. Es sondeo HTTP, no WebSocket/SignalR.

Al guardar el mensaje interno, la API intenta enviar un email individual a cada destinatario. El remitente configurado del sistema se usa como `From` y el email registrado del usuario emisor se establece como `Reply-To`. Si SMTP no está configurado o falla alguna entrega, las copias internas se conservan y la respuesta informa que el email no se pudo enviar a todos.

Configurar SMTP mediante variables de entorno de ASP.NET Core: `Email__SmtpHost`, `Email__SmtpPort` (predeterminado `587`), `Email__EnableSsl` (predeterminado `false`), `Email__FromAddress`, `Email__FromName` (opcional), `Email__Username` y `Email__Password`. No guardar credenciales en el repositorio.

## API y fuentes

/api/Mensajes/recibidos, /enviados, /noleidos, /destinatarios, /{id}: GET; /api/Mensajes: POST; /{id}/leido: PATCH.

[Servicio](../EducAR.API/Services/MensajeService.cs), [repositorio](../EducAR.API/Repositories/MensajeRepository.cs), [controller](../EducAR.API/Controllers/MensajesController.cs), [pantalla](../../EducAr_Web/src/pages/Docente/Mensajes/MensajesPage.tsx), [notificación](../../EducAr_Web/src/components/MensajesNotification.tsx).
