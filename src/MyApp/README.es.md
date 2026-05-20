# TaskManagerPro - API Backend

Una API RESTful para gestión de tareas con organización multinivel: tareas, subtareas e hitos. Construida con .NET 10 y Entity Framework Core.

## Inicio Rápido

### Requisitos Previos
- **.NET 10** o superior
- **SQLite** (incluido con .NET)

### Configuración

**Opción 1: Usar Base de Datos de Ejemplo (La Más Rápida)**
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
```

**Opción 2: Crear Base de Datos Fresca**
```bash
cd src/MyApp
dotnet run
```

La aplicación automáticamente:
- Creará la base de datos SQLite mediante migraciones de Entity Framework
- Inicializará todas las tablas (Users, MyTasks, SubTasks, Milestones)
- Creará índices de rendimiento
- Cargará datos de prueba (solo en ambiente Development)

Accede a la API en `http://localhost:5141`

## Comandos de Desarrollo

```bash
# Iniciar el servidor backend
dotnet run

# Compilar sin ejecutar
dotnet build

# Ejecutar todos los tests
dotnet test

# Crear una nueva migración
dotnet ef migrations add NombreDeTuMigracion

# Aplicar migraciones a la base de datos
dotnet ef database update

# Reiniciar la base de datos
dotnet ef database drop -f
dotnet ef database update
```

## Puntos de Acceso de la API

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

### Notificaciones (Asincrónicas)
- `GET /api/v1/notifications` - Obtener todas las notificaciones del usuario actual
- `GET /api/v1/notifications/unread` - Obtener cantidad de notificaciones no leídas
- `PATCH /api/v1/notifications/{id}/read` - Marcar una notificación como leída
- `PATCH /api/v1/notifications/read-all` - Marcar todas las notificaciones como leídas

**Todos los puntos de acceso requieren autenticación JWT Bearer** (excepto `/auth/register` y `/auth/login`)

## Trabajos en Segundo Plano (Hangfire)

Las notificaciones asincrónicas se ejecutan mediante trabajos en segundo plano de Hangfire:
- **Task Created**: Se genera una notificación cuando se crea una nueva tarea
- **Task Completed**: Se genera una notificación cuando el estado de una tarea cambia a Completed
- **Task Overdue Check**: Trabajo recurrente (cada hora) para detectar y notificar tareas vencidas

**Panel de Control**: Accede a `http://localhost:5141/hangfire` en desarrollo para monitorear trabajos.

## Estructura del Proyecto

```
src/MyApp/
├── Domain/                    # Entidades del núcleo
│   ├── Entities/
│   └── Enums/
├── Application/               # Lógica de negocio
│   ├── Services/
│   ├── Interfaces/
│   ├── DTOs/
│   └── Validators/
├── Infrastructure/            # Capa de acceso a datos
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── DataSeeder.cs
│   └── Migrations/
├── API/                       # Controladores y middleware
│   ├── Controllers/
│   ├── Middleware/
│   └── Validators/
├── Program.cs                 # Inicio de la aplicación
└── appsettings*.json          # Configuración
```

## Principios de Arquitectura

- **Clean Architecture**: Separación de responsabilidades (Domain → Application → Infrastructure → API)
- **Inyección de Dependencias**: Todos los servicios registrados en Program.cs
- **DTOs**: Objetos de solicitud/respuesta para contratos de API
- **AutoMapper**: Mapeo de DTOs a entidades de dominio
- **Fluent Validation**: Reglas de validación del lado del servidor
- **Entity Framework Core**: ORM para operaciones de base de datos
- **Soft Delete**: Eliminación lógica con flag `IsDeleted`

## Autenticación y Seguridad

- **Tokens JWT Bearer**: Autenticación segura basada en tokens
- **Hash de Contraseñas**: BCrypt para almacenamiento seguro
- **Aislamiento de Usuarios**: Arquitectura multi-inquilino - los usuarios solo ven sus propios datos
- **Autorización**: Todas las operaciones validan la propiedad del usuario
- **CORS**: Configurado para el origen del frontend

### Variables de Entorno

Requeridas para producción:
```bash
JWT_KEY="tu-clave-secreta-de-256-bits"
ASPNETCORE_ENVIRONMENT="Production"
```

