# TaskManagerPro - Pruebas de Integración

Suite integral de pruebas de integración para el backend de TaskManagerPro. Las pruebas verifican la funcionalidad de la API, autorización, operaciones de base de datos y lógica de negocio sin mockear la base de datos.

## Inicio Rápido

### Ejecutar Todas las Pruebas

```bash
# Desde la raíz del proyecto
dotnet test
```

Resultado esperado: ✅ **22/22 pruebas pasando**

### Ejecutar Archivo de Prueba Específico

```bash
dotnet test tests/MyApp.Tests.Integration/Controllers/TasksControllerTests.cs
```

### Ejecutar Pruebas con Salida Detallada

```bash
dotnet test --verbosity detailed
```

### Ejecutar Pruebas con Cobertura

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=json
```

## Estructura de Pruebas

```
tests/MyApp.Tests.Integration/
├── Controllers/              # Pruebas de puntos de acceso de API
│   ├── AuthControllerTests.cs
│   ├── TasksControllerTests.cs
│   ├── SubTasksControllerTests.cs
│   └── MilestonesControllerTests.cs
├── Infrastructure/          # Configuración y helpers de pruebas
│   ├── CustomWebApplicationFactory.cs    # Factory del servidor de pruebas
│   └── IntegrationTestBase.cs           # Clase base para pruebas
└── Features/                # Pruebas específicas de características (si existen)
```

## Arquitectura de Pruebas

### CustomWebApplicationFactory

Crea una aplicación web específica para pruebas con:
- **Base de Datos SQLite en Memoria**: Cada ejecución de prueba obtiene una base de datos aislada y fresca (`:memory:`)
- **Ambiente Development**: Usa configuración de desarrollo y datos de semilla
- **Configuración JWT**: Pre-configurada con clave JWT de prueba
- **Sin Dependencias Reales**: Todos los servicios externos están configurados para pruebas

```csharp
// Ejemplo de uso
var factory = new CustomWebApplicationFactory();
var client = factory.CreateClient();
```

### Base de Pruebas de Integración

Todas las pruebas heredan de `IntegrationTestBase` que proporciona:
- Configuración automática de factory y client
- Limpieza después de cada prueba
- Datos comunes de prueba y utilidades

## Patrones de Prueba

### Pruebas de Autenticación

```csharp
[Fact]
public async Task Register_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var request = new { username = "testuser", password = "Password123!" };
    
    // Act
    var response = await _client.PostAsync("/api/v1/auth/register", 
        JsonContent.Create(request));
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### Puntos de Acceso Autorizados

Las pruebas verifican que los puntos de acceso requieren autenticación JWT:

```csharp
[Fact]
public async Task GetTasks_WithoutToken_Returns401()
{
    // Act - sin token
    var response = await _client.GetAsync("/api/v1/tasks");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
}
```

### Pruebas de Aislamiento de Usuarios

Las pruebas verifican que los usuarios solo ven sus propios datos (multi-inquilino):

```csharp
[Fact]
public async Task GetTasks_ReturnsOnlyCurrentUserTasks()
{
    // Arrange - crear dos usuarios
    var user1Token = await RegisterUser("user1", "pass");
    var user2Token = await RegisterUser("user2", "pass");
    
    // Crear tarea como user1
    var taskId = await CreateTask(user1Token, "Task 1");
    
    // Act - user2 intenta obtener la tarea de user1
    var response = await GetTask(user2Token, taskId);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
}
```

### Pruebas de Operaciones CRUD

Pruebas para operaciones Crear, Leer, Actualizar, Eliminar:

```csharp
[Fact]
public async Task CreateTask_WithValidData_ReturnsCreatedTask()
{
    // Prueba que datos válidos crean tarea exitosamente
}

[Fact]
public async Task UpdateTask_WithInvalidData_ReturnsBadRequest()
{
    // Prueba validación en actualización
}

[Fact]
public async Task DeleteTask_RemovesSoftDeleteFlag()
{
    // Prueba que eliminación suave funciona correctamente
}
```

## Cobertura de Pruebas

La suite de pruebas cubre:

### Autenticación (3 pruebas)
- ✅ Registro de usuario con validación
- ✅ Login de usuario con credenciales válidas/inválidas
- ✅ Generación y validación de token JWT

