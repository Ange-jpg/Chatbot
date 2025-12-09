# Chatbot de Reportes – WhatsApp Cloud API

Backend desarrollado en **ASP.NET Core Web API** para la gestión de reportes ciudadanos, integrado con **PostgreSQL** y conectado a **WhatsApp Cloud API** mediante webhooks para recibir y procesar mensajes.

Este proyecto forma parte de un chatbot que permite a los usuarios enviar reportes vía WhatsApp.

---

##  Descripción general

El sistema recibe mensajes desde WhatsApp, procesa la información y registra reportes en una base de datos PostgreSQL.  
Actualmente se cuenta con:

- API REST en .NET
- Base de datos PostgreSQL
- Swagger para pruebas
- Webhook funcional de WhatsApp Cloud API
- Túnel con ngrok para desarrollo local

---

##  Tecnologías usadas

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **Npgsql**
- **Swagger / OpenAPI**
- **WhatsApp Cloud API (Meta)**
- **ngrok** (entorno local)

---

##  Estructura del proyecto

ReportesApi/
│
├── Controllers/
│ ├── ReportesController.cs
│ └── WhatsAppWebhookController.cs
│
├── Models/
│ └── Reporte.cs
│
├── Data/
│ └── AppDbContext.cs
│
├── Services/
│ ├── FolioService.cs
│ └── WhatsAppService.cs
│
├── appsettings.json -- no incluido
└── Program.cs


---

##  Base de datos

Tabla principal: **reportes**

Campos relevantes:
- `id`
- `folio` (varchar 150)
- `categoria`
- `subcategoria`
- `telefono` (10 dígitos)
- `correo`
- `direccion`
- `referencias`
- `descripcion_reporte`
- `fecha_creacion` (UTC)
- `ruta_imagen`

---

🚧 Estado del proyecto

🟡 En desarrollo

Autora

Lic. Angelica Ines Caiceros Ruiz
Proyecto de chatbot con integración a WhatsApp Cloud API.



