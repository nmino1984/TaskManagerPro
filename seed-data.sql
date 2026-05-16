-- ========================================
-- SEED DATA para TaskManagerPro
-- Datos de ejemplo: ~1 mes de actividad
-- 4 usuarios, 20 tareas, 15 subtasks, 11 milestones
-- ========================================

-- ========================================
-- LIMPIAR DATOS EXISTENTES
-- ========================================
DELETE FROM Milestones;
DELETE FROM SubTasks;
DELETE FROM MyTasks;
DELETE FROM Users;

-- ========================================
-- INSERTAR USUARIOS
-- ========================================
INSERT INTO Users (Username, Email, PasswordHash) VALUES
('carlos.dev', 'carlos@taskmaster.io', '$2a$11$hashed_password_carlos_dev'),
('maria.qa', 'maria@taskmaster.io', '$2a$11$hashed_password_maria_qa'),
('juan.backend', 'juan@taskmaster.io', '$2a$11$hashed_password_juan_backend'),
('sofia.frontend', 'sofia@taskmaster.io', '$2a$11$hashed_password_sofia_frontend');

-- ========================================
-- INSERTAR TAREAS
-- ========================================

-- Carlos (user-001): Tareas de Backend
INSERT INTO MyTasks (UserId, Title, Description, Status, Priority, StartDate, EndDate, Progress, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('user-001', 'Implementar autenticación JWT', 'Configurar JWT para la API REST', 'Completed', 'High', '2026-04-16 08:00:00', '2026-04-20 17:00:00', 100, '2026-04-16 08:00:00', '2026-04-20 15:30:00', 0),
('user-001', 'API Rest para gestión de tareas', 'Implementar endpoints CRUD para tareas', 'Completed', 'High', '2026-04-16 09:00:00', '2026-04-19 17:00:00', 100, '2026-04-16 09:00:00', '2026-04-19 15:00:00', 0),
('user-001', 'Documentación de API', 'Crear documentación Swagger/OpenAPI para la API REST', 'NotStarted', 'Low', '2026-05-15 09:00:00', '2026-05-22 17:00:00', 0, '2026-05-14 14:00:00', '2026-05-14 14:00:00', 0),
('user-001', 'Implementar logging centralizado', 'Configurar Serilog para logging centralizado', 'Completed', 'Medium', '2026-05-01 09:00:00', '2026-05-03 17:00:00', 100, '2026-05-01 09:00:00', '2026-05-03 14:30:00', 0),
('user-001', 'Manejo de errores global', 'Implementar error handling centralizado en la API', 'Completed', 'High', '2026-04-28 09:00:00', '2026-04-30 17:00:00', 100, '2026-04-28 09:00:00', '2026-04-30 16:15:00', 0);

-- Maria (user-002): Tareas de QA
INSERT INTO MyTasks (UserId, Title, Description, Status, Priority, StartDate, EndDate, Progress, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('user-002', 'Pruebas unitarias del servicio de tareas', 'Escribir y ejecutar pruebas unitarias para TaskService', 'Completed', 'High', '2026-04-21 09:00:00', '2026-04-25 17:00:00', 100, '2026-04-21 09:00:00', '2026-04-25 16:45:00', 0),
('user-002', 'Validación de formularios', 'Implementar validaciones en formularios del frontend', 'InProgress', 'Medium', '2026-05-06 10:00:00', '2026-05-15 17:00:00', 45, '2026-05-06 10:00:00', '2026-05-14 11:30:00', 0),
('user-002', 'Integración de paginación', 'Implementar paginación en la lista de tareas', 'Completed', 'Medium', '2026-05-02 10:00:00', '2026-05-04 17:00:00', 100, '2026-05-02 10:00:00', '2026-05-04 15:45:00', 0),
('user-002', 'Testing E2E con Cypress', 'Configurar y escribir tests E2E para flujos principales', 'NotStarted', 'Medium', '2026-05-16 09:00:00', '2026-05-25 17:00:00', 0, '2026-05-15 10:00:00', '2026-05-15 10:00:00', 0),
('user-002', 'Reporte de calidad de código', 'Ejecutar análisis SonarQube y revisar resultados', 'NotStarted', 'Low', '2026-05-22 09:00:00', '2026-05-29 17:00:00', 0, '2026-05-14 09:00:00', '2026-05-14 09:00:00', 0);

-- Juan (user-003): Tareas de Database/Backend
INSERT INTO MyTasks (UserId, Title, Description, Status, Priority, StartDate, EndDate, Progress, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('user-003', 'Refactorizar la base de datos', 'Optimizar índices y relaciones en SQLite', 'Completed', 'Medium', '2026-04-18 10:00:00', '2026-04-22 17:00:00', 100, '2026-04-18 10:00:00', '2026-04-22 14:20:00', 0),
('user-003', 'Implementar soft delete en entidades', 'Añadir campo IsDeleted a las entidades principales', 'Completed', 'Medium', '2026-04-25 09:00:00', '2026-04-27 17:00:00', 100, '2026-04-25 09:00:00', '2026-04-27 14:00:00', 0),
('user-003', 'Optimización de queries de base de datos', 'Analizar y optimizar queries lentas', 'InProgress', 'Medium', '2026-05-08 10:00:00', '2026-05-18 17:00:00', 35, '2026-05-08 10:00:00', '2026-05-14 13:20:00', 0),
('user-003', 'Cache de datos en memoria', 'Implementar caching para mejorar rendimiento', 'NotStarted', 'Low', '2026-05-20 09:00:00', '2026-05-28 17:00:00', 0, '2026-05-14 11:00:00', '2026-05-14 11:00:00', 0),
('user-003', 'Seguridad: validar inputs', 'Implementar validación de inputs para prevenir SQL injection', 'InProgress', 'High', '2026-05-09 10:00:00', '2026-05-16 17:00:00', 70, '2026-05-09 10:00:00', '2026-05-14 14:15:00', 0);

-- Sofia (user-004): Tareas de Frontend
INSERT INTO MyTasks (UserId, Title, Description, Status, Priority, StartDate, EndDate, Progress, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('user-004', 'Diseñar componentes Angular con Material', 'Crear componentes reutilizables con Angular Material', 'InProgress', 'High', '2026-04-23 08:30:00', '2026-05-02 17:00:00', 65, '2026-04-23 08:30:00', '2026-05-14 10:15:00', 0),
('user-004', 'Integración de Modal de tareas', 'Crear modal para crear y editar tareas', 'Completed', 'High', '2026-05-01 08:00:00', '2026-05-05 17:00:00', 100, '2026-05-01 08:00:00', '2026-05-05 16:30:00', 0),
('user-004', 'Feature de búsqueda avanzada', 'Implementar búsqueda con filtros en el frontend', 'InProgress', 'Medium', '2026-05-10 09:00:00', '2026-05-20 17:00:00', 55, '2026-05-10 09:00:00', '2026-05-14 15:00:00', 0),
('user-004', 'Responsive design para móviles', 'Optimizar interfaz para dispositivos móviles', 'InProgress', 'High', '2026-05-05 08:00:00', '2026-05-17 17:00:00', 40, '2026-05-05 08:00:00', '2026-05-14 12:30:00', 0),
('user-004', 'Dark mode para la aplicación', 'Implementar tema oscuro con Angular Material', 'NotStarted', 'Low', '2026-05-26 09:00:00', '2026-06-05 17:00:00', 0, '2026-05-14 13:30:00', '2026-05-14 13:30:00', 0);

-- ========================================
-- INSERTAR SUBTASKS
-- ========================================

-- Task 1 (JWT Authentication) - Subtasks
INSERT INTO SubTasks (TaskId, Description, Status, DueDate, Notes, CreatedAt, UpdatedAt)
VALUES
(1, 'Configurar librerías JWT', 'Completed', '2026-04-18 17:00:00', 'Usado System.IdentityModel.Tokens.Jwt', '2026-04-16 08:00:00', '2026-04-18 16:00:00'),
(1, 'Crear endpoints de login y registro', 'Completed', '2026-04-19 17:00:00', 'Endpoints POST /api/auth/login y /api/auth/register', '2026-04-16 08:00:00', '2026-04-19 15:30:00'),
(1, 'Testear autenticación', 'Completed', '2026-04-20 17:00:00', 'Tests unitarios completados', '2026-04-16 08:00:00', '2026-04-20 14:00:00');

-- Task 2 (Unit Tests) - Subtasks
INSERT INTO SubTasks (TaskId, Description, Status, DueDate, Notes, CreatedAt, UpdatedAt)
VALUES
(2, 'Escribir pruebas para GetByIdAsync', 'Completed', '2026-04-22 17:00:00', '5 tests creados', '2026-04-21 09:00:00', '2026-04-22 15:00:00'),
(2, 'Escribir pruebas para CreateAsync', 'Completed', '2026-04-23 17:00:00', 'Tests para validación de datos', '2026-04-21 09:00:00', '2026-04-23 16:30:00'),
(2, 'Ejecutar y validar todos los tests', 'Completed', '2026-04-25 17:00:00', '15/15 tests pasados', '2026-04-21 09:00:00', '2026-04-25 16:45:00');

-- Task 3 (Database Refactoring) - Subtasks
INSERT INTO SubTasks (TaskId, Description, Status, DueDate, Notes, CreatedAt, UpdatedAt)
VALUES
(3, 'Análisis de índices actuales', 'Completed', '2026-04-19 17:00:00', 'Identificadas 3 queries lentas', '2026-04-18 10:00:00', '2026-04-19 14:00:00'),
(3, 'Crear índices para búsqueda y filtrado', 'Completed', '2026-04-21 17:00:00', 'Índices compuestos en UserId, IsDeleted, CreatedAt', '2026-04-18 10:00:00', '2026-04-21 15:30:00'),
(3, 'Validar mejora en rendimiento', 'Completed', '2026-04-22 17:00:00', 'Mejora del 40% en queries', '2026-04-18 10:00:00', '2026-04-22 14:20:00');

-- Task 4 (Angular Components) - Subtasks
INSERT INTO SubTasks (TaskId, Description, Status, DueDate, Notes, CreatedAt, UpdatedAt)
VALUES
(4, 'Diseñar TaskListComponent', 'Completed', '2026-04-25 17:00:00', 'Componente standalone con signals', '2026-04-23 08:30:00', '2026-04-25 16:00:00'),
(4, 'Crear TaskFormComponent', 'Completed', '2026-04-29 17:00:00', 'Modal con formulario reactivo', '2026-04-23 08:30:00', '2026-04-29 15:45:00'),
(4, 'Implementar tabs para edit mode', 'InProgress', '2026-05-02 17:00:00', 'Task Info, Subtasks, Milestones', '2026-04-23 08:30:00', '2026-05-14 10:15:00');

-- Task 6 (Form Validation) - Subtasks
INSERT INTO SubTasks (TaskId, Description, Status, DueDate, Notes, CreatedAt, UpdatedAt)
VALUES
(6, 'Crear validadores personalizados', 'InProgress', '2026-05-10 17:00:00', 'Validators para título y descripción', '2026-05-06 10:00:00', '2026-05-14 11:30:00'),
(6, 'Mostrar mensajes de error', 'InProgress', '2026-05-12 17:00:00', 'Mat-error en cada field', '2026-05-06 10:00:00', '2026-05-14 11:30:00');

-- ========================================
-- INSERTAR MILESTONES
-- ========================================

INSERT INTO Milestones (TaskId, Title, Description, TargetDate, Status, CreatedAt, UpdatedAt)
VALUES
(1, 'Integración de JWT', 'Completar la integración de autenticación con JWT', '2026-04-20 17:00:00', 'Completed', '2026-04-16 08:00:00', '2026-04-20 15:30:00'),
(1, 'Testing de autenticación', 'Completar todos los tests de autenticación', '2026-04-21 17:00:00', 'Completed', '2026-04-16 08:00:00', '2026-04-20 14:30:00'),
(3, 'Optimización de BD', 'Crear índices y optimizar queries', '2026-04-22 17:00:00', 'Completed', '2026-04-18 10:00:00', '2026-04-22 14:20:00'),
(5, 'API CRUD completa', 'Endpoints GET, POST, PUT, DELETE funcionando', '2026-04-19 17:00:00', 'Completed', '2026-04-16 09:00:00', '2026-04-19 15:00:00'),
(8, 'Modal de tareas', 'Interfaz modal para crear y editar tareas', '2026-05-05 17:00:00', 'Completed', '2026-05-01 08:00:00', '2026-05-05 16:30:00'),
(4, 'Componentes Material', 'Crear componentes reutilizables con Angular Material', '2026-05-15 17:00:00', 'Pending', '2026-04-23 08:30:00', '2026-05-14 10:15:00'),
(6, 'Validación de formularios', 'Implementar validación en todos los formularios', '2026-05-15 17:00:00', 'Pending', '2026-05-06 10:00:00', '2026-05-14 11:30:00'),
(11, 'Optimización de queries', 'Mejorar rendimiento de consultas a BD', '2026-05-18 17:00:00', 'Pending', '2026-05-08 10:00:00', '2026-05-14 13:20:00'),
(12, 'Búsqueda avanzada', 'Feature de búsqueda con filtros múltiples', '2026-05-20 17:00:00', 'Pending', '2026-05-10 09:00:00', '2026-05-14 15:00:00'),
(16, 'Responsive design', 'Optimizar para dispositivos móviles', '2026-05-17 17:00:00', 'Pending', '2026-05-05 08:00:00', '2026-05-14 12:30:00'),
(19, 'Validación de seguridad', 'Implementar validación de inputs para seguridad', '2026-05-16 17:00:00', 'Overdue', '2026-05-09 10:00:00', '2026-05-14 14:15:00');

-- ========================================
-- VERIFICACIÓN: Contar registros insertados
-- ========================================
SELECT 'Usuarios insertados:' as Info, COUNT(*) as Total FROM Users
UNION ALL
SELECT 'Tareas insertadas:', COUNT(*) FROM MyTasks
UNION ALL
SELECT 'Subtasks insertadas:', COUNT(*) FROM SubTasks
UNION ALL
SELECT 'Milestones insertados:', COUNT(*) FROM Milestones;
