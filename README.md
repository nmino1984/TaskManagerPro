# TaskManagerPro

A comprehensive task management system with multi-level organization: tasks, subtasks, and milestones. Built with .NET 10 and Angular 21.

## Key Features

- **Task Management**: Create, track, and organize tasks with priority levels and status tracking
- **Subtasks**: Break down work into manageable pieces  
- **Milestones**: Define key checkpoints and deliverables
- **Multi-User**: JWT authentication with user isolation (multi-tenancy)
- **Export**: Milestones to JSON, XML, or iCalendar format

## Quick Start

### Option 1: Use Example Database (Fastest)
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
# In another terminal:
cd frontend && npm start
```

### Option 2: Create Fresh Database
```bash
cd src/MyApp && dotnet run
# In another terminal:
cd frontend && npm start
```

Access at `http://localhost:4200`

## Project Structure

| Component | Location | Purpose |
|-----------|----------|---------|
| **Backend** | `src/MyApp/` | .NET 10 RESTful API, Clean Architecture |
| **Frontend** | `frontend/` | Angular 21 UI, Material Design, Signals |
| **Database** | `src/MyApp/TaskManagerPro.db` | SQLite (dev) |
| **Tests** | `tests/` | 22+ integration tests |

## Documentation

### 🇬🇧 English
- **[Backend Setup](src/MyApp/README.md)** - .NET configuration, API endpoints, architecture
- **[Frontend Setup](frontend/README.md)** - Angular build, development server, components
- **[Database Setup](DATABASE_SETUP.md)** - Database initialization, seeding, management
- **[Testing Guide](tests/README.md)** - Running tests, test structure, patterns

### 🇪🇸 Español
- **[Setup del Backend](src/MyApp/README.es.md)** - Configuración de .NET, endpoints, arquitectura
- **[Setup del Frontend](frontend/README.es.md)** - Build de Angular, servidor, componentes
- **[Configuración de BD](DATABASE_SETUP.es.md)** - Inicialización, carga, gestión de datos
- **[Guía de Testing](tests/README.es.md)** - Ejecutar tests, estructura, patrones

## Technology Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | .NET 10, ASP.NET Core, Entity Framework Core, JWT |
| **Frontend** | Angular 21, Angular Material, TypeScript, Signals |
| **Database** | SQLite (development), SQL Server (production) |
| **Testing** | xUnit, FluentAssertions, In-Memory SQLite |

## License

Provided as-is for educational and demonstration purposes.

## Support

For detailed setup and troubleshooting:
- **Backend** → [Backend README](src/MyApp/README.md)
- **Frontend** → [Frontend README](frontend/README.md)  
- **Database** → [Database Setup](DATABASE_SETUP.md)
- **Tests** → [Testing Guide](tests/README.md)
