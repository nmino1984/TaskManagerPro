# TaskManagerPro

Un sistema completo de gestión de tareas con soporte para tareas, subtareas e hitos. Construye proyectos complejos con claridad y organización.

## Características

✅ **Gestión de Tareas**
- Crear, leer, actualizar y eliminar tareas
- Rastrear estado de tarea: No iniciada, En progreso, Completada, Retrasada
- Establecer prioridades: Baja, Media, Alta
- Definir fechas de inicio y fin

✅ **Subtareas**
- Desglosa tareas en subtareas manejables
- Rastrear el estado de completación de subtareas
- Calcular automáticamente el progreso de la tarea basado en subtareas

✅ **Hitos (Milestones)**
- Define puntos de control importantes en tu proyecto
- Rastrear estado del hito: Pendiente, Completado, Retrasado
- Exportar hitos en múltiples formatos

✅ **Funcionalidad de Exportación**
- Exportar hitos a **JSON** (integración de datos)
- Exportar hitos a **XML** (formato estándar)
- Exportar hitos a **iCal** (integración con calendario: Google Calendar, Outlook, Apple Calendar)

✅ **Notificaciones Asincrónicas**
- Trabajos en segundo plano con Hangfire
- Notificaciones cuando se crean tareas
- Notificaciones cuando se completan tareas
- Verificación automática cada hora de tareas vencidas
- Badge en tiempo real en la barra de navegación con contador de no leídas
- Menú desplegable con notificaciones recientes

✅ **Multi-Usuario y Seguridad**
- Registro de usuarios y autenticación JWT
- Cada usuario ve solo sus propios datos (multi-inquilino)
- Hash seguro de contraseñas con BCrypt
- Validación de autorización adecuada en todas las operaciones

✅ **Búsqueda y Organización**
- Paginación con tamaño de página configurable
- Filtrar por estado, prioridad o término de búsqueda
- Ver todas las subtareas e hitos de una tarea

## Inicio Rápido

### Requisitos Previos
- **.NET 10** o superior
- **Node.js 18+** y npm
- **SQLite** (incluido con .NET)

### Configuración del Backend

**Opción 1: Usar la Base de Datos de Ejemplo Incluida (La Más Rápida)**
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
```

**Opción 2: Crear una Base de Datos Fresca Automáticamente**
```bash
cd src/MyApp
dotnet run
```

La aplicación:
- Creará automáticamente la base de datos SQLite (`TaskManagerPro.db`) vía migraciones de EF Core
- Aplicará todas las migraciones de base de datos
- Cargará datos de prueba (solo en modo desarrollo)
- Se iniciará en `http://localhost:5141`

**Para instrucciones detalladas de configuración de base de datos, carga de datos y limpieza**, ver [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md)

### Configuración del Frontend

```bash
cd frontend
npm install
npm start
```

El frontend se iniciará en `http://localhost:4200`

### Acceder a la Aplicación

1. Abre `http://localhost:4200` en tu navegador
2. Haz clic en **Registrarse** para crear una nueva cuenta
3. Ingresa nombre de usuario y contraseña
4. ¡Comienza a crear tareas!

## Puntos de Acceso de API

### Autenticación
- `POST /api/v1/auth/register` - Crear nuevo usuario
- `POST /api/v1/auth/login` - Login de usuario

### Tareas
- `GET /api/v1/tasks?page=1&pageSize=10` - Listar tareas con paginación
- `POST /api/v1/tasks` - Crear tarea
- `PUT /api/v1/tasks/{id}` - Actualizar tarea
- `DELETE /api/v1/tasks/{id}` - Eliminar tarea (eliminación suave)

### Subtareas
- `GET /api/v1/subtasks/bytask/{taskId}` - Obtener subtareas de una tarea
- `POST /api/v1/subtasks` - Crear subtarea
- `PUT /api/v1/subtasks/{id}` - Actualizar subtarea
- `DELETE /api/v1/subtasks/{id}` - Eliminar subtarea

