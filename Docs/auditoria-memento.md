# Auditoria con patron Memento

## Analisis previo

### Arquitectura encontrada

El proyecto esta organizado por capas:

- `Domain`: entidades y objetos de dominio (`Usuario`, permisos, idioma, bitacora, sesion).
- `Application`: casos de uso y coordinacion de reglas de aplicacion.
- `Repository`: fachada de acceso a datos para cada agregado o modulo.
- `DAL`: mappers SQL Server y `DatabaseContext`.
- `Services`: servicios tecnicos simples, como hash de password e idioma.
- `UI`: formularios WinForms y vistas.
- `Database`: scripts de schema y stored procedures.

La modificacion de usuarios entra desde `UI\Views\Usuarios\UsuariosView.cs`, llama a `UsuarioApplicationService.ModificarUsuario`, normaliza datos y delega en `UsuarioRepository`, que a su vez usa `UsuarioDataMapper`.

### Hallazgos

- Ya existe una `Bitacora`, pero esta orientada a eventos de seguridad y operacion, como login y registro fallido. No conviene mezclarla con auditoria de estado historico.
- `UsuarioApplicationService` instancia repositorios concretos en el constructor por defecto. Esto facilita el uso desde UI, pero acopla la aplicacion a implementaciones concretas.
- `UsuarioDataMapper.Editar` actualiza password con `COALESCE`, pero las consultas actuales no recuperan el hash. Es correcto no exponerlo para auditoria.
- No existia una consulta por id de usuario, necesaria para capturar el estado real anterior y posterior.
- La base de datos tiene algunos historiales especificos (`HistorialEstadoTurno`, `IdiomaEstadoHistorial`), pero no una tabla generica de auditoria reutilizable.

### Riesgos tecnicos

- Si la auditoria se registra despues de modificar el usuario, una falla al insertar auditoria no revierte el cambio principal. Para este alcance se mantiene esa decision para no redisenar transacciones entre repositorios.
- Si se agregan campos sensibles a futuras entidades, deben excluirse o enmascararse desde el memento.
- El uso de SQL embebido en mappers obliga a mantener sincronizados los scripts de `Database` con el codigo DAL.
- La ausencia de interfaces para repositorios hace mas costoso testear casos de uso aislados.

### Recomendaciones

- Mantener `Bitacora` para eventos del sistema y usar `Auditoria` para cambios reconstruibles de entidades.
- Guardar snapshots y cambios en JSON para evitar migraciones cada vez que cambie una entidad.
- Hacer que cada entidad auditable exponga su propio memento. Asi la entidad decide que estado es auditable y que datos sensibles se omiten.
- Centralizar comparacion y persistencia en un servicio de aplicacion de auditoria.
- Para futuras entidades, agregar `CrearMemento` en la entidad y llamar al servicio de auditoria desde el caso de uso que realiza la modificacion.

## Diseno de la solucion

### Uso del patron Memento

`Usuario` actua como originator: conoce su estado interno y puede generar una captura auditable mediante `SaveToMemento`. Se mantiene `CrearMemento` como alias local para no perder legibilidad en el codigo existente.

`AuditoriaMemento` representa la captura del estado de una entidad en un momento dado. Expone:

- Entidad.
- Id de entidad.
- Fecha de captura.
- Estado como diccionario clave/valor.

En el flujo real, `AuditoriaApplicationService` coordina el historial persistente: recibe el memento anterior y posterior, calcula diferencias y delega la persistencia.

### Flujo de auditoria para Usuario

1. `UsuarioApplicationService.ModificarUsuario` valida y normaliza el usuario recibido.
2. Obtiene el usuario actual desde base por id.
3. Crea el memento anterior con `SaveToMemento`.
4. Ejecuta la modificacion existente.
5. Si la modificacion fue exitosa, vuelve a obtener el usuario desde base.
6. Crea el memento posterior con `SaveToMemento`.
7. Calcula cambios campo por campo.
8. Persiste un registro en `Auditoria` con JSON para estado anterior, estado posterior y cambios.

### Modelo persistido

La tabla `Auditoria` es generica:

- `id_auditoria`
- `entidad`
- `id_entidad`
- `accion`
- `id_usuario_actor`
- `identificador_usuario_actor`
- `fecha_evento`
- `estado_anterior_json`
- `estado_nuevo_json`
- `cambios_json`

Esta estructura permite incorporar nuevas entidades sin alterar el esquema.

## Extension a futuras entidades

Para auditar una entidad nueva:

1. Agregar un metodo `SaveToMemento` en la entidad, y opcionalmente `RestoreFromMemento` si se necesita recomposicion/restauracion.
2. Incluir solo campos auditables y excluir datos sensibles.
3. En el caso de uso de modificacion, obtener estado anterior y posterior.
4. Llamar a `AuditoriaApplicationService.RegistrarModificacion`.
5. No modificar la tabla `Auditoria`.

Ejemplo conceptual:

```csharp
AuditoriaMemento anterior = productoAnterior.CrearMemento();
bool guardado = _productoRepository.Modificar(producto);
AuditoriaMemento posterior = _productoRepository.ObtenerPorId(producto.Id).CrearMemento();
_auditoriaService.RegistrarModificacion(anterior, posterior);
```

## Consideraciones

- La password de `Usuario` no se guarda en auditoria para evitar persistir datos sensibles, ni siquiera como hash.
- Si se necesita auditar cambios de password, conviene registrar un indicador semantico como `PasswordActualizada`, sin guardar valores anterior/posterior.
- La restauracion historica queda facilitada por `estado_anterior_json` y `estado_nuevo_json`, pero no se implementa en este alcance porque el pedido actual se limita a registrar modificaciones de `Usuario`.
