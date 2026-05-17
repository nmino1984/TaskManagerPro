# TaskManagerPro - Aplicación Frontend

Una interfaz moderna y responsiva para gestión de tareas construida con Angular 21 y Angular Material. Incluye gestión de estado basada en señales, formularios reactivos y componentes Material Design.

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

La aplicación se ejecutará en `http://localhost:4200` y se recargará automáticamente al hacer cambios.

## Comandos de Desarrollo

```bash
# Iniciar servidor de desarrollo
npm start

# Construir para producción
npm run build

# Ejecutar construcción de producción localmente
npm run serve:ssr

# Ejecutar linter
npm run lint

# Formatear código
npm run format

# Ejecutar pruebas unitarias (si está configurado)
npm test

# Solo construir (sin servir)
ng build
```

## Estructura del Proyecto

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── models/              # Interfaces y enums de TypeScript
│   │   │   ├── services/            # Servicios HTTP (tareas, hitos, auth)
│   │   │   ├── interceptors/        # Interceptores HTTP (JWT, manejo de errores)
│   │   │   └── guards/              # Guards de ruta (autenticación)
│   │   ├── features/
│   │   │   ├── auth/                # Componentes de Login/Registro
│   │   │   ├── tasks/               # Componentes de lista y detalle de tareas
│   │   │   ├── task-form/           # Formulario de creación/edición de tareas
│   │   │   ├── subtasks/            # Componentes de subtareas
│   │   │   └── milestones/          # Componentes de hitos
│   │   ├── shared/
│   │   │   ├── components/          # Componentes reutilizables
│   │   │   └── pipes/               # Pipes personalizados
│   │   ├── app.component.*          # Componente raíz
│   │   ├── app.routes.ts            # Configuración de rutas
│   │   └── app.config.ts            # Configuración de la aplicación
│   ├── environments/                # Configuraciones de entorno (dev, prod)
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
- **Angular Signals**: Gestión de estado reactivo para señales de componentes
- **Sin RxJS Subjects**: Manejo de estado limpio y predecible
- **Change Detection OnPush**: Optimización de rendimiento en toda la aplicación

### Servicios
Todos los servicios siguen el mismo patrón:
- Principio de responsabilidad única
- HttpClient para comunicación con API
- Tipos de retorno Observable
- Manejo de errores vía interceptores

### Componentes
- **Componentes Standalone**: Sin dependencias NgModule
- **Formularios Reactivos**: Controles de formulario tipados con validación
- **Material Design**: Interfaz consistente con Angular Material
- **MatDialog**: Modales para operaciones de crear/editar

### Capa HTTP
- **JWT Interceptor**: Agrega automáticamente token Bearer a solicitudes
- **Error Interceptor**: Manejo centralizado de errores con notificaciones snackbar
- **URLs basadas en entorno**: Puntos finales de API de desarrollo vs producción

## Características Principales

### Autenticación
- Registro e inicio de sesión de usuarios
- Almacenamiento de token JWT (localStorage)
- Inyección automática de token en solicitudes HTTP
- Funcionalidad de login/logout

### Tareas
- Operaciones CRUD (Crear, Leer, Actualizar, Eliminar)
- Paginación y filtrado
- Niveles de prioridad (Baja, Media, Alta)
- Seguimiento de estado (No Iniciada, En Progreso, Completada, Retrasada)
- Soporte de eliminación suave

### SubTareas
- Desglosar tareas en partes manejables
- Rastrear estado de completación
- Cálculo automático de progreso
- Operaciones CRUD completas

### Hitos
- Definir puntos de control del proyecto
- Rastrear estado de hitos
- Exportar a múltiples formatos:
  - **JSON**: Para integración de datos
  - **XML**: Para integración de sistemas
  - **iCalendar**: Para aplicaciones de calendario (Google Calendar, Outlook, Apple Calendar)

## Componentes Material Design Utilizados

- **MatTable**: Visualización de datos y paginación
- **MatDialog**: Diálogos modales para crear/editar
- **MatForm**: Entradas de formulario y validación
- **MatButton**: Botones de acción
- **MatIcon**: Iconos en toda la aplicación
- **MatCard**: Diseños de tarjeta
- **MatDatepicker**: Selección de fechas
- **MatSelect**: Selección desplegable
- **MatSnackBar**: Notificaciones toast

## Flujo de Desarrollo

### Crear un Nuevo Componente

```bash
ng generate component features/tu-caracteristica/tu-componente
```

El componente será:
- Standalone (sin NgModule)
- Usando change detection OnPush
- Con hoja de estilos SCSS

### Agregar un Nuevo Servicio

```bash
ng generate service core/services/tu-servicio
```

Los servicios deben:
- Depender de HttpClient
- Retornar Observables
- Manejar comunicación con API

### Mejores Prácticas de Formularios

```typescript
// Usar FormGroup con controles tipados
form = new FormGroup({
  title: new FormControl('', Validators.required),
  description: new FormControl('', Validators.required),
});

// Acceder a valores de control tipado
this.form.value.title
```

## Configuración de Entorno

### Development (`environment.ts`)
```typescript
apiUrl: 'http://localhost:5141/api/v1'
production: false
```

### Production (`environment.prod.ts`)
```typescript
apiUrl: '/api/v1'
production: true
```

## Manejo de Errores

Los errores se manejan centralmente vía `error.interceptor.ts`:
- Errores de red → Notificación snackbar
- 401/403 No Autorizado → Redirigir a login
- Errores 4xx/5xx → Mostrar mensaje de error
- Reintentos automáticos para ciertos errores

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

- **Change Detection OnPush**: Solo se actualiza cuando cambian las entradas
- **Lazy Loading**: Características cargadas bajo demanda vía routing
- **Signals**: Re-renders mínimos en comparación con RxJS
- **Production Build**: Minificación, eliminación de código no utilizado, empaquetamiento

## Salida de Construcción

```bash
npm run build
```

Produce una construcción de producción optimizada en el directorio `dist/`:
- JavaScript minificado
- CSS optimizado
- Bundles de características cargadas perezosamente
- Mapas de origen (si está habilitado)

## Compatibilidad de Navegadores

- Chrome/Chromium (última versión)
- Firefox (última versión)
- Safari (última versión)
- Edge (última versión)

## Estilos

- **SCSS**: Para estilos de componentes y globales
- **CSS Variables**: Para temas
- **Material Theme**: Basado en sistema Material Design
- **Responsivo**: Diseños para móvil, tablet y escritorio

## Interceptores HTTP

### JWT Interceptor
```typescript
// Agrega automáticamente token a todas las solicitudes
Authorization: 'Bearer {token}'
```

### Error Interceptor
```typescript
// Captura y maneja errores centralmente
// Muestra notificaciones snackbar
// Registra errores para depuración
```

## Solución de Problemas

### Puerto Ya en Uso
```bash
# macOS/Linux
lsof -i :4200

# Windows
netstat -ano | findstr :4200
```

### Dependencias No Se Instalan
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
- [Manual de TypeScript](https://www.typescriptlang.org/docs/)
- [Angular Signals](https://angular.dev/guide/signals)

## Soporte

Para problemas relacionados con la API, asegúrate de que el servidor backend se esté ejecutando en la URL de API configurada.
