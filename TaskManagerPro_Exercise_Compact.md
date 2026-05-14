# TaskManagerPro - Ejercicio Técnico

## El Problema

Usuarios manejan proyectos complejos que requieren múltiples niveles de organización: tareas principales, subtareas que desglosan el trabajo, y hitos que marcan objetivos importantes. Además, necesitan compartir estos hitos con otros sistemas (calendarios, reportes, integraciones). La solución debe integrar todo en una única aplicación.

## Requisitos Funcionales

**Tareas y Subtareas** (ya implementadas)
- CRUD de tareas con título, descripción, fechas, prioridad y estado
- Subtareas que desglosan el trabajo dentro de cada tarea

**Hitos (Milestones)** (nuevo)
- CRUD de hitos dentro de tareas: título, descripción, fecha objetivo, estado
- Un hito representa un objetivo importante, diferente de una subtarea

**Exportación de Hitos** (nuevo)
- Exportar hitos a JSON, XML e iCal
- Integración con calendarios externos (Google Calendar, Outlook, etc.)

**Autenticación**
- Registro e inicio de sesión con JWT
- Cada usuario solo ve sus propios datos

---

## Stack Técnico

| Aspecto | Tecnología |
|--------|-----------|
| Backend | .NET 10, ASP.NET Core, EF Core |
| Frontend | Angular (standalone), Angular Material |
| Base de Datos | SQLite (dev) |
| Autenticación | JWT |

---

## Criterios de Éxito

✅ Tasks y SubTasks funcionan como está (sin cambios)  
✅ Milestones CRUD implementado  
✅ Exportación a JSON, XML e iCal funciona  
✅ Interfaz distingue claramente SubTask vs Milestone  
✅ Clean Architecture sin acoplamiento  

---

## Repositorio

`https://github.com/nmino1984/TaskManagerPro`
