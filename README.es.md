# TaskManagerPro

Aplicación full-stack de gestión de tareas construida con **.NET 10** y **Angular 21**. Soporta múltiples usuarios con tareas, subtareas, hitos, comentarios, historial de cambios y notificaciones asíncronas en segundo plano.

---

## Características

- **Tareas** — CRUD completo con prioridades, estados, paginación, búsqueda y filtros
- **Subtareas** — Divide el trabajo; el progreso de la tarea se calcula automáticamente
- **Hitos** — Puntos de control con exportación a JSON, XML o iCalendar (.ics)
- **Asignación** — Asigna tareas a otros usuarios registrados
- **Comentarios** — Comentarios por tarea con notificaciones de menciones (@usuario)
- **Auditoría** — Historial completo de cambios en tareas, subtareas e hitos
- **Notificaciones** — Notificaciones en la app vía trabajos en segundo plano (Hangfire); badge con contador en la barra de navegación
- **Autenticación** — JWT con BCrypt; todos los datos están aislados por usuario (multi-tenancy)

---

## Inicio Rápido

**Requisitos:** .NET 10, Node.js 18+

```bash
# Opción 1: Usar la base de datos de ejemplo incluida (más rápido)
cd src/TaskManagerPro.API
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run

# Opción 2: Base de datos nueva (se crea automáticamente al arrancar)
cd src/TaskManagerPro.API && dotnet run
```

```bash
# Frontend (en otra terminal)
cd frontend
npm install
npm start
```

Abre `http://localhost:4200`, regístate y empieza a crear tareas.

---

## Estructura del Proyecto

```
TaskManagerPro/
├── src/
│   ├── TaskManagerPro.Domain/          # Entidades, enums — sin dependencias externas
│   ├── TaskManagerPro.Application/     # Servicios, DTOs, interfaces, validadores
│   ├── TaskManagerPro.Infrastructure/  # EF Core, repositorios, Hangfire, JWT, BCrypt
│   └── TaskManagerPro.API/             # Controladores, middleware, Program.cs
├── tests/
│   ├── TaskManagerPro.Integration.Tests/   # 64 tests de integración (SQLite in-memory real)
│   └── TaskManagerPro.Unit.Tests/          # 11 tests unitarios (lógica de dominio)
├── frontend/                           # App Angular 21 (componentes standalone, signals)
├── TaskManagerPro.db.example           # Base de datos de ejemplo para inicio rápido
└── DATABASE_SETUP.es.md                # Guía de configuración y gestión de la base de datos
```

**Arquitectura:** Clean Architecture — las capas apuntan hacia el Dominio. Application nunca referencia Infrastructure.

---

## Endpoints de la API

Todos los endpoints requieren `Authorization: Bearer {token}` excepto los de autenticación.

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Registrar usuario |
| POST | `/api/v1/auth/login` | Iniciar sesión |
| GET | `/api/v1/tasks` | Listar tareas (paginación, filtros, búsqueda) |
| POST | `/api/v1/tasks` | Crear tarea |
| GET | `/api/v1/tasks/{id}` | Obtener tarea |
| PUT | `/api/v1/tasks/{id}` | Actualizar tarea |
| DELETE | `/api/v1/tasks/{id}` | Eliminar tarea (soft delete) |
| PATCH | `/api/v1/tasks/{id}/assign` | Asignar tarea a otro usuario |
| GET | `/api/v1/tasks/{id}/history` | Historial de auditoría de la tarea |
| GET | `/api/v1/subtasks/bytask/{taskId}` | Listar subtareas |
| POST | `/api/v1/subtasks` | Crear subtarea |
| PUT | `/api/v1/subtasks/{id}` | Actualizar subtarea |
| DELETE | `/api/v1/subtasks/{id}` | Eliminar subtarea |
| GET | `/api/v1/subtasks/{id}/history` | Historial de auditoría de la subtarea |
| GET | `/api/v1/milestones/bytask/{taskId}` | Listar hitos |
| POST | `/api/v1/milestones` | Crear hito |
| PUT | `/api/v1/milestones/{id}` | Actualizar hito |
| DELETE | `/api/v1/milestones/{id}` | Eliminar hito |
| GET | `/api/v1/milestones/bytask/{taskId}/export/json` | Exportar hitos como JSON |
| GET | `/api/v1/milestones/bytask/{taskId}/export/xml` | Exportar hitos como XML |
| GET | `/api/v1/milestones/bytask/{taskId}/export/ical` | Exportar hitos como iCal |
| GET | `/api/v1/milestones/{id}/history` | Historial de auditoría del hito |
| GET | `/api/v1/tasks/{taskId}/comments` | Listar comentarios |
| POST | `/api/v1/tasks/{taskId}/comments` | Crear comentario |
| PUT | `/api/v1/comments/{id}` | Editar comentario |
| DELETE | `/api/v1/comments/{id}` | Eliminar comentario |
| GET | `/api/v1/notifications` | Listar notificaciones |
| GET | `/api/v1/notifications/unread` | Contador de no leídas |
| PATCH | `/api/v1/notifications/{id}/read` | Marcar una como leída |
| PATCH | `/api/v1/notifications/read-all` | Marcar todas como leídas |

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Backend | .NET 10, ASP.NET Core, Entity Framework Core |
| Frontend | Angular 21, Angular Material, TypeScript, Signals |
| Base de datos | SQLite (desarrollo), SQL Server (producción) |
| Trabajos en segundo plano | Hangfire con MemoryStorage (dev) / SqlServer (prod) |
| Autenticación | JWT Bearer, BCrypt |
| Validación | FluentValidation (backend), Angular Reactive Forms (frontend) |
| Testing | xUnit, FluentAssertions, SQLite in-memory |

---

## Ejecutar Tests

```bash
# Los 75 tests
dotnet test TaskManagerPro.sln

# Suite específica
dotnet test --filter "TasksControllerTests"
```

Esperado: **75 pasando** (11 unitarios + 64 integración)

---

## Configuración

**Desarrollo** — los valores por defecto están en `src/TaskManagerPro.API/appsettings.Development.json`. No requiere configuración adicional.

**Producción** — define las variables de entorno:
```bash
JWT_KEY=tu-clave-secreta-de-256-bits
ConnectionStrings__DefaultConnection=Server=...;Database=TaskManagerPro;...
ASPNETCORE_ENVIRONMENT=Production
```

Los orígenes CORS se configuran en `appsettings.Development.json` bajo `"CorsOrigins"`. Si Angular arranca en un puerto diferente, agrégalo ahí.

Panel de Hangfire: `http://localhost:5141/hangfire` (solo en desarrollo)

---

## Base de Datos

Ver [DATABASE_SETUP.es.md](DATABASE_SETUP.es.md) para instrucciones de inicialización, carga de datos, migraciones y reset.

---

## Licencia

Proporcionado tal cual para fines educativos y de demostración.