### Hitos
- `GET /api/v1/milestones/bytask/{taskId}` - Obtener hitos de una tarea
- `POST /api/v1/milestones` - Crear hito
- `PUT /api/v1/milestones/{id}` - Actualizar hito
- `DELETE /api/v1/milestones/{id}` - Eliminar hito

### Exportar Hitos
- `GET /api/v1/milestones/bytask/{taskId}/export/json` - Exportar como JSON
- `GET /api/v1/milestones/bytask/{taskId}/export/xml` - Exportar como XML
- `GET /api/v1/milestones/bytask/{taskId}/export/ical` - Exportar como iCalendar (.ics)

### Notificaciones
- `GET /api/v1/notifications` - Obtener todas las notificaciones del usuario actual
- `GET /api/v1/notifications/unread` - Obtener número de notificaciones no leídas
- `PATCH /api/v1/notifications/{id}/read` - Marcar una notificación como leída
- `PATCH /api/v1/notifications/read-all` - Marcar todas las notificaciones como leídas

**Todos los puntos de acceso requieren autenticación JWT** (excepto registro e inicio de sesión)

## Estructura del Proyecto

```
TaskManagerPro/
├── src/MyApp/
│   ├── Domain/              # Entidades (Task, SubTask, Milestone, User)
│   ├── Application/         # Servicios, DTOs, Lógica de Negocio
│   ├── Infrastructure/      # Base de datos, EF Core, Migraciones
│   ├── API/                 # Controladores, Middleware, Validadores
│   └── Program.cs           # Configuración de inicio
├── tests/MyApp.Tests.Integration/
│   ├── Controllers/         # Pruebas de controladores
│   ├── Infrastructure/      # Fábricas de pruebas y clases base
│   └── *Tests.cs            # Suites de pruebas de integración
├── frontend/
│   ├── src/app/
│   │   ├── core/            # Modelos, Servicios, Interceptores
│   │   ├── features/        # Módulos de características (tareas, subtareas, hitos)
│   │   ├── shared/          # Componentes compartidos y utilidades
│   │   └── app.component.ts # Componente raíz
│   └── angular.json         # Configuración de Angular
├── TaskManagerPro.db.example # Base de datos de ejemplo (puede copiarse para inicio rápido)
├── DATABASE_SETUP.es.md      # Configuración, carga y limpieza de base de datos
└── README.es.md              # Este archivo
```

## Pila Tecnológica

| Capa | Tecnología |
|------|-----------|
| **Backend** | .NET 10, ASP.NET Core, Entity Framework Core, Hangfire |
| **Frontend** | Angular 21, Angular Material, TypeScript, Signals |
| **Base de Datos** | SQLite (desarrollo), SQL Server (producción) |
| **Trabajos en Segundo Plano** | Hangfire 1.8.6 con MemoryStorage (dev) / SqlServer (prod) |
| **Autenticación** | JWT Bearer Tokens |
| **Validación** | FluentValidation, Angular Reactive Forms |
| **Logging** | Serilog |
| **Exportación** | System.Text.Json, System.Xml, Ical.Net |

## Trabajos en Segundo Plano (Hangfire)

Las notificaciones asincrónicas se ejecutan a través de trabajos en segundo plano de Hangfire:
- **Tarea Creada**: Se genera una notificación cuando se crea una nueva tarea
- **Tarea Completada**: Se genera una notificación cuando el estado de una tarea cambia a Completada
- **Verificación de Tareas Vencidas**: Trabajo recurrente (cada hora) para detectar y notificar tareas vencidas

**Panel de Control**: Accede a `http://localhost:5141/hangfire` en desarrollo para monitorear trabajos.

## Pruebas

### Ejecutar Pruebas de Integración

```bash
# Desde la raíz del repositorio
dotnet test
```

Esperado: **22/22 pruebas pasando** ✓

### Pruebas Manuales

1. **Registrarse**: Crea una nueva cuenta de usuario
2. **Crear Tarea**: Añade una tarea con título, descripción, fechas y prioridad
3. **Añadir Subtareas**: Crea subtareas para desglosa el trabajo
4. **Añadir Hitos**: Define puntos de control importantes
5. **Exportar**: Descarga hitos en formato JSON, XML o iCal
6. **Actualizar**: Edita tareas y rastrear progreso
7. **Eliminar**: Elimina tareas completadas