### Tareas (7 pruebas)
- ✅ Obtener tareas con paginación
- ✅ Crear tarea con validación
- ✅ Actualizar tarea con autorización
- ✅ Eliminar tarea (eliminación suave)
- ✅ Filtrado y ordenamiento
- ✅ Aislamiento de usuarios

### SubTareas (5 pruebas)
- ✅ Obtener subtareas de una tarea
- ✅ Crear subtarea con validación
- ✅ Actualizar y eliminar subtareas
- ✅ Autorización de usuario

### Hitos (7 pruebas)
- ✅ Obtener hitos de una tarea
- ✅ Crear hito con validación
- ✅ Actualizar y eliminar hitos
- ✅ Exportar a JSON, XML, iCal
- ✅ Autorización de usuario

## Base de Datos en Pruebas

Las pruebas usan **SQLite en memoria** (`:memory:`):
- Base de datos fresca e aislada para cada prueba
- Sin necesidad de limpieza entre pruebas
- Aplicación automática de migraciones
- Datos de semilla poblados en ambiente Development

```csharp
// Configuración de base de datos en factory
var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection)
    .Options;
```

## Escribir Nuevas Pruebas

### 1. Crear Clase de Prueba

```csharp
public class YourFeatureTests : IntegrationTestBase
{
    [Fact]
    public async Task YourTest_WithScenario_ExpectedResult()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
}
```

### 2. Registrar Usuario y Obtener Token

```csharp
var response = await _client.PostAsync("/api/v1/auth/register",
    JsonContent.Create(new { username = "user", password = "Pass123!" }));
var token = // extraer de respuesta
_client.DefaultRequestHeaders.Authorization = 
    new("Bearer", token);
```

### 3. Hacer Llamada a API

```csharp
var response = await _client.GetAsync("/api/v1/tasks");
```

### 4. Afirmar Resultados

```csharp
response.StatusCode.Should().Be(HttpStatusCode.OK);
var content = await response.Content.ReadAsStringAsync();
var data = JsonSerializer.Deserialize<TaskListResponse>(content);
data.Should().NotBeNull();
```

## Mejores Prácticas

✅ **Aislamiento**: Cada prueba es independiente  
✅ **Claridad**: Los nombres de pruebas describen qué prueban  
✅ **Patrón AAA**: Arrange → Act → Assert  
✅ **Sin Mocking**: Usa base de datos real en memoria  
✅ **Rápido**: Todas las 22 pruebas se completan en < 2 segundos  
✅ **Determinístico**: Mismos resultados cada ejecución  

## Solución de Problemas

### Las Pruebas Fallan con "JWT Key Not Configured"

Las pruebas se ejecutan con ambiente Development que proporciona una clave codificada. Si esto falla, asegúrate:
```bash
dotnet test --environment Development
```

### Las Pruebas Pasan Localmente pero Fallan en CI

Asegúrate de:
- .NET 10 instalado
- Todas las migraciones aplicadas
- Variables de entorno de Development configuradas
- Ningún proceso existente usando la base de datos de prueba

### Una Sola Prueba Falla

Reinicia el corredor de pruebas:
```bash
dotnet test --no-build
```

## Documentación Relacionada

- **Setup del Backend**: [Backend README](../src/MyApp/README.md)
- **Setup de Base de Datos**: [Guía de Base de Datos](../DATABASE_SETUP.md)
- **Descripción General del Proyecto**: [README del Proyecto](../README.md)

## Frameworks de Prueba

- **xUnit**: Framework de pruebas
- **FluentAssertions**: Aserciones legibles
- **Moq**: (No se usa - las pruebas usan base de datos real)
- **SQLite En Memoria**: Base de datos de prueba

## Ejecutar Pruebas en Desarrollo

Durante el desarrollo, ejecuta pruebas frecuentemente:

```bash
# Ejecutar pruebas después de cada cambio
dotnet test --watch

# O ejecutar clase de prueba específica
dotnet test --filter "TasksControllerTests"
```

## Próximos Pasos

- Todas las 22 pruebas pasando ✅
- Listo para despliegue a producción
- Monitorear rendimiento de pruebas en CI/CD
