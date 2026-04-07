# ✂️ Redjo Barbers

> 🇧🇬 Уеб приложение за управление на барбършоп  
> 🇬🇧 Barber shop management web application

![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)
![EF Core](https://img.shields.io/badge/EF_Core-Code_First-informational)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-red)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📋 Table of Contents / Съдържание

- 🇧🇬 [Документация (Български)](#-документация-български)
- 🇬🇧 [Documentation (English)](#-documentation-english)

---

# 🇧🇬 Документация (Български)

## 📌 Обща информация

**Redjo Barbers** е уеб приложение за управление на барбършоп, разработено с ASP.NET Core MVC (.NET 10), използващо Entity Framework Core (Code First) и ASP.NET Core Identity.

---

## 🎯 Цел на проекта

- MVC архитектура  
- EF Core (Code First)  
- Identity и роли  
- Реална бизнес логика  

---

## 🛠️ Технологии

- .NET 10  
- ASP.NET Core MVC  
- Entity Framework Core  
- ASP.NET Core Identity  
- SQL Server / LocalDB  
- Bootstrap 5  
- Razor Views  

---

## 🏗️ Архитектура

Controllers → Services → Data Layer → Database  
Views → Razor Rendering  
ViewModels → Presentation Layer  

---

## 🔐 Роли

- User  
- Admin  

---

## 🧱 Areas

- Admin  

---

## 🗄️ База данни

Модели:

- Barber  
- BarberService  
- BarberBarberService  
- Appointment  
- Review  

---

## 🔄 Миграции и Seeding

    Database.MigrateAsync();

---

## 🚀 Стартиране

    git clone ...
    dotnet restore
    dotnet run

---

## ⚙️ Конфигурация на база данни

Проектът използва **User Secrets (secrets.json)** за съхранение на connection string с цел по-добра сигурност.

Пример:

    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost\SQLEXPRESS01;Database=RedjoBarbers2026;Trusted_Connection=True;TrustServerCertificate=True;"
      }
    }

⚠️ Важно:

- Всеки разработчик трябва да си конфигурира собствен connection string  

Възможни варианти:

- appsettings.json  
- appsettings.Development.json  
- User Secrets (препоръчително)  

---

## 👑 Конфигурация на администратора

В **appsettings.json** е дефинирана секция **AdminSettings**, която съдържа имейл адреса на администратора.
Администратора трябва да се регистрира с този имейл, за да получи администраторските си права.

```json
{
  "AdminSettings": {
    "AdminEmail": "admin321@gmail.com"
  }
}
```
---

## 📋 Функционалности

- Резервации  
- Ревюта  
- Админ панел  

---

## 🔍 Филтриране

- Филтриране на отзиви по най-нови и с най-висок рейтинг  

⚠️ Няма реализирана текстова търсачка, защото структурата не ми го позволява

---

## 📄 Пагинация

- Услуги  
- Резервации  
- Ревюта  

---

## ✔️ Валидации

Client + Server  

---

## ⚠️ Грешки
Добавени са персонализирани страници за съответните грешки:
- 404 
- 500 

---

## 🛡️ Сигурност

- SQL Injection  
- XSS  (Cross-Site Scripting)
- CSRF  (Cross-Site Request Forgery)
- Parameter Tampering (промяна на параметри)

---

## 🧩 Dependency Injection

Вградената DI система  

---

## 🧪 Unit Tests и Coverage

- coverlet.collector  
- ReportGenerator  

---

## 🌐 Deployment

В момента само в GitHub.

---

## 📦 Source Control

- 30+ комита  
- 7+ дни  

---

# 🇬🇧 Documentation (English)

## 📌 Overview

**Redjo Barbers** is a barber shop management web application built with ASP.NET Core MVC (.NET 10), using Entity Framework Core (Code First) and ASP.NET Core Identity.

---

## 🎯 Project Purpose

- MVC architecture  
- EF Core (Code First)  
- Identity and role management  
- Real-world business logic  

---

## 🛠️ Technologies

- .NET 10  
- ASP.NET Core MVC  
- Entity Framework Core  
- ASP.NET Core Identity  
- SQL Server / LocalDB  
- Bootstrap 5  
- Razor Views  

---

## 🏗️ Architecture

Controllers → Services → Data Layer → Database  
Views → Razor Rendering  
ViewModels → Presentation Layer  

---

## 🔐 Roles

- User  
- Admin  

---

## 🧱 Areas

- Admin  

---

## 🗄️ Database

Models:

- Barber  
- BarberService  
- BarberBarberService  
- Appointment  
- Review  

---

## 🔄 Migrations and Seeding

    Database.MigrateAsync();

---

## 🚀 Run

    git clone ...
    dotnet restore
    dotnet run

---

## ⚙️ Database Configuration

The project uses **User Secrets (secrets.json)** to store the connection string for better security.

Example:

    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost\\SQLEXPRESS01;Database=RedjoBarbers2026;Trusted_Connection=True;TrustServerCertificate=True;"
      }
    }

⚠️ Important:

- Each developer must configure their own connection string  

Possible options:

- appsettings.json  
- appsettings.Development.json  
- User Secrets (recommended)  

---

## 👑 Admin Configuration

In **appsettings.json**, there is an **AdminSettings** section that contains the administrator email.

The administrator must register using this email in order to receive admin privileges.

```json
{
  "AdminSettings": {
    "AdminEmail": "admin321@gmail.com"
  }
}
