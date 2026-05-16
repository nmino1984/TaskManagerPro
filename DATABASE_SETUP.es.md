# TaskManagerPro - Guía de Configuración de Base de Datos

## Descripción General

TaskManagerPro utiliza SQLite para desarrollo y pruebas locales. El archivo de base de datos (`TaskManagerPro.db`) **NO se confirma en Git** para evitar conflictos y contaminación de datos.

## Configuración Inicial

### Opción A: Usar la Base de Datos de Ejemplo (Recomendado para Pruebas)

Una base de datos preconfigurada se incluye en el repositorio: `TaskManagerPro.db.example`

**Para usarla:**

#### Linux/macOS:
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
```

#### Windows (PowerShell):
```powershell
cd src/MyApp
Copy-Item ../../TaskManagerPro.db.example -Destination ./TaskManagerPro.db
dotnet run
```

**¿Qué hay en la base de datos de ejemplo:**
- Todas las tablas creadas y listas para usar
- Esquema completo con índices para rendimiento
- Opcional: Datos de prueba para pruebas inmediatas

Esta es la **forma más rápida de comenzar** - la base de datos está lista inmediatamente.

### Opción B: Auto-Crear Base de Datos Fresca

Si deseas una base de datos completamente nueva, la aplicación puede crear una automáticamente:

```bash
cd src/MyApp
dotnet run
```

La aplicación:
- Creará `TaskManagerPro.db` automáticamente vía migraciones de Entity Framework
- Inicializará todas las tablas (Users, MyTasks, SubTasks, Milestones, etc.)
- Creará índices para optimización del rendimiento
- Cargará datos de prueba (solo en modo de desarrollo)

**Esto toma 2-3 segundos más que la Opción A.**

## Ubicación de la Base de Datos

La base de datos SQLite se encuentra en:
```
src/MyApp/TaskManagerPro.db
```

Archivos WAL (Write-Ahead Logging) asociados:
- `src/MyApp/TaskManagerPro.db-shm` - Archivo de memoria compartida
- `src/MyApp/TaskManagerPro.db-wal` - Registro de escritura anticipada

**Todos estos archivos son ignorados por Git** (.gitignore) y no deben ser confirmados.

## Esquema de Base de Datos

Entity Framework Core maneja la gestión de esquema a través de **migraciones**:

```bash
# Ver migraciones
ls src/MyApp/Infrastructure/Migrations/

# Crear nueva migración (si modificas modelos)
cd src/MyApp
dotnet ef migrations add NombreDeTuMigracion

# Aplicar migraciones
dotnet ef database update
```

**Nunca edites manualmente el esquema de la base de datos** - usa migraciones de EF Core en su lugar.

## Agregar Datos a la Base de Datos

### Método 1: A través de la Interfaz de Usuario de la Aplicación

1. Inicia el backend: `dotnet run` (en `src/MyApp`)
2. Inicia el frontend: `npm start` (en `frontend`)
3. Abre `http://localhost:4200`
4. Registra un nuevo usuario
5. Haz clic en "Agregar Tarea" y crea tareas manualmente
6. Agrega subtareas e hitos a través de la interfaz de usuario

**Ventajas:**
- Interfaz visual
- Ver cambios en tiempo real
- Entender el flujo de trabajo de la aplicación

### Método 2: A través de la API

Usa curl o Postman para llamar directamente a la API:

#### Registrar un usuario:
```bash
curl -X POST http://localhost:5141/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "usuarioprueba",
    "password": "ContraseñaPrueba123!"
  }'
```

**Guarda el `token` devuelto para las siguientes solicitudes.**

#### Crear una tarea:
```bash
curl -X POST http://localhost:5141/api/v1/tasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "title": "Mi Primera Tarea",
    "description": "Descripción de la tarea",
    "startDate": "2026-05-16T00:00:00Z",
    "endDate": "2026-06-16T00:00:00Z",
    "priority": "High"
  }'
```

**Guarda el `myTaskId` devuelto.**

#### Crear una subtarea:
```bash
curl -X POST http://localhost:5141/api/v1/subtasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "taskId": ID_TAREA_AQUI,
    "description": "Descripción de subtarea"
  }'
```

#### Crear un hito:
```bash
curl -X POST http://localhost:5141/api/v1/milestones \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "taskId": ID_TAREA_AQUI,
    "title": "Título del Hito",
    "description": "Punto de control importante",
    "targetDate": "2026-06-01T00:00:00Z",
    "status": "Pending"
  }'
```

### Método 3: Usar el Script de Datos de Prueba

La aplicación carga automáticamente datos de prueba en **modo de Desarrollo**:

```bash
# Asegúrate de estar en ambiente de Desarrollo
set ASPNETCORE_ENVIRONMENT=Development  # Windows
export ASPNETCORE_ENVIRONMENT=Development  # Linux/macOS

cd src/MyApp
dotnet run
```

La clase `DataSeeder` poblará automáticamente la base de datos con:
- Tareas de muestra con varios estados
- Subtareas de muestra
- Hitos de muestra

**Ver**: `src/MyApp/Infrastructure/Data/DataSeeder.cs`

---

## Limpiar Datos

### Método 1: Eliminar Todos los Datos de Usuario (Mantener Esquema)

**ADVERTENCIA: Esto elimina todos tus datos. Haz una copia de seguridad primero si es necesario.**

No hay un punto de acceso de API integrado para eliminar todos los datos, así que usa la base de datos directamente:

