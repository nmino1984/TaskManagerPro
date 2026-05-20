# TaskManagerPro - Frontend Application

A responsive, modern task management interface built with Angular 21 and Angular Material. Features signal-based state management, reactive forms, and Material Design components.

## Quick Start

### Prerequisites
- **Node.js 18+** and npm
- **Angular 21** CLI (installed via npm)

### Setup

```bash
cd frontend
npm install
npm start
```

The application will run at `http://localhost:4200` and automatically reload on changes.

## Development Commands

```bash
# Start development server
npm start

# Build for production
npm run build

# Run production build locally
npm run serve:ssr

# Run linter
npm run lint

# Format code
npm run format

# Run unit tests (if configured)
npm test

# Build only (no serve)
ng build
```

## Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── models/              # TypeScript interfaces & enums
│   │   │   ├── services/            # HTTP services (tasks, milestones, auth)
│   │   │   ├── interceptors/        # HTTP interceptors (JWT, error handling)
│   │   │   └── guards/              # Route guards (authentication)
│   │   ├── features/
│   │   │   ├── auth/                # Login/Register components
│   │   │   ├── tasks/               # Task list and detail components
│   │   │   ├── task-form/           # Task creation/editing form
│   │   │   ├── subtasks/            # Subtask components
│   │   │   └── milestones/          # Milestone components
│   │   ├── shared/
│   │   │   ├── components/          # Reusable UI components
│   │   │   └── pipes/               # Custom pipes
│   │   ├── app.component.*          # Root component
│   │   ├── app.routes.ts            # Route configuration
│   │   └── app.config.ts            # Application configuration
│   ├── environments/                # Environment configs (dev, prod)
│   ├── index.html
│   ├── styles.scss
│   └── main.ts
├── angular.json                     # Angular CLI configuration
├── tsconfig.json                    # TypeScript configuration
├── package.json                     # Dependencies
└── README.md
```

## Architecture

### State Management
- **Angular Signals**: Reactive state management for component signals
- **No RxJS Subjects**: Clean, predictable state handling
- **Change Detection OnPush**: Performance optimization throughout the app

### Services
All services follow the same pattern:
- Single responsibility principle
- HTTP client for API communication
- Observable return types (or Signal-based for polling)
- Error handling via interceptors

Key services:
- **AuthService**: User authentication and JWT token management
- **TaskService**: Task CRUD and querying
- **SubTaskService**: Subtask management
- **MilestoneService**: Milestone operations and exports
- **NotificationService**: Notification polling, reading, and state management (Signals-based)

### Components
- **Standalone Components**: No NgModule dependencies
- **Reactive Forms**: Typed form controls with validation
- **Material Design**: Consistent UI with Angular Material
- **MatDialog**: Modals for create/edit operations

### HTTP Layer
- **JWT Interceptor**: Automatically adds Bearer token to requests
- **Error Interceptor**: Centralized error handling with snackbar notifications
- **Environment-based URLs**: Development vs Production API endpoints

## Key Features

### Authentication
- User registration and login
- JWT token storage (localStorage)
- Automatic token injection in HTTP requests
- Login/logout functionality

### Tasks
- Create, read, update, delete (CRUD) operations
- Pagination and filtering
- Priority levels (Low, Medium, High)
- Status tracking (Not Started, In Progress, Completed, Overdue)
- Soft delete support

### SubTasks
- Break down tasks into manageable pieces
- Track completion status
- Automatic progress calculation
- Full CRUD operations

### Milestones
- Define project checkpoints
- Track milestone status
- Export to multiple formats:
  - **JSON**: For data integration
  - **XML**: For system integration
  - **iCalendar**: For calendar applications (Google Calendar, Outlook, Apple Calendar)

### Notifications
- **Real-time Badge**: Navbar icon with unread notification count
- **Notification Dropdown**: Click bell icon to view recent notifications
- **Async Updates**: Notifications polled every 30 seconds
- **Mark as Read**: Individual or bulk marking of notifications
- **Event Triggers**: Notifications for task creation, completion, and overdue alerts

## Material Design Components Used

- **MatTable**: Data display and pagination
- **MatDialog**: Modal dialogs for create/edit
- **MatForm**: Form inputs and validation
- **MatButton**: Action buttons
- **MatIcon**: Icons throughout
- **MatCard**: Card layouts
- **MatDatepicker**: Date selection
- **MatSelect**: Dropdown selection
- **MatSnackBar**: Toast notifications
- **MatBadge**: Notification count badge on navbar
- **MatMenu**: Dropdown menu for notifications
- **MatToolbar**: Header and navigation bar
- **MatTooltip**: Hover tooltips

## Development Workflow

### Creating a New Component

```bash
ng generate component features/your-feature/your-component
```

The component will be:
- Standalone (no NgModule)
- Using OnPush change detection
- With SCSS stylesheet

### Adding a New Service

```bash
ng generate service core/services/your-service
```

Services should:
- Depend on HttpClient
- Return Observables
- Handle API communication

### Forms Best Practices

```typescript
// Use FormGroup with typed controls
form = new FormGroup({
  title: new FormControl('', Validators.required),
  description: new FormControl('', Validators.required),
});

