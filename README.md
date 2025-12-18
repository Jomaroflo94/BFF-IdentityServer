# BFF-IdentityServer

**Ejemplo básico de un Identity Provider (IDP) con Proxy BFF (Backend for Frontend) inverso implementado con . NET 8, Duende IdentityServer y React.**

## 🏷️ Tecnologías y Badges

### Backend
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Duende IdentityServer](https://img.shields.io/badge/Duende%20IdentityServer-7.0.4-orange?logo=shield&logoColor=white)](https://duendesoftware.com/products/identityserver)
[![Duende BFF](https://img.shields.io/badge/Duende%20BFF-2.2.0-orange?logo=shield&logoColor=white)](https://duendesoftware.com/products/bff)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework%20Core-8.0-512BD4?logo=microsoft&logoColor=white)](https://docs.microsoft.com/ef/)
[![Serilog](https://img.shields.io/badge/Serilog-8.0-00A4EF?logo=serilog&logoColor=white)](https://serilog.net/)
[![YARP](https://img.shields.io/badge/YARP-Reverse%20Proxy-0078D4?logo=microsoft&logoColor=white)](https://microsoft.github.io/reverse-proxy/)

### Frontend
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react&logoColor=black)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.4-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-5.2-646CFF?logo=vite&logoColor=white)](https://vitejs.dev/)
[![React Query](https://img.shields.io/badge/React%20Query-3.39-FF4154?logo=react-query&logoColor=white)](https://tanstack.com/query/latest)
[![Axios](https://img.shields.io/badge/Axios-1.7-5A29E4?logo=axios&logoColor=white)](https://axios-http.com/)
[![React Router](https://img.shields.io/badge/React%20Router-6.23-CA4245?logo=react-router&logoColor=white)](https://reactrouter.com/)

### Base de Datos
[![SQLite](https://img.shields.io/badge/SQLite-InMemory-003B57?logo=sqlite&logoColor=white)](https://www.sqlite.org/)

### Seguridad y Autenticación
[![OAuth 2.0](https://img.shields.io/badge/OAuth-2.0-3C873A?logo=oauth&logoColor=white)](https://oauth.net/2/)
[![OpenID Connect](https://img.shields.io/badge/OpenID%20Connect-OIDC-F78C40?logo=openid&logoColor=white)](https://openid.net/connect/)
[![JWT](https://img.shields.io/badge/JWT-Bearer-000000?logo=json-web-tokens&logoColor=white)](https://jwt.io/)

### Licencia
[![License](https://img.shields.io/badge/license-Educational%20Use%20Only-yellow.svg)](LICENSE)
[![Commercial License Required](https://img.shields.io/badge/Production-Commercial%20License%20Required-red.svg)](https://duendesoftware.com/products/identityserver)

---

## ⚠️ AVISO IMPORTANTE - LICENCIAS Y USO

### 🔴 Este proyecto utiliza software con licencias comerciales

**Este proyecto ha sido desarrollado ÚNICAMENTE con fines educativos y de aprendizaje.**

#### Duende IdentityServer - Licencia Comercial

Este proyecto utiliza **Duende IdentityServer** (versión 7.0.4) y **Duende BFF** (versión 2.2.0), que requieren una **licencia comercial** para uso en producción: 

📄 Más información:
-   [https://duendesoftware.com/products/identityserver](https://duendesoftware.com/products/identityserver)
-   [https://duendesoftware.com/products/bff](https://duendesoftware.com/products/bff)


```
ESTE PROYECTO ES ÚNICAMENTE PARA FINES EDUCATIVOS Y DE DEMOSTRACIÓN. 

- NO está listo para producción sin la adquisición de las licencias apropiadas
- El autor NO se hace responsable del uso indebido de este código
- Es responsabilidad del usuario asegurar el cumplimiento de todas las licencias
- NO se proporciona ninguna garantía, expresa o implícita
- El código se proporciona "TAL CUAL" (AS-IS)
```

## 📋 Descripción del Proyecto

Este proyecto demuestra una arquitectura de seguridad completa utilizando el patrón BFF (Backend for Frontend) con un servidor de identidad basado en Duende IdentityServer.  La solución consta de tres componentes principales que trabajan juntos para proporcionar autenticación y autorización segura para una aplicación web SPA.

### Arquitectura

```
┌─────────────────┐
│   Client.Web    │  ← Frontend React (Puerto 5173)
│   (React/Vite)  │
└────────┬────────┘
         │
         │ HTTPS
         ▼
┌─────────────────┐
│   BFF.Proxy     │  ← Backend for Frontend (Puerto 7291)
│   (ASP.NET 8)   │
└────┬─────┬──────┘
     │     │
     │     └──────────┐
     │                │
     │ HTTPS          │ HTTPS
     ▼                ▼
┌─────────────┐  ┌──────────────┐
│ IDP.WebApp  │  │ Resource.Api │  ← API de Recursos (Puerto 7293)
│ (Identity   │  │ (ASP.NET 8)  │
│  Server)    │  └──────────────┘
│ (Puerto     │
│  7292)      │
└─────────────┘
```

## 🏗️ Componentes del Sistema

### 1. **IDP.WebApp** - Servidor de Identidad

Servidor de autenticación basado en Duende IdentityServer 7.0.4 que gestiona:

- **Autenticación de usuarios** utilizando ASP.NET Identity
- **Emisión de tokens** (Access Tokens, ID Tokens, Refresh Tokens)
- **OAuth 2.0 y OpenID Connect** flows
- **Base de datos** SQLite para almacenamiento de usuarios y configuración
- **Páginas Razor** para UI de login, consentimiento y gestión de dispositivos

**Características:**
- Flujos de autenticación:  Authorization Code, CIBA, Device Flow
- Gestión de consentimiento de usuarios
- Integración con proveedores externos (ej: Google)
- Diagnósticos y páginas de error personalizadas

### 2. **BFF.Proxy** - Backend for Frontend

Proxy inverso que actúa como intermediario seguro entre el frontend y los servicios backend:

- **Gestión de sesiones** mediante cookies HTTP-only
- **Protección CSRF** con tokens anti-forgery
- **Proxy inverso** usando Duende. BFF y YARP (Yet Another Reverse Proxy)
- **Endpoints de gestión BFF** (/bff/login, /bff/logout, /bff/user)
- **Token management** automático (access tokens, refresh tokens)

**Ventajas del patrón BFF:**
- Los tokens nunca se exponen al navegador
- Protección contra ataques XSS y CSRF
- Simplificación de la lógica del cliente
- Gestión centralizada de la seguridad

### 3. **Resource.Api** - API de Recursos

API REST protegida que expone recursos (en este caso, lenguajes de programación):

- **Endpoints CRUD** para gestión de lenguajes de programación
- **Autenticación JWT Bearer** 
- **Autorización basada en scopes** (read, write, update, delete)
- **Base de datos en memoria** (Entity Framework Core InMemory)
- **Documentación Swagger** con autenticación OAuth2

**Endpoints disponibles:**
```
GET    /api/ProgrammingLanguage      → Obtener todos (scope: read)
GET    /api/ProgrammingLanguage/{id} → Obtener por ID (scope: read)
POST   /api/ProgrammingLanguage      → Crear nuevo (scope: write)
PUT    /api/ProgrammingLanguage      → Actualizar (scope: update)
DELETE /api/ProgrammingLanguage/{id} → Eliminar (scope: delete)
```

### 4. **Client.Web** - Aplicación Frontend

Aplicación SPA (Single Page Application) construida con React y TypeScript:

- **Interfaz de usuario** para login/logout
- **Consumo de API** protegida mediante el BFF
- **React Query** para gestión de estado y caché
- **Axios** para llamadas HTTP
- **Vite** como build tool y dev server

## 🚀 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18 o superior)
- [Git](https://git-scm.com/)

## 📦 Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/Jomaroflo94/BFF-IdentityServer.git
cd BFF-IdentityServer
```

### 2. Configurar el Identity Server

```bash
cd IDP.WebApp

# Inicializar la base de datos con usuarios de prueba
dotnet run /seed

# El comando anterior creará: 
# - Usuario: alice / Pass: alice
# - Usuario: bob / Pass:  bob
```

### 3. Iniciar los servicios backend

Necesitarás **tres terminales** para ejecutar los servicios:

**Terminal 1 - Identity Server:**
```bash
cd IDP.WebApp
dotnet run
# Se ejecutará en https://localhost:7292
```

**Terminal 2 - BFF Proxy:**
```bash
cd BFF.Proxy
dotnet run
# Se ejecutará en https://localhost:7291
```

**Terminal 3 - Resource API:**
```bash
cd Resource.Api
dotnet run
# Se ejecutará en https://localhost:7293
```

### 4. Configurar y ejecutar el cliente web

**Terminal 4 - Frontend React:**
```bash
cd Client.Web
npm install
npm run dev
# Se ejecutará en https://localhost:5173
```

## 🔧 Configuración

### Variables de Entorno

Cada proyecto utiliza variables de entorno definidas en `launchSettings.json`:

**BFF. Proxy:**
```json
{
  "IDP_BASE_ADDRESS": "https://localhost:7292",
  "BFF_CLIENT_ID": "BFF.Proxy",
  "BFF_CLIENT_SECRET": "Secret.BFF.Proxy",
  "X_CSRF_VALUE": "testCSRF",
  "RESOURCE_BASE_ADDRESS": "https://localhost:7293"
}
```

**Resource.Api:**
```json
{
  "AUTHORITY": "https://localhost:7292",
  "CLIENT_ID":  "Swagger. Resource.Api",
  "CLIENT_SECRET": "Secret. Swagger.Resource.Api",
  "JWT_AUDIENCE": "Resource.Api",
  "JWT_TYPES": "at+jwt"
}
```

## 🎯 Uso

### Flujo de Autenticación

1. **Accede a la aplicación**:  Abre tu navegador en `https://localhost:5173`
2. **Haz clic en Login**:  Serás redirigido al Identity Server
3. **Inicia sesión** con las credenciales: 
   - Usuario: `alice` / Contraseña: `alice`
   - Usuario: `bob` / Contraseña: `bob`
4. **Autoriza la aplicación**:  Otorga los permisos solicitados
5. **Accede a los recursos**: Una vez autenticado, podrás ver la lista de lenguajes de programación

### Probar la API directamente

Puedes acceder a Swagger UI para probar los endpoints:

- **Resource API**: `https://localhost:7293/swagger`
- **BFF Proxy**: `https://localhost:7291/swagger`

## 🔐 Seguridad

### Scopes Implementados

El proyecto utiliza los siguientes scopes para control de acceso:

- `openid` - Identificación básica del usuario
- `profile` - Información del perfil del usuario
- `offline_access` - Refresh tokens
- `resource. api. read` - Lectura de recursos
- `resource.api.write` - Creación de recursos
- `resource.api.update` - Actualización de recursos
- `resource.api.delete` - Eliminación de recursos
- `resource.api.all` - Acceso completo

### Protección CSRF

El BFF implementa protección CSRF mediante el header `X-CSRF` que debe incluirse en todas las peticiones desde el cliente:

```typescript
const requestConfig = {
  headers: {
    "X-CSRF":  "testCSRF"
  }
};
```

### Cookies Seguras

Las cookies de autenticación están configuradas con: 
- `SameSite:  Strict`
- `HttpOnly: true`
- `Secure: true`
- Expiración: 8 horas

## 📚 Conceptos Clave

### ¿Qué es BFF (Backend for Frontend)?

El patrón BFF es una arquitectura donde se crea un backend específico para cada tipo de frontend.  En este caso, el BFF: 

1. **Gestiona la autenticación**:  Mantiene la sesión del usuario mediante cookies
2. **Protege los tokens**: Los access tokens nunca llegan al navegador
3. **Actúa como proxy**: Reenvía las peticiones a las APIs backend agregando los tokens necesarios
4. **Simplifica el cliente**: El frontend no necesita lógica compleja de OAuth2

### Flujo de Tokens

```
1. Usuario → Login → Identity Server
2. Identity Server → Emite tokens → BFF Proxy
3. BFF Proxy → Guarda tokens → Cookie de sesión
4. Cliente → Petición con cookie → BFF Proxy
5. BFF Proxy → Petición con token → Resource API
6. Resource API → Respuesta → BFF Proxy → Cliente
```

## 👤 Autor

**Jomaroflo94**
- GitHub: [@Jomaroflo94](https://github.com/Jomaroflo94)

## 📚 Recursos Adicionales

### Documentación Técnica
- [Documentación de Duende IdentityServer](https://docs.duendesoftware.com/identityserver/v7)
- [Patrón BFF](https://docs.duendesoftware.com/identityserver/v7/bff/)
- [OAuth 2.0 y OpenID Connect](https://oauth.net/2/)
- [Guía de React](https://react.dev/)

### Licencias y Compra
- [Información de Licencias Duende Software](https://duendesoftware.com/products/identityserver)
- [Preguntas Frecuentes sobre Licencias](https://duendesoftware.com/products/faq)
- [Calculadora de Precios](https://duendesoftware.com/products/pricing)

---

⚠️ **RECUERDA**: Este proyecto es solo para desarrollo y aprendizaje.  Para producción, adquiere las licencias necesarias. 

⭐ Si este proyecto te resulta útil para aprender, considera darle una estrella en GitHub! 
