# GestorDeUsuarios-Evoltis

Sistema de gestión de usuarios desarrollado en .NET 8.0 con Entity Framework Core y MySQL.

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (versión 8.0 o superior)
- [Git](https://git-scm.com/downloads)
- IDE: [Visual Studio Code](https://code.visualstudio.com/) o [Visual Studio](https://visualstudio.microsoft.com/)

## 🚀 Configuración e Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/agustinleonardi/GestorDeUsuarios-Evoltis.git
cd GestorDeUsuarios-Evoltis
```

### 2. Configuración de la Base de Datos

#### 2.1 Configurar Variables de Entorno

Crear un archivo `.env` en la **raíz del proyecto** (mismo nivel que `GestorDeUsuarios.sln`):

```env
DB_CONNECTION_STRING=Server=localhost;Database=gestor_usuarios;User=gestor_user;Password=tu_password_segura;
```

### 3. Restaurar Dependencias

```bash
dotnet restore
```

### 4. Aplicar Migraciones de Base de Datos

Las migraciones se aplican automáticamente al ejecutar la aplicación, pero también puedes hacerlo manualmente:

```bash
cd src/GestorDeUsuarios.API
dotnet ef database update --project ../GestorDeUsuarios.Infrastructure
```

### 5. Ejecutar la Aplicación

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

El proyecto incluye las siguientes entidades:

### Tabla: Usuarios

- `Id` (int, PK, auto-increment)
- `Nombre` (varchar(100), obligatorio)
- `Email` (varchar(100), obligatorio, único)
- `FechaCreacion` (datetime)

### Tabla: Domicilios

- `Id` (int, PK, auto-increment)
- `Calle` (varchar(100), obligatorio)
- `Ciudad` (varchar(50), obligatorio)
- `CodigoPostal` (varchar(10), obligatorio)
- `UsuarioId` (int, FK a Usuarios)

### Construcción y Pruebas

```bash
# Compilar solución completa
dotnet build

# Ejecutar en modo desarrollo con hot reload
dotnet watch run --project src/GestorDeUsuarios.API

# Limpiar y reconstruir
dotnet clean && dotnet build
```
