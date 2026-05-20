# TaskManagerPro - Aplicación Frontend

Una interfaz de gestión de tareas responsiva y moderna construida con Angular 21 y Angular Material. Presenta gestión de estado basada en signals, formularios reactivos y componentes Material Design.

## Inicio Rápido

### Requisitos Previos
- **Node.js 18+** y npm
- **Angular 21** CLI (instalado vía npm)

### Configuración

```bash
cd frontend
npm install
npm start
```

La aplicación se ejecutará en `http://localhost:4200` y se recargará automáticamente en los cambios.

## Comandos de Desarrollo

```bash
# Iniciar servidor de desarrollo
npm start

# Compilar para producción
npm run build

# Ejecutar compilación de producción localmente
npm run serve:ssr

# Ejecutar linter
npm run lint

# Formatear código
npm run format

# Ejecutar pruebas unitarias (si está configurado)
npm test

# Solo compilar (sin servir)
ng build
```

## Estructura del Proyecto

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── models/              # Interfaces y enums de TypeScript
│   │   │   ├── services/            # Servicios HTTP (tareas, hitos, autenticación)
│   │   │   ├── interceptors/        # Interceptores HTTP (JWT, manejo de errores)
│   │   │   └── guards/              # Guardias de rutas (autenticación)
│   │   ├── features/
│   │   │   ├── auth/                # Componentes de Login/Registro
│   │   │   ├── tasks/               # Componentes de lista y detalle de tareas
│   │   │   ├── task-form/           # Formulario de creación/edición de tareas
│   │   │   ├── subtasks/            # Componentes de subtareas
│   │   │   └── milestones/          # Componentes de hitos
│   │   ├── shared/
│   │   │   ├── components/          # Componentes UI reutilizables
│   │   │   └── pipes/               # Pipes personalizados
│   │   ├── app.component.*          # Componente raíz
│   │   ├── app.routes.ts            # Configuración de rutas
│   │   └── app.config.ts            # Configuración de la aplicación
│   ├── environments/                # Configuración de entornos (dev, prod)
│   ├── index.html
│   ├── styles.scss
│   └── main.ts
├── angular.json                     # Configuración de Angular CLI
├── tsconfig.json                    # Configuración de TypeScript
├── package.json                     # Dependencias
└── README.es.md
```

## Arquitectura

### Gestión de Estado
- **Angular Signals**: Gestión de estado reactivo para signals de componentes
- **Sin RxJS Subjects**: Gestión de estado limpia y predecible
- **Change Detection OnPush**: Optimización de rendimiento en toda la aplicación

### Servicios
Todos los servicios siguen el mismo patrón:
- Principio de responsabilidad única
- HttpClient para comunicación con API
- Tipos de retorno Observable (o basados en Signals para polling)
- Manejo de errores vía interceptores

Servicios clave:
- **AuthService**: Autenticación de usuarios y gestión de tokens JWT
- **TaskService**: CRUD y consultas de tareas
- **SubTaskService**: Gestión de subtareas
- **MilestoneService**: Operaciones de hitos y exportaciones
- **NotificationService**: Polling de notificaciones, lectura y gestión de estado (basado en Signals)

### Componentes
- **Componentes Standalone**: Sin dependencias de NgModule
- **Formularios Reactivos**: Controles de formulario tipados con validación
- **Material Design**: UI consistente con Angular Material
- **MatDialog**: Modales para operaciones de crear/editar

### Capa HTTP
- **JWT Interceptor**: Añade automáticamente el token Bearer a las solicitudes
- **Error Interceptor**: Manejo de errores centralizado con notificaciones snackbar
- **URLs basadas en entorno**: Puntos de acceso de API para desarrollo vs producción

## Características Clave

### Autenticación
- Registro e inicio de sesión de usuarios
- Almacenamiento de tokens JWT (localStorage)
- Inyección automática de tokens en solicitudes HTTP
- Funcionalidad de inicio/cierre de sesión

### Tareas
- Operaciones de crear, leer, actualizar, eliminar (CRUD)
- Paginación y filtrado
- Niveles de prioridad (Bajo, Medio, Alto)
- Seguimiento de estado (No iniciada, En progreso, Completada, Retrasada)
- Soporte de eliminación suave

### SubTareas
- Desglosar tareas en piezas manejables
- Rastrear estado de completación
- Cálculo automático de progreso
- Operaciones CRUD completas

### Hitos
- Definir puntos de control del proyecto
- Rastrear estado del hito
- Exportar a múltiples formatos:
  - **JSON**: Para integración de datos
  - **XML**: Para integración de sistemas
  - **iCalendar**: Para aplicaciones de calendario (Google Calendar, Outlook, Apple Calendar)

### Notificaciones
- **Badge en Tiempo Real**: Icono en la barra de navegación con contador de no leídas
- **Menú Desplegable de Notificaciones**: Haz clic en el icono de campana para ver notificaciones recientes
- **Actualizaciones Asincrónicas**: Notificaciones consultadas cada 30 segundos
- **Marcar como Leído**: Marcado individual o en lote de notificaciones
- **Disparadores de Eventos**: Notificaciones para creación de tareas, completación y alertas de vencimiento

## Componentes Material Design Utilizados

- **MatTable**: Visualización de datos y paginación
- **MatDialog**: Diálogos modales para crear/editar
- **MatForm**: Entradas de formulario y validación
- **MatButton**: Botones de acción
- **MatIcon**: Iconos en toda la aplicación
- **MatCard**: Diseños de tarjeta
- **MatDatepicker**: Selección de fecha
- **MatSelect**: Selección desplegable
- **MatSnackBar**: Notificaciones tipo toast
- **MatBadge**: Badge de contador de notificaciones en barra de navegación
- **MatMenu**: Menú desplegable para notificaciones
- **MatToolbar**: Barra de encabezado y navegación
- **MatTooltip**: Tooltips al pasar el ratón

## Flujo de Trabajo de Desarrollo

### Crear un Nuevo Componente

```bash
ng generate component features/tu-caracteristica/tu-componente
```

El componente será:
- Standalone (sin NgModule)
- Usando detección de cambios OnPush
- Con hoja de estilos SCSS

### Agregar un Nuevo Servicio

```bash
ng generate service core/services/tu-servicio
```

Los servicios deben:
- Depender de HttpClient
- Devolver Observables
- Manejar comunicación con API

### Mejores Prácticas de Formularios

```typescript
// Usar FormGroup con controles tipados
form = new FormGroup({
  title: new FormControl('', Validators.required),
  description: new FormControl('', Validators.required),
});