#### Vía CLI de SQLite:
```bash
# Conectar a la base de datos
sqlite3 src/MyApp/TaskManagerPro.db

# Eliminar todos los datos (mantiene tablas/esquema)
DELETE FROM Milestones;
DELETE FROM SubTasks;
DELETE FROM MyTasks;
DELETE FROM Users;

# Verificar (debe mostrar vacío)
SELECT COUNT(*) FROM Users;

# Salir
.exit
```

#### Vía PowerShell (Windows):
```powershell
# Requiere sqlite3.exe instalado
sqlite3 "src/MyApp/TaskManagerPro.db" "DELETE FROM Milestones; DELETE FROM SubTasks; DELETE FROM MyTasks; DELETE FROM Users;"
```

### Método 2: Eliminar Datos de Usuario Específico

Elimina solo las tareas de un usuario (más específico):

```bash
sqlite3 src/MyApp/TaskManagerPro.db
DELETE FROM MyTasks WHERE UserId = 'id-usuario-aqui';
.exit
```

### Método 3: Eliminación Suave (Bandera de Eliminación Suave)

Las tareas utilizan **eliminación suave** (no se eliminan físicamente). Para ver tareas eliminadas:

```bash
sqlite3 src/MyApp/TaskManagerPro.db
SELECT * FROM MyTasks WHERE IsDeleted = 1;
.exit
```

Para eliminar permanentemente registros eliminados suavemente:

```bash
sqlite3 src/MyApp/TaskManagerPro.db
DELETE FROM MyTasks WHERE IsDeleted = 1;
.exit
```

### Método 4: Reinicio Completo de Base de Datos

Para comenzar completamente de nuevo:

#### Linux/macOS:
```bash
cd src/MyApp
rm TaskManagerPro.db TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null
dotnet run
```

**Esto:**
1. Elimina la base de datos actual
2. Crea una fresca vía migraciones
3. Carga datos de prueba automáticamente (modo de desarrollo)

#### Windows (PowerShell):
```powershell
cd src/MyApp
Remove-Item TaskManagerPro.db, TaskManagerPro.db-shm, TaskManagerPro.db-wal -ErrorAction SilentlyContinue
dotnet run
```

#### O restaura desde el ejemplo:
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
rm TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null  # Elimina archivos WAL
dotnet run
```

---

## Restablecimiento de Base de Datos

Para comenzar con una base de datos nueva:

### Opción A: Eliminar y Regenerar (Linux/macOS)
```bash
cd src/MyApp
rm TaskManagerPro.db TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null
dotnet run
```

### Opción B: Eliminar y Regenerar (Windows PowerShell)
```powershell
cd src/MyApp
Remove-Item TaskManagerPro.db, TaskManagerPro.db-shm, TaskManagerPro.db-wal -ErrorAction SilentlyContinue
dotnet run
```

La base de datos se recreará automáticamente con migraciones y datos de prueba.

## Desarrollo vs Producción

### Desarrollo (SQLite)
- **Ubicación**: `src/MyApp/TaskManagerPro.db`
- **Auto-migración**: Sí (automático al iniciar)
- **Auto-sembrado**: Sí (solo en ambiente de Desarrollo)
- **Recrear**: Elimina archivo .db y reinicia

### Producción (SQL Server/Otro)
- Configura cadena de conexión en variables de entorno
- Ejecuta migraciones vía: `dotnet ef database update`
- Usa procedimientos de copia de seguridad/restauración de base de datos de producción

## Solución de Problemas

### La base de datos está bloqueada
**Problema**: Error "base de datos está bloqueada"
**Solución**: 
- Cierra todas las conexiones a la base de datos
- Elimina archivos `.db-shm` y `.db-wal`
- Reinicia la aplicación

### Las migraciones están fuera de sincronización
**Problema**: Errores de migración al iniciar
**Solución**:
```bash
cd src/MyApp
dotnet ef database drop -f
dotnet ef database update
dotnet run
```

### No se puede conectar a la base de datos
**Problema**: Conexión rechazada
**Solución**:
- Verifica que `appsettings.json` tenga la cadena de conexión correcta
- Asegúrate de tener permisos de escritura al directorio `src/MyApp/`
- Verifica que ningún otro proceso esté bloqueando el archivo de base de datos

## Variables de Entorno

No se necesitan variables de entorno de base de datos para desarrollo - SQLite usa la ruta de archivo local configurada en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TaskManagerPro.db"
  }
}
```

Para producción, establece:
```bash
export ConnectionString="tu-cadena-de-conexion-de-produccion"
```

## Pruebas de Integración

Las pruebas de integración utilizan **SQLite en memoria** (`:memory:`) y no utilizan la base de datos basada en archivos:

```csharp
// CustomWebApplicationFactory.cs
var connection = new SqliteConnection("Data Source=:memory:");
```

Cada ejecución de prueba obtiene una base de datos aislada y fresca. No se necesita limpieza.

## Próximos Pasos

1. ✅ Ejecuta backend: `dotnet run` (base de datos creada automáticamente)
2. ✅ Ejecuta frontend: `npm start` (en directorio `frontend/`)
3. ✅ Accede en: `http://localhost:4200`
4. ✅ Registra un usuario de prueba o usa datos de prueba
5. ✅ Crea tareas, subtareas e hitos

Para preguntas sobre la configuración de la base de datos, ver el [README.es.md](README.es.md) principal.
