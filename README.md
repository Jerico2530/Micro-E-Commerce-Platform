# 🧠 Micro-Commerce Lite - Backend de E-commerce

Mini backend de E-commerce construido con **C# (.NET 8), PostgreSQL y Docker**, usando **microservicios independientes, escalables y seguros**.

---

## 👉 Visión General

Este sistema permite a un usuario:

- Autenticarse
- Consultar productos
- Crear órdenes de compra

Todo esto usando **microservicios independientes**, cada uno con su **propia base de datos**, comunicación **REST**, y seguridad con **JWT + BCrypt**.

---

## 🧱 Principios Clave

Cada microservicio:

- Tiene **una sola responsabilidad**.
- Posee **su propia base de datos**.
- Se comunica con otros mediante **HTTP (REST)**.
- Puede escalar de manera independiente.
- Usa **roles y permisos (PermRol)** para control de acceso.

Esto refleja un diseño profesional y conocimiento sólido de microservicios.

---

## 1️⃣ Identity Service

*"El cerebro de seguridad del sistema"*

### 🎯 Responsabilidad
- Gestionar **identidad y autorización** de usuarios.
- NO gestiona productos ni órdenes.

### 📦 Modelo
- Usuario
- Rol
- Permiso
- UserRol
- PermRol

### 🔐 Funciones Clave
- **Autenticación**
  - Login
  - Generación de JWT seguro
  - Contraseñas con BCrypt
- **Autorización**
  - Validación de roles y permisos
  - Ejemplo de JWT emitido:

```json
{
  "userId": 1,
  "email": "user@mail.com",
  "roles": ["Admin"],
  "permissions": ["CREATE_PRODUCT", "CREATE_ORDER"]
}
