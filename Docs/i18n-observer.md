# Sistema de gestion dinamica de idiomas

## Analisis de la solucion

La solucion implementa i18n sin usar archivos estaticos como fuente principal. Los idiomas, etiquetas y traducciones se guardan en la base de datos y se administran desde la pantalla `Idiomas`.

El cambio de idioma se resuelve con Observer:

- Observable/Subject: `LanguageManager`.
- Observers: `MainForm` y las vistas que heredan de `LocalizedUserControl`.
- Servicio de traduccion: `TranslationService`.
- Persistencia: `LanguageRepository`, `TranslationRepository` y `UsuarioRepository`.

Cada texto visible se identifica con una clave estable, por ejemplo `BTN_SAVE`. La UI no conoce las traducciones ni llama a otras pantallas; solo se suscribe al manager y vuelve a pedir sus textos cuando recibe la notificacion.

## Funcionamiento

1. Al iniciar sesion, `LanguageManager.Initialize(usuario)` busca `Usuario.IdiomaPreferidoId`.
2. Si el usuario no tiene idioma preferido o esta inactivo, se usa el idioma activo por defecto.
3. El selector de idioma muestra idiomas activos desde BD.
4. Al cambiar el selector, `LanguageManager.ChangeLanguage(idioma, usuario)` actualiza el idioma actual.
5. Si hay usuario autenticado, se persiste `Usuario.id_idioma`.
6. `LanguageManager.Notify()` avisa a todos los observers.
7. Cada observer vuelve a pedir traducciones con `LanguageManager.Translate(key)`.
8. La UI se actualiza en tiempo de ejecucion sin reiniciar.

## Clases principales

```mermaid
classDiagram
    class IObservableLanguage {
        +Attach(observer)
        +Detach(observer)
        +Notify()
    }

    class IObserverLanguage {
        +OnLanguageChanged(idioma)
    }

    class LanguageManager {
        +CurrentLanguage
        +Initialize(usuario)
        +ChangeLanguage(idioma, usuario)
        +Translate(key)
        +ListarIdiomasActivos()
    }

    class TranslationService {
        +Translate(key, idioma)
        +ListarPorIdioma(idioma)
    }

    class LanguageRepository {
        +Crear(idioma, usuarioId)
        +Actualizar(idioma, usuarioId, motivo)
        +ObtenerDefault()
        +Listar(soloActivos)
    }

    class TranslationRepository {
        +CrearEtiqueta(etiqueta)
        +GuardarTraduccion(traduccion)
        +ObtenerTexto(key, idiomaId)
        +ListarEtiquetas()
    }

    class MainForm
    class LocalizedUserControl
    class IdiomasView
    class Idioma
    class Etiqueta
    class Traduccion
    class Usuario

    IObservableLanguage <|.. LanguageManager
    IObserverLanguage <|.. MainForm
    IObserverLanguage <|.. LocalizedUserControl
    LocalizedUserControl <|-- IdiomasView
    LanguageManager --> TranslationService
    LanguageManager --> LanguageRepository
    LanguageManager --> Usuario
    TranslationService --> TranslationRepository
    LanguageRepository --> Idioma
    TranslationRepository --> Etiqueta
    TranslationRepository --> Traduccion
```

## DER

```mermaid
erDiagram
    IDIOMA ||--o{ TRADUCCION : contiene
    ETIQUETA ||--o{ TRADUCCION : define
    IDIOMA ||--o{ USUARIO : preferido_por
    IDIOMA ||--o{ IDIOMA_ESTADO_HISTORIAL : audita
    USUARIO ||--o{ IDIOMA_ESTADO_HISTORIAL : responsable

    IDIOMA {
        int id_idioma PK
        varchar codigo
        varchar nombre
        varchar estado_idioma
    }

    ETIQUETA {
        int id_etiqueta PK
        varchar clave
        varchar descripcion
    }

    TRADUCCION {
        int id_traduccion PK
        int id_etiqueta FK
        int id_idioma FK
        nvarchar texto
    }

    USUARIO {
        int id_usuario PK
        int id_idioma FK
        varchar nombre_usuario
        varchar email
    }

    IDIOMA_ESTADO_HISTORIAL {
        int id_idioma_estado_historial PK
        int id_idioma FK
        varchar estado_anterior
        varchar estado_nuevo
        varchar motivo
        datetime fecha_cambio
        int id_usuario_responsable FK
    }
```

## Secuencia de cambio de idioma

```mermaid
sequenceDiagram
    actor Usuario
    participant Combo as ComboBox Idiomas
    participant LM as LanguageManager
    participant UR as UsuarioRepository
    participant Obs as Observers UI
    participant TS as TranslationService
    participant TR as TranslationRepository

    Usuario->>Combo: Selecciona idioma
    Combo->>LM: ChangeLanguage(idioma, usuario)
    LM->>LM: Actualiza CurrentLanguage
    LM->>UR: ActualizarIdiomaPreferido(usuarioId, idiomaId)
    LM->>Obs: Notify / OnLanguageChanged(idioma)
    Obs->>LM: Translate(key)
    LM->>TS: Translate(key, idioma)
    TS->>TR: ObtenerTexto(key, idiomaId)
    TR-->>TS: Texto
    TS-->>LM: Texto traducido
    LM-->>Obs: Texto traducido
    Obs->>Obs: Actualiza controles visibles
```

## Extension

Para agregar un nuevo idioma no se recompila:

1. Crear idioma desde la pantalla `Idiomas`.
2. Activarlo.
3. Crear o seleccionar etiquetas.
4. Agregar traducciones por idioma.
5. El idioma aparece en el selector si esta activo.

La activacion/desactivacion queda persistida en `Idioma.estado_idioma` y documentada en `IdiomaEstadoHistorial`.