## Gestión de Base de Datos

### Inicio Rápido con Base de Datos de Ejemplo
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
```

### Agregar Datos a la Base de Datos

**Método 1: A través de la Interfaz Web**
- Registra un usuario y usa la interfaz de la aplicación

**Método 2: A través de la API**
- Usa curl o Postman para crear tareas vía API REST

**Método 3: Datos de Prueba Automáticos**
- Los datos se cargan automáticamente en modo de Desarrollo

Ver [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md) para instrucciones detalladas y ejemplos.

### Limpiar Base de Datos

```bash
# Opción 1: Eliminar y recrear desde el ejemplo
cd src/MyApp
rm TaskManagerPro.db
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db

# Opción 2: Restablecer completamente (base de datos fresca)
rm TaskManagerPro.db TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null
dotnet run
```

Ver [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md) para más opciones e instrucciones detalladas de limpieza.

## Características de Seguridad

✅ **Autenticación**
- Tokens JWT Bearer con caducidad configurable
- Hash seguro de contraseñas (BCrypt)
- Validación de token automática en todos los puntos de acceso protegidos

✅ **Autorización**
- Acceso a datos con alcance de usuario (los usuarios solo ven sus propios datos)
- Validación de permisos en todas las operaciones
- Cumplimiento de multi-inquilino a nivel de base de datos

✅ **Protección de Datos**
- Eliminación suave (los datos no se eliminan permanentemente)
- Manejo de errores adecuado (sin exposición de esquema)
- Gestión de secretos con variables de entorno
- Política CORS para acceso de frontend

## Configuración

### Desarrollo

Configuración del backend: `src/MyApp/appsettings.Development.json`
- Clave JWT: Clave de desarrollo predeterminada
- Base de datos: Archivo SQLite local (`TaskManagerPro.db`)
- Logging: Nivel de información
- Datos de prueba: Se cargan automáticamente al iniciar

### Producción

Establece variables de entorno:
```bash
JWT_KEY="tu-clave-secreta-de-256-bits"
ASPNETCORE_ENVIRONMENT="Production"
```

Base de datos: SQL Server u otra base de datos de producción vía cadena de conexión.

## Solución de Problemas

### Puerto Ya en Uso
```bash
# Backend (puerto 5141)
lsof -i :5141              # macOS/Linux
netstat -ano | findstr :5141  # Windows

# Frontend (puerto 4200)
lsof -i :4200
```

### Problemas de Base de Datos
Ver [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md) para solución de problemas detallada.

### Errores de Compilación
```bash
# Backend
cd src/MyApp
dotnet clean
dotnet build

# Frontend
cd frontend
rm -rf node_modules
npm install
npm start
```

## Notas sobre Organización de Archivos

⚠️ **Archivos de Base de Datos**: `TaskManagerPro.db` y archivos WAL (`*.db-shm`, `*.db-wal`) **NO se confirman en Git**.
- Se crean localmente cuando ejecutas la aplicación por primera vez
- Usa `TaskManagerPro.db.example` incluido en Git para un inicio rápido
- Cada desarrollador tiene su propia instancia de base de datos local
- Ver [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md) para detalles

## Contribuciones

1. Crea una rama de características: `git checkout -b feature/tu-caracteristica`
2. Realiza tus cambios
3. Ejecuta pruebas: `dotnet test`
4. Confirma con mensaje claro: `git commit -m "feat: tu característica"`
5. Envía y crea una Solicitud de Extracción

## Licencia

Este proyecto se proporciona tal cual para fines educativos y de demostración.

## Soporte

Para problemas, preguntas o sugerencias:
1. Consulta [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md) para preguntas sobre base de datos
2. Revisa las pruebas de integración para ejemplos de uso
3. Verifica las respuestas de la API para detalles de errores

---

**Construido con** ❤️ usando .NET 10 y Angular 21
