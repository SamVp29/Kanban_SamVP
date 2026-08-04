# Kanban Agile Project Management

Plataforma web de gestión de proyectos ágiles basada en tableros Kanban interactivos.

Permite administrar proyectos, personalizar flujos de trabajo mediante columnas dinámicas y gestionar tareas en tiempo real mediante arrastre y soltar (Drag & Drop), con exportación dual de reportes (PDF y Excel) alimentados por una única consulta a la base de datos.

---

## 🚀 1. Requisitos Previos e Instalación Paso a Paso

### Opción A: Despliegue Automatizado con Docker (Recomendado)

El proyecto está 100% contenedorizado con **Docker** y **Docker Compose** para garantizar un entorno reproducible e independiente de SDKs locales.

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/SamVp29/Kanban_SamVP.git
   cd Kanban_SamVP
   ```
2. **Asegurar que el archivo `.env` existe en la raíz (puedes duplicar `.env.example`):**
   ```bash
   cp .env.example .env
   ```
3. **Ejecutar los servicios en segundo plano:**
   ```bash
   docker-compose up -d --build
   ```

Los servicios quedarán disponibles de inmediato en:
- 🌐 **Frontend (Angular 17 SPA - Nginx):** `http://localhost:4200`
- ⚡ **Backend REST API (.NET 8):** `http://localhost:8080/api`
- 🗄️ **Base de Datos (PostgreSQL 15):** `localhost:5432`

Para detener y limpiar los contenedores:
```bash
docker-compose down -v
```

---

### Opción B: Ejecución Local Manual (Sin Docker)

**Requisitos:** .NET 8 SDK, Node.js v20+, Angular CLI 17, PostgreSQL 15+.

1. **Base de Datos:** Crear una BD PostgreSQL llamada `kanban_db`.
2. **Backend:**
   ```bash
   cd backend
   dotnet ef database update --project Kanban.Infrastructure --startup-project Kanban.WebApi
   dotnet run --project Kanban.WebApi
   ```
3. **Frontend:**
   ```bash
   cd frontend
   npm install
   ng serve
   ```

---

## 🗄️ 2. Migraciones Incremental EF Core y Datos Semilla

### Migraciones Incrementales de Entity Framework Core
La base de datos PostgreSQL se sincroniza automáticamente mediante **Entity Framework Core Migrations** (ubicadas en `backend/Kanban.Infrastructure/Migrations/`).

- **En entorno contenedorizado (Docker / Producción):** Al iniciar la aplicación backend, el adaptador de infraestructura invoca automáticamente `app.Services.ApplyInfrastructureMigrations()`, ejecutando las migraciones incrementales en orden y creando las tablas, relaciones e índices sin intervención manual.
- **En entorno CLI (Manual):** Puedes gestionar las migraciones con los comandos oficiales desde la carpeta `backend/`:
  ```bash
  # Generar una nueva migración incremental
  dotnet ef migrations add <NombreMigracion> --project Kanban.Infrastructure --startup-project Kanban.WebApi

  # Aplicar migraciones pendientes
  dotnet ef database update --project Kanban.Infrastructure --startup-project Kanban.WebApi
  ```

### Datos Semilla Precargados (Data Seeding)
En `ApplicationDbContext.cs` (`SeedData`), se precargan los usuarios iniciales cifrados con **BCrypt** (coste 11):

| Usuario | Correo Electrónico | Contraseña | Rol / Propósito |
| :--- | :--- | :--- | :--- |
| **Usuario 1** | `admin@kanban.com` | `Password123!` | Administrador / Pruebas multi-sesión |
| **Usuario 2** | `tester@kanban.com` | `Password123!` | Evaluador / Pruebas de tiempo real |

---

## 🏗️ 3. Arquitectura Hexagonal y Decisiones de Diseño

El backend implementa **Arquitectura Hexagonal (Ports & Adapters)** en .NET 8, desacoplando completamente el Núcleo de Negocio de los detalles de infraestructura, frameworks y bases de datos.

