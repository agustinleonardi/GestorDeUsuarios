# GestorDeUsuarios-Evoltis

Sistema de gestión de usuarios desarrollado en .NET 8.0 con Entity Framework Core y MySQL.

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (versión 8.0 o superior)
- [Git](## 🚀 Deploy y Producción

### Variables de Entorno Requeridas
```bash
# Base de datos
DB_CONNECTION_STRING=Server=prod-server;Database=gestor_usuarios;User=prod_user;Password=secure_password;

# Logging (opcional)
ASPNETCORE_ENVIRONMENT=Production
```downloads)
- IDE: [Visual Studio Code](https://code.visualstudio.com/) o [Visual Studio](https://visualstudio.microsoft.com/)

## 🚀 Configuración e Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/agustinleonardi/GestorDeUsuarios-Evoltis.git
cd GestorDeUsuarios-Evoltis
```

### 2. Configurar Variables de Entorno (IMPORTANTE)

⚠️ **ANTES de continuar**, crear un archivo `.env` en la **raíz del proyecto** (mismo nivel que `GestorDeUsuarios.sln`):

```env
DB_CONNECTION_STRING=Server=localhost;Database=gestor_usuarios;User=tu_usuario_mysql;Password=tu_password_mysql;
```

**Reemplaza los valores con tus credenciales de MySQL:**
- `tu_usuario_mysql`: Tu usuario de MySQL (ej: `root`)
- `tu_password_mysql`: Tu contraseña de MySQL
- Puedes cambiar `gestor_usuarios` por el nombre de BD que prefieras

### 3. Restaurar Dependencias

```bash
dotnet restore
```

### 4. Configurar Base de Datos MySQL

**Opción A - Crear BD automáticamente (recomendado):**
```bash
# La aplicación creará la base de datos automáticamente al ejecutarse
dotnet run --project src/GestorDeUsuarios.API
```

**Opción B - Crear BD manualmente:**
```sql
-- Conectar a MySQL y ejecutar:
CREATE DATABASE gestor_usuarios;
```

### 5. Aplicar Migraciones (Opcional)

Las migraciones se aplican automáticamente, pero puedes hacerlo manualmente:

```bash
cd src/GestorDeUsuarios.API
dotnet ef database update --project ../GestorDeUsuarios.Infrastructure
```

### 6. Ejecutar la Aplicación

```bash
cd src/GestorDeUsuarios.API
dotnet run
```

La aplicación estará disponible en:

- **HTTPS**: https://localhost:7193
- **HTTP**: http://localhost:5193
- **Swagger UI**: https://localhost:7193/swagger (solo en desarrollo)

## 🏗️ Arquitectura del Proyecto

```
src/
├── GestorDeUsuarios.API/          # Capa de presentación (Web API)
├── GestorDeUsuarios.Application/  # Capa de aplicación (casos de uso)
├── GestorDeUsuarios.Domain/       # Capa de dominio (entidades y reglas de negocio)
└── GestorDeUsuarios.Infrastructure/ # Capa de infraestructura (acceso a datos)
```

## 🗄️ Esquema de Base de Datos

El proyecto utiliza MySQL con las siguientes tablas:

### Tabla: Users (Usuarios)

| Campo | Tipo | Restricciones |
|-------|------|---------------|
| `Id` | `int` | PK, auto-increment |
| `Name` | `varchar(100)` | NOT NULL |
| `Email` | `varchar(100)` | NOT NULL, UNIQUE |
| `CreationDate` | `datetime(6)` | NOT NULL |

### Tabla: Addresses (Direcciones)

| Campo | Tipo | Restricciones |
|-------|------|---------------|
| `Id` | `int` | PK, auto-increment |
| `UserId` | `int` | FK → Users.Id, NOT NULL, UNIQUE |
| `Street` | `varchar(100)` | NOT NULL (Calle) |
| `Number` | `varchar(10)` | NOT NULL (Número) |
| `Province` | `varchar(50)` | NOT NULL (Provincia) |
| `City` | `varchar(50)` | NOT NULL (Ciudad) |
| `CreationDate` | `datetime(6)` | NOT NULL |

### Relaciones
- **Users ↔ Addresses**: Relación 1:1 (Un usuario puede tener máximo una dirección)

## 🧪 Ejecutar Tests

El proyecto incluye tests unitarios y de integración completos:

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar solo tests unitarios
dotnet test --filter "Category=Unit"

# Ejecutar solo tests de integración
dotnet test --filter "IntegrationTests"

# Tests con coverage
dotnet test --collect:"XPlat Code Coverage"

# Tests con verbosidad detallada
dotnet test --verbosity normal
```

### Coverage de Tests
- ✅ **Tests Unitarios**: Use Cases, Validators, Mappers
- ✅ **Tests de Integración**: Endpoints completos de la API
- ✅ **Frameworks**: xUnit, Moq, FluentAssertions
- ✅ **Base de datos**: SQLite en memoria para tests

## 📡 Endpoints de la API

### Usuarios

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/users` | Crear un nuevo usuario |
| `GET` | `/api/users/{id}` | Obtener usuario por ID |
| `PUT` | `/api/users/{id}` | Actualizar usuario existente |
| `DELETE` | `/api/users/{id}` | Eliminar usuario |
| `GET` | `/api/users?name={name}&province={province}&city={city}` | Buscar usuarios por criterios |

### Ejemplos de Requests

#### Crear Usuario
```json
POST /api/users
{
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "address": {
    "calle": "Av. Corrientes",
    "numero": "1234",
    "provincia": "Buenos Aires",
    "ciudad": "CABA"
  }
}
```

#### Actualizar Usuario
```json
PUT /api/users/1
{
  "name": "Juan Carlos Pérez",
  "email": "juan.carlos@example.com",
  "address": {
    "calle": "Av. Santa Fe",
    "numero": "5678",
    "provincia": "Buenos Aires",
    "ciudad": "Palermo"
  }
}
```

#### Buscar Usuarios
```bash
GET /api/users?name=Juan&province=Buenos%20Aires&city=CABA
```

### Respuestas de la API

#### Usuario Exitoso
```json
{
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "address": {
    "calle": "Av. Corrientes",
    "numero": "1234",
    "provincia": "Buenos Aires",
    "ciudad": "CABA"
  }
}
```

#### Error de Validación
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["El nombre es requerido"],
    "Email": ["El formato del email no es válido"]
  }
}
```

## 🏗️ Arquitectura y Patrones

### Clean Architecture
- **Capa de Dominio**: Entidades, reglas de negocio, abstracciones
- **Capa de Aplicación**: Casos de uso, DTOs, validadores
- **Capa de Infraestructura**: Repositorios, Entity Framework, base de datos
- **Capa de Presentación**: Controllers, middleware, configuración

### Patrones Implementados
- ✅ **Repository Pattern**: Abstracción del acceso a datos
- ✅ **Use Case Pattern**: Lógica de negocio encapsulada
- ✅ **DTO Pattern**: Transferencia de datos entre capas
- ✅ **Mapper Pattern**: Conversión automática con AutoMapper
- ✅ **Dependency Injection**: Inversión de control con .NET DI
- ✅ **Exception Handling**: Middleware global para manejo de errores

### Tecnologías Utilizadas

#### Backend
- **.NET 8.0**: Framework principal
- **Entity Framework Core 8.0.2**: ORM para acceso a datos
- **MySQL**: Base de datos principal (con Pomelo.EntityFrameworkCore.MySql 8.0.2)
- **AutoMapper 12.0.1**: Mapeo automático de objetos
- **FluentValidation 11.3.1**: Validación de requests
- **DotNetEnv 3.1.1**: Manejo de variables de entorno

#### Testing
- **xUnit 2.5.3**: Framework de testing
- **Moq 4.20.69**: Mocking de dependencias
- **FluentAssertions 6.12.0**: Assertions más expresivas
- **Microsoft.EntityFrameworkCore.Sqlite 8.0.2**: Base de datos en memoria para tests
- **Microsoft.AspNetCore.Mvc.Testing 8.0.0**: Tests de integración

#### Herramientas de Desarrollo
- **Swagger/OpenAPI (Swashbuckle 6.6.2)**: Documentación interactiva de la API
- **Entity Framework Migrations**: Versionado y evolución de base de datos
- **Global Exception Middleware**: Manejo centralizado de errores
- **Hot Reload**: Desarrollo con recarga automática



## 🔧 Comandos de Desarrollo

```bash
# Compilar solución completa
dotnet build

# Ejecutar en modo desarrollo con hot reload
dotnet watch run --project src/GestorDeUsuarios.API

# Limpiar y reconstruir
dotnet clean && dotnet build

# Crear nueva migración
dotnet ef migrations add NombreMigracion --project src/GestorDeUsuarios.Infrastructure

# Aplicar migraciones
dotnet ef database update --project src/GestorDeUsuarios.Infrastructure

# Generar script SQL de migraciones
dotnet ef migrations script --project src/GestorDeUsuarios.Infrastructure
```

## �️ Solución de Problemas Comunes

### Error: "No se encontró la variable DB_CONNECTION_STRING"
- ✅ Verifica que el archivo `.env` esté en la raíz del proyecto
- ✅ Asegúrate de que la variable esté escrita correctamente (sin espacios)
- ✅ Reinicia la aplicación después de crear el archivo `.env`

### Error de conexión a MySQL
- ✅ Verifica que MySQL esté ejecutándose: `sudo service mysql start` (Linux) o verificar en Servicios (Windows)
- ✅ Confirma las credenciales en el archivo `.env`
- ✅ Asegúrate de que el puerto 3306 esté disponible

### Error de migraciones
- ✅ Instala EF Core tools: `dotnet tool install --global dotnet-ef`
- ✅ Ejecuta desde la carpeta correcta: `cd src/GestorDeUsuarios.API`
- ✅ Verifica que la BD exista antes de aplicar migraciones

### Tests fallan
- ✅ Ejecuta `dotnet restore` en la raíz del proyecto
- ✅ Verifica que no haya otros procesos usando la BD de tests
- ✅ Limpia y recompila: `dotnet clean && dotnet build`

## �🚀 Deploy y Producción

### Variables de Entorno Requeridas
```bash
# Base de datos
DB_CONNECTION_STRING=Server=prod-server;Database=gestor_usuarios;User=prod_user;Password=secure_password;

# Logging (opcional)
ASPNETCORE_ENVIRONMENT=Production
```

### Docker (Opcional)
```dockerfile
# Ejemplo de Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "GestorDeUsuarios.API.dll"]
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear un Pull Request

### Convenciones de Código
- Seguir las convenciones de C# y .NET
- Todos los métodos públicos deben tener tests
- Mantener cobertura de tests > 80%
- Usar FluentValidation para validaciones
- Seguir principios SOLID

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 👨‍💻 Autor

**Agustín Leonardi**
- GitHub: [@agustinleonardi](https://github.com/agustinleonardi)
- Proyecto: [GestorDeUsuarios-Evoltis](https://github.com/agustinleonardi/GestorDeUsuarios-Evoltis)
