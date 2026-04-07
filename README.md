# 💈 Redjo Barbers

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

**RedjoBarbers** е пълнофункционално *ASP.NET Core* уеб приложение, предназначено за управление на модерна бръснарница. Платформата позволява на потребителите да разглеждат услуги, да си запазват час, да оставят отзиви и да взаимодействат с бръснари, докато администраторите управляват цялата система чрез специална администраторска зона.

Проектът демонстрира напреднали умения за разработка с *ASP.NET Core*, включително многопластова архитектура, инжектиране на зависимости, сигурност, валидиране, тестване и внедряване.

---

## 🎯 Функции

### 👤 Функции за потребителя
- Регистрация / Вход (ASP.NET Identity)
- Разглеждане на бръснарски услуги
- Запазване на час
- Оставяне на отзиви
- Филтриране и номериране на данни

### 🛠️ Функции за администратор
- Управление на услуги
- Управление на срещи
- Административно табло
- Контрол на достъпа, базиран на роли

---

## 🏗️ Архитектура

Приложението следва многопластова архитектура:

- Уеб слой – Контролери, Изгледи, Области
- Слой за услуги – Бизнес логика (Services.Core)
- Слой за данни – EF Core, DbContext
- Слой за модели – Модели на обекти
- Слой за ViewModels – Модели, специфични за потребителския интерфейс

---

## 🧩 Използвани технологии

- ASP.NET Core (.NET 10)  
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Identity
- Razor Views
- Bootstrap
- NUnit
- Coverlet + ReportGenerator 

---

## 🔐 Сигурност и валидиране

- ASP.NET удостоверяване на самоличността и роли (потребител / администратор)
- Токени против фалшифициране
- Защита срещу:
- SQL инжектиране
- XSS (Cross-Site Scripting)
- CSRF
- Подмяна на параметри
- Валидиране от страна на сървъра и клиента

---

## 🗄️ База данни и начално ниво

Приложението използва Entity Framework Core със SQL Server.

---

## 📄 Страници и структура

- 10+ преглеждания
- 5+ контролера
- MVC области (Администрация, Идентичност)

---

## ⚙️ Конфигурация на база данни

Проектът използва **User Secrets (secrets.json)** за съхранение на connection string с цел по-добра сигурност.

⚠️ Важно:

- Всеки разработчик трябва да си конфигурира собствен connection string, въпреки, че ще има fallback такъв.

Възможни варианти къде да го направите:

- appsettings.json  
- appsettings.Development.json  
- User Secrets (препоръчително, ако е за production)  

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

- Филтриране и номериране на страници, внедрени където е приложимо.

⚠️ Няма реализирана текстова търсачка, защото структурата не ми го позволява

---

## 📑 Добавени custom страници за грешки (404 и 500)

В проекта са създадени и интегрирани персонализирани (custom) страници за обработка на грешки 404 (Not Found) и 500 (Internal Server Error).

---

## 🧪 Тестване и покритие

- Модулни тестове
- Интеграционни тестове
- ~99% покритие на слоя с услуги (ако тестовете не са грешни)

---

## 🌐 Разгръщане

GitHub хранилище:
https://github.com/DanielUE5/RedjoBarbers.Web

---

## 📦 Source Control

- 30+ комита  
- 7+ дни  

---

## 👨‍💻 Автор на уеб приложението
- Даниел

---

# 🇬🇧 Documentation (English)

## 📌 Project Overview

**RedjoBarbers** is a full-stack *ASP.NET Core* web application designed to manage a modern barber shop. The platform allows users to browse services, book appointments, leave reviews, and interact with barbers, while administrators manage the entire system through a dedicated admin area.

The project demonstrates advanced *ASP.NET Core* development skills, including layered architecture, dependency injection, security, validation, testing, and deployment.

---

## 🎯 Features

### 👤 User Features
- Register / Login (ASP.NET Identity)
- Browse barber services
- Book appointments
- Leave reviews
- Filter and paginate data

### 🛠️ Admin Features
- Manage services
- Manage appointments
- Administrative dashboard
- Role-based access control

---

## 🏗️ Architecture

The application follows a layered architecture:

- Web Layer – Controllers, Views, Areas
- Services Layer – Business logic (Services.Core)
- Data Layer – EF Core, DbContext
- Models Layer – Entity models
- ViewModels Layer – UI-specific models

---

## 🧩 Technologies Used

- ASP.NET Core (.NET 6+)
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Identity
- Razor Views
- Bootstrap
- xUnit
- Coverlet + ReportGenerator

---

## 🔐 Security & Validation

- ASP.NET Authentication and Roles (User/Admin)
- Anti-Forgery Tokens
- Protection against:
- SQL Injection
- XSS (Cross-Site Scripting)
- CSRF
- Parameter Substitution
- Server-side and Client-side Validation

---

## 🗄️ Database & Seeding

The application uses Entity Framework Core with SQL Server.

---

## 📄 Pages & Structure

- 10+ Views
- 5+ Controllers
- MVC Areas (Admin, Identity)

---

## 🔎 Filtering & Pagination

Filtering and pagination implemented where applicable.

---

## 🧪 Testing & Coverage

- Unit Tests
- Integration Tests
- ~99% coverage on services layer

Run tests:

dotnet test

---

## ⚙️ Database Configuration

The project uses **User Secrets (secrets.json)** to store the connection string for better security.

⚠️ Important:

- Each developer must configure their own connection string, although there will be a backup one.

Possible options where to do it:

- appsettings.json
- appsettings.Development.json
- User Secrets (recommended if for production)

---

## 🌐 Deployment

GitHub repository:
https://github.com/DanielUE5/RedjoBarbers.Web

---

## 👨‍💻 Web Application Author
- Daniel