```text
Kanban_SamVP/
├── backend/
│   ├── Kanban.Domain/          # Núcleo de Dominio (Entidades puras, Interfaces Puertos de Repositorio)
│   ├── Kanban.Application/     # Casos de Uso y Puertos de Salida (DTOs, IPasswordHasher, IJwtTokenGenerator, IBoardNotifier)
│   ├── Kanban.Infrastructure/  # Adaptadores Secundarios (EF Core, PostgreSQL, BCrypt, JwtTokenGenerator, QuestPDF, EPPlus)
│   ├── Kanban.WebApi/          # Adaptadores Primarios (Controllers REST HTTP, SignalR Hubs, Adaptador IBoardNotifier)
│   └── Kanban.UnitTests/       # Pruebas Automatizadas Backend (xUnit, Moq, FluentAssertions)
│
├── frontend/src/
│   ├── app/
│   │   ├── core/               # ⚙️ Servicios globales (auth.service, kanban.service, guard, interceptor)
│   │   ├── features/           # 🧩 Módulos de negocio (projects-list, board kanban)
│   │   ├── pages/              # 📄 Páginas de sistema (login, access denied, not found 404)
│   │   ├── layout/             # 🎨 Layout Sakai de PrimeNG
│   │   ├── app-routing.module.ts
│   │   └── app.module.ts
│   └── assets/                 # 🖼️ Recursos estáticos
│
├── docker-compose.yml
├── er-diagram.png
└── README.md
```

### Justificación de las Capas Hexagonales
1. **`Kanban.Domain` (Core Puro):** Entidades C# puras y Puertos de salida de persistencia (`IProyectoRepository`, `ITareaRepository`, `IColumnaRepository`, `IUsuarioRepository`). **0 dependencias externas**.
2. **`Kanban.Application` (Casos de Uso & Puertos):** Define las reglas de negocio, DTOs de transferencia y **Puertos de Salida** para infraestructura (`IPasswordHasher`, `IJwtTokenGenerator`, `IBoardNotifier`, `IReportGenerator`). **0 dependencias a EF Core, BCrypt, JWT o SignalR**.
3. **`Kanban.Infrastructure` (Adaptadores Secundarios):** Implementaciones concretas de la infraestructura (`BCryptPasswordHasher`, `JwtTokenGenerator`, `PdfReportGenerator`, `ExcelReportGenerator`, `ApplicationDbContext`). Centraliza el registro de DI y la ejecución de migraciones de base de datos.
4. **`Kanban.WebApi` (Adaptadores Primarios):** Controladores REST HTTP y Hubs de SignalR. Los controladores son adaptadores HTTP puros que no conocen SignalR ni EF Core.

---

## 💡 4. Decisiones Técnicas y Justificaciones

### 1. Sincronización en Tiempo Real: SignalR
- **Tecnología Elegida:** **ASP.NET Core SignalR**.
- **Justificación:** Integración nativa de alto rendimiento con .NET y clientes JS/Angular. Maneja reconexión automática, transporte transparente (WebSockets con fallback) y aislamiento de notificaciones por grupo de proyecto (`JoinBoardGroup` / `LeaveBoardGroup`).
- **Alternativas Descartadas:**
  - *Server-Sent Events (SSE):* Descartado por ser unidireccional (servidor a cliente) y requerir peticiones HTTP auxiliares para manejar suscripciones.
  - *Raw WebSockets:* Descartado por requerir implementación manual de reconexiones, handshakes, heartbeat y enrutamiento por salas.
  - *Short Polling:* Descartado por ineficiencia en consultas recurrentes a la BD y latencia superior al umbral exigido.

### 2. Estrategia de Índices de Ordenamiento: Lexicographical Ranking ($O(1)$)
- **Estrategia Elegida:** Campo numérico decimal `Orden` (float/double) en tareas y columnas.
- **Justificación:** Al mover una tarea entre dos elementos o al final de una columna, la nueva posición se calcula mediante la fórmula `(orden_anterior + orden_siguiente) / 2` o `maxOrden + 65536`. Esto permite una **operación de actualización atómica de costo $O(1)$ sin reindexar ni actualizar masivamente los demás registros en la base de datos**.