// Acceder a valores de control tipados
this.form.value.title
```

## Configuración de Entorno

### Desarrollo (`environment.ts`)
```typescript
apiUrl: 'http://localhost:5141/api/v1'
production: false
```

### Producción (`environment.prod.ts`)
```typescript
apiUrl: '/api/v1'
production: true
```

## Manejo de Errores

Los errores se manejan centralmente vía `error.interceptor.ts`:
- Errores de red → Notificación snackbar
- 401/403 No autorizado → Redirigir a inicio de sesión
- Errores 4xx/5xx → Mostrar mensaje de error
- Reintento automático para ciertos errores

## Pruebas

Patrón de prueba de componentes:
```typescript
it('should create', () => {
  expect(component).toBeTruthy();
});

it('should load tasks', fakeAsync(() => {
  // Implementación de prueba
}));
```

## Optimización de Rendimiento

- **OnPush Change Detection**: Solo actualiza cuando las entradas cambian
- **Lazy Loading**: Características cargadas bajo demanda vía routing
- **Signals**: Menos re-renders comparado con RxJS
- **Production Build**: Minificación, tree-shaking, bundling

## Salida de Compilación

```bash
npm run build
```

Produce compilación de producción optimizada en directorio `dist/`:
- JavaScript minificado
- CSS optimizado
- Bundles de características cargadas perezosamente
- Source maps (si está habilitado)

## Compatibilidad de Navegadores

- Chrome/Chromium (última versión)
- Firefox (última versión)
- Safari (última versión)
- Edge (última versión)

## Estilos

- **SCSS**: Para estilos de componentes y globales
- **CSS Variables**: Para tematización
- **Material Theme**: Construido sobre el sistema Material Design
- **Responsivo**: Diseños para móvil, tablet y escritorio

## Interceptores HTTP

### JWT Interceptor
```typescript
// Añade automáticamente token a todas las solicitudes
Authorization: 'Bearer {token}'
```

### Error Interceptor
```typescript
// Captura y maneja errores centralmente
// Muestra notificaciones snackbar
// Registra errores para depuración
```

## Sistema de Notificaciones

### NotificationService
El `NotificationService` gestiona notificaciones en tiempo real con:
- **Estado basado en Signals**: La signal `notifications` contiene la lista, `unreadCount` contiene el contador
- **Auto-polling**: Llama al backend cada 30 segundos
- **Carga Perezosa**: Comienza a hacer polling en login de usuario, se detiene en logout
- **Marcar como Leído**: Operaciones individuales o en lote
- **Manejo de Errores**: Fallos silenciosos con registro en consola

### Integración en la Barra de Navegación
- **Icono de Campana de Notificaciones**: Se muestra en el encabezado de la barra de navegación
- **Badge de No Leídas**: Badge rojo que muestra el contador de no leídas (oculto si es 0)
- **Menú Desplegable**: Haz clic en la campana para abrir el panel de notificaciones
- **Notificaciones Recientes**: Muestra hasta 10 más recientes
- **Marcar Todo**: Botón para marcar todo como leído de una vez

## Solución de Problemas

### Puerto Ya en Uso
```bash
# macOS/Linux
lsof -i :4200

# Windows
netstat -ano | findstr :4200
```

### Dependencias Sin Instalar
```bash
rm -rf node_modules package-lock.json
npm install
```

### Fallos de Compilación
```bash
npm run build -- --verbose
```

### Limpiar Caché
```bash
rm -rf .angular/cache
npm start
```

## Documentación Relacionada

- **Configuración del Backend**: Ver `src/MyApp/README.md`
- **Gestión de Base de Datos**: Ver `DATABASE_SETUP.md` en la raíz del proyecto
- **Descripción General del Proyecto**: Ver `README.md` en la raíz del proyecto

## Recursos de Angular

- [Documentos Oficiales de Angular](https://angular.dev/)
- [Documentos de Angular Material](https://material.angular.io/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Angular Signals](https://angular.dev/guide/signals)

## Soporte

Para problemas relacionados con la API, asegúrate de que el servidor backend se ejecute en la URL de API configurada.