Development usa valores por defecto codificados en `appsettings.Development.json`.

## Base de Datos

La base de datos usa SQLite para desarrollo con migraciones de Entity Framework Core.

### Ubicación de la Base de Datos
```
src/MyApp/TaskManagerPro.db
src/MyApp/TaskManagerPro.db-shm  # Archivo de memoria compartida WAL
src/MyApp/TaskManagerPro.db-wal  # Registro de escritura anticipada
```

Estos archivos están ignorados por Git y se crean localmente cuando se ejecuta la aplicación.

### Gestión del Esquema

Nunca edites manualmente el esquema de la base de datos. Usa migraciones de Entity Framework:

```bash
# Ver migraciones existentes
ls src/MyApp/Infrastructure/Migrations/

# Crear nueva migración después de modificar entidades
dotnet ef migrations add NombreDescriptivo

# Aplicar migraciones
dotnet ef database update
```

## Pruebas

### Ejecutar Pruebas de Integración
```bash
dotnet test
```

Esperado: **22/22 pruebas pasando**

### Arquitectura de Pruebas
- **Framework**: xUnit
- **Aserciones**: FluentAssertions
- **Base de Datos**: SQLite en memoria (`:memory:`)
- **Factory**: CustomWebApplicationFactory para configuración de pruebas

Las pruebas usan una base de datos aislada en memoria - no se necesita limpieza.

## Carga de Datos

En ambiente Development, la clase `DataSeeder` poblará automáticamente la base de datos con datos de ejemplo:
- Tareas de demostración con varias prioridades y estados
- Subtareas e hitos
- Datos de ejemplo para pruebas

Para modificar datos de semilla, edita `src/MyApp/Infrastructure/Data/DataSeeder.cs` y reinicia.

## Configuración

### Development (`appsettings.Development.json`)
- Base de datos SQLite archivo local
- Auto-migraciones habilitadas
- Auto-carga de datos habilitada
- Nivel de logging de desarrollo
- Clave JWT codificada

### Producción
- Establece variable de entorno `JWT_KEY`
- Configura cadena de conexión de base de datos de producción
- Establece `ASPNETCORE_ENVIRONMENT=Production`

## Solución de Problemas

### Puerto Ya en Uso
```bash
# Windows
netstat -ano | findstr :5141

# macOS/Linux
lsof -i :5141
```

### Base de Datos Bloqueada
```bash
# Elimina archivos WAL e reinicia
rm src/MyApp/TaskManagerPro.db-shm src/MyApp/TaskManagerPro.db-wal
dotnet run
```

### Migraciones Fuera de Sincronización
```bash
dotnet ef database drop -f
dotnet ef database update
dotnet run
```

### Compilación Limpia
```bash
dotnet clean
dotnet build
dotnet run
```

## Dependencias

Paquetes NuGet clave:
- **Microsoft.EntityFrameworkCore**: Framework ORM
- **Microsoft.IdentityModel.Tokens**: Manejo de tokens JWT
- **BCrypt.Net**: Hash de contraseñas
- **FluentValidation**: Validación del lado del servidor
- **Ical.Net**: Formato de exportación iCalendar
- **Serilog**: Logging estructurado
- **AutoMapper**: Mapeo de objetos
- **Hangfire.Core**: Programación de trabajos en segundo plano
- **Hangfire.AspNetCore**: Integración con ASP.NET Core
- **Hangfire.MemoryStorage**: Almacenamiento en memoria (desarrollo)
- **Hangfire.SqlServer**: Almacenamiento en SQL Server (producción)
- **xUnit**: Framework de pruebas
- **FluentAssertions**: Aserciones de pruebas

## Documentación Relacionada

- **Configuración del Frontend**: Ver `frontend/README.md`
- **Gestión de Base de Datos**: Ver `DATABASE_SETUP.md` en la raíz del proyecto
- **Descripción General del Proyecto**: Ver `README.md` en la raíz del proyecto

## Soporte

Para preguntas sobre la base de datos, consulta `DATABASE_SETUP.md` para procedimientos detallados de configuración, carga de datos y limpieza.
