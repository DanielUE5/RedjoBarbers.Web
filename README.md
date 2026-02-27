# ✂️ Redjo Barbers

> 🇧🇬 Уеб приложение за управление на барбършоп -- резервации, отзиви и
> административен панел.\
> 🇬🇧 Barber shop management web application -- appointments, reviews and
> admin panel.

![.NET Version](https://img.shields.io/badge/.NET-10.0-purple) ![ASP.NET
Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue) ![EF
Core](https://img.shields.io/badge/EF_Core-Code_First-informational)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-red)
![License](https://img.shields.io/badge/license-MIT-green)

------------------------------------------------------------------------

## 📋 Table of Contents

-   🇧🇬 Документация (Български)
-   🇬🇧 Documentation (English)

------------------------------------------------------------------------

# 🇧🇬 Документация (Български)

## 📌 Обща информация

**Redjo Barbers** е уеб приложение за управление на барбършоп,
разработено с **ASP.NET Core MVC (.NET 10)**, използващо **Entity
Framework Core (Code First)** и **ASP.NET Core Identity**.

Приложението се стартира локално на:

https://localhost:7137

------------------------------------------------------------------------

## 🛠️ Използвани технологии

-   .NET 10
-   ASP.NET Core MVC
-   Entity Framework Core (Code First)
-   ASP.NET Core Identity
-   SQL Server / LocalDB
-   Bootstrap 5
-   Razor Views

------------------------------------------------------------------------

## 🔐 Ролева система

Поддържани роли:

-   User
-   Admin

Само потребители с роля **Admin** имат достъп до административния панел.

------------------------------------------------------------------------

## 🏗️ Архитектура

Controllers → Business Logic → Data Layer → Database\
Views → Razor Rendering\
ViewModels → Presentation Layer

------------------------------------------------------------------------

## 🗄️ База данни

Проектът използва **Entity Framework Core -- Code First**.

Основни модели:

-   Barber
-   BarberService
-   BarberBarberService
-   Appointment
-   Review

Enum: - AppointmentStatus

### 🔄 Автоматични миграции и Seeding

При стартиране на приложението автоматично се изпълнява:

    Database.MigrateAsync();

Това означава:

-   Всички pending миграции се прилагат автоматично
-   Базата данни се създава, ако не съществува
-   Изпълнява се автоматичен seeding (ролите `User` и `Admin`, начални
    данни и др.)

Ръчно изпълнение на `dotnet ef database update` не е необходимо в
стандартен сценарий.

------------------------------------------------------------------------

## 🚀 Инсталация и стартиране

    git clone https://github.com/your-username/your-repo-name.git
    cd your-repo-name
    dotnet restore
    dotnet run

Приложението ще бъде достъпно на:

https://localhost:7137

### ⚠️ Optional (при нужда)

    dotnet ef database update

------------------------------------------------------------------------

## ⚙️ Конфигурация

Файл: appsettings.json

    "ConnectionStrings": {
      "DevConnection": "Server=(localdb)\MSSQLLocalDB;Database=RedjoBarbersDb;Trusted_Connection=True;"
    }

------------------------------------------------------------------------

## 🎯 Цел на проекта

Проектът демонстрира:

-   ASP.NET Core MVC архитектура
-   CRUD операции
-   Работа с EF Core
-   Identity и Role-based Authorization

------------------------------------------------------------------------

# 🇬🇧 Documentation (English)

## 📌 Overview

**Redjo Barbers** is a barber shop management web application, developed
with **ASP.NET Core MVC (.NET 10)** using **Entity Framework Core (Code
First)** and **ASP.NET Core Identity**.

The application runs locally at:

https://localhost:7137

------------------------------------------------------------------------

## 🛠️ Technologies Used

-   .NET 10
-   ASP.NET Core MVC
-   Entity Framework Core (Code First)
-   ASP.NET Core Identity
-   SQL Server / LocalDB
-   Bootstrap 5
-   Razor Views

------------------------------------------------------------------------

## 🔐 Role System

Supported roles:

-   User
-   Admin

Only users with the **Admin** role have access to the administrative
panel.

------------------------------------------------------------------------

## 🏗️ Architecture

Controllers → Business Logic → Data Layer → Database\
Views → Razor Rendering\
ViewModels → Presentation Layer

------------------------------------------------------------------------

## 🗄️ Database

The project uses **Entity Framework Core -- Code First**.

Main models:

-   Barber
-   BarberService
-   BarberBarberService
-   Appointment
-   Review

Enum: - AppointmentStatus

### 🔄 Automatic Migrations and Seeding

On application startup, the following is executed automatically:

    Database.MigrateAsync();

This means:

-   All pending migrations are automatically applied
-   The database is created if it does not exist
-   Automatic seeding is executed (roles `User` and `Admin`, initial
    data, etc.)

Manual execution of `dotnet ef database update` is not required in the
standard scenario.

------------------------------------------------------------------------

## 🚀 Installation and Run

    git clone https://github.com/your-username/your-repo-name.git
    cd your-repo-name
    dotnet restore
    dotnet run

The application will be available at:

https://localhost:7137

### ⚠️ Optional (if needed)

    dotnet ef database update

------------------------------------------------------------------------

## ⚙️ Configuration

File: appsettings.json

    "ConnectionStrings": {
      "DevConnection": "Server=(localdb)\MSSQLLocalDB;Database=RedjoBarbersDb;Trusted_Connection=True;"
    }

------------------------------------------------------------------------

## 🎯 Project Purpose

This project demonstrates:

-   ASP.NET Core MVC architecture
-   CRUD operations
-   Working with EF Core
-   Identity and Role-based Authorization