### 3. Patrón Aplicado en la Exportación Dual (PDF & Excel)
- **Patrón Elegido:** **Strategy Pattern** + Eager Loading con consulta única.
- **Justificación:** `ReportService` inyecta la colección de estrategias `IEnumerable<IReportGenerator>`. La exportación a PDF (QuestPDF) y Excel (EPPlus) consume una **única consulta SQL optimizada** (`GetProyectoCompletoReporteAsync` con `.Include().ThenInclude()`), eliminando el problema de consulta N+1.

### 4. Seguridad y Autenticación
- **Hashes Criptográficos:** Cifrado con **BCrypt** de coste 11 (salting dinámico).
- **JWT & Interceptor:** Autenticación mediante Tokens Bearer JWT. El `AuthInterceptor` en Angular adjunta el token Bearer en las cabeceras HTTP y gestiona las respuestas 401.

---

## 📊 5. Diagrama del Modelo de Base de Datos (ER)

<p align="center">
  <img src="er-diagram.png" width="100%" alt="Diagrama ER Modelo de Datos" />
</p>

---

## ⭐ 6. Funcionalidades Destacadas del Sistema

1. **Gestión de Proyectos:** CRUD completo con listado paginado y búsqueda en servidor.
2. **Tablero Kanban Dinámico:** Arrastre y soltar (Drag & Drop) de tareas con actualización optimista.
3. **Reordenación de Columnas:** Reordenamiento dinámico de columnas desde la interfaz.
4. **Sincronización en Tiempo Real:** Actualizaciones inmediatas entre múltiples navegadores sin recargar pantalla.
5. **Indicador de Usuarios Conectados:** Conteo activo en tiempo real de usuarios en la misma sala del tablero.
6. **Búsqueda y Filtros Combinados:** Filtro parcial por texto, Prioridad y Responsable con botón de limpiado.
7. **Interfaz Sakai Depurada:** Plantilla Sakai de PrimeNG optimizada con diálogos de confirmación nativos.

---

## 🧪 7. Pruebas Automatizadas

El proyecto incluye **13 pruebas unitarias automatizadas** (6 en Backend y 7 en Frontend), ejecutando al 100%.

### Ejecución de Pruebas en Backend (.NET 8 xUnit)
```bash
dotnet test backend/Kanban.UnitTests/Kanban.UnitTests.csproj
```
- **Prueba Obligatoria de Cálculo de Posición:** `CrearTareaAsync_DebeCalcularNuevaPosicionLexicografica_CuandoSeAgregaUnaTarea` (verifica ranking $maxOrden + 65536$).
- **Regla de Negocio de Columnas:** `DeleteAsync_DebeLanzarExcepcion_CuandoColumnaTieneTareas` (bloquea eliminación de columnas no vacías).

### Ejecución de Pruebas en Frontend (Angular 17 Jasmine / Karma)
```bash
cd frontend
npm run test -- --watch=false
```

---

## 🤖 8. Declaración del Uso de Asistentes de Inteligencia Artificial (IA)

En cumplimiento de las especificaciones de la prueba técnica (Secciones 8 y 9), se declara el uso transparente y supervisado de **Antigravity AI (Gemini 3.6 Flash)** como herramienta auxiliar de desarrollo en las siguientes áreas:

1. **Maquetado y Estilos de Interfaz:** Asistencia en la adaptación de plantillas HTML, estilos SCSS y componentes visuales de PrimeNG (Sakai).
2. **Generación de Código Repetitivo (Boilerplate):** Aceleración en el armado inicial de DTOs de transferencia y estructuras base.
3. **Documentación y Guías:** Ayuda en la redacción y formateo Markdown del archivo README y configuración de Docker Compose.