// Access typed control values
this.form.value.title
```

## Environment Configuration

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

## Error Handling

Errors are handled centrally via `error.interceptor.ts`:
- Network errors → Snackbar notification
- 401/403 Unauthorized → Redirect to login
- 4xx/5xx Errors → Display error message
- Automatic retry for certain errors

## Testing

Component testing pattern:
```typescript
it('should create', () => {
  expect(component).toBeTruthy();
});

it('should load tasks', fakeAsync(() => {
  // Test implementation
}));
```

## Performance Optimization

- **OnPush Change Detection**: Only updates when inputs change
- **Lazy Loading**: Features loaded on demand via routing
- **Signals**: Minimal re-renders compared to RxJS
- **Production Build**: Minification, tree-shaking, bundling

## Build Output

```bash
npm run build
```

Produces optimized production build in `dist/` directory:
- Minified JavaScript
- Optimized CSS
- Lazy-loaded feature bundles
- Source maps (if enabled)

## Browser Support

- Chrome/Chromium (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Styling

- **SCSS**: For component and global styles
- **CSS Variables**: For theming
- **Material Theme**: Built on Material Design system
- **Responsive**: Mobile, tablet, and desktop layouts

## HTTP Interceptors

### JWT Interceptor
```typescript
// Automatically adds token to all requests
Authorization: 'Bearer {token}'
```

### Error Interceptor
```typescript
// Catches and handles errors centrally
// Shows snackbar notifications
// Logs errors for debugging
```

## Notification System

### NotificationService
The `NotificationService` manages real-time notifications with:
- **Signals-based State**: `notifications` signal holds the list, `unreadCount` holds count
- **Auto-polling**: Calls backend every 30 seconds
- **Lazy Loading**: Starts polling on user login, stops on logout
- **Mark as Read**: Individual or bulk operations
- **Error Handling**: Silent failures with console logging

### Navbar Integration
- **Notification Bell Icon**: Shows in navbar header
- **Unread Badge**: Red badge displays unread count (hidden if 0)
- **Dropdown Menu**: Click bell to open notification panel
- **Recent Notifications**: Shows up to 10 most recent
- **Mark All**: Button to mark all as read at once

## Troubleshooting

### Port Already in Use
```bash
# macOS/Linux
lsof -i :4200

# Windows
netstat -ano | findstr :4200
```

### Dependencies Not Installing
```bash
rm -rf node_modules package-lock.json
npm install
```

### Build Failures
```bash
npm run build -- --verbose
```

### Clear Cache
```bash
rm -rf .angular/cache
npm start
```

## Related Documentation

- **Backend Setup**: See `src/MyApp/README.md`
- **Database Management**: See `DATABASE_SETUP.md` in project root
- **Full Project Overview**: See `README.md` in project root

## Angular Resources

- [Angular Official Docs](https://angular.dev/)
- [Angular Material Docs](https://material.angular.io/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Angular Signals](https://angular.dev/guide/signals)

## Support

For API-related issues, ensure the backend server is running at the configured API URL.
