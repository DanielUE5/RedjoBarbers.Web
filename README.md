# ✂️ Redjo Barbers

ASP.NET Core MVC Web Application (.NET 10)

------------------------------------------------------------------------

# 🇧🇬 Документация (Български)

## 📌 Обща информация

**Redjo Barbers** е уеб приложение за управление на барбършоп,
разработено с **ASP.NET Core MVC (.NET 10)**, използващо **Entity
Framework Core (Code First)** и **ASP.NET Core Identity**.

Приложението за сега е предназначено за работа на **локален сървър** със SQL
Server / LocalDB.

------------------------------------------------------------------------

## 🛠️ Използвани технологии

-   .NET 10
-   ASP.NET Core MVC
-   Entity Framework Core
-   ASP.NET Core Identity
-   SQL Server / LocalDB
-   Bootstrap 5
-   Razor Views

------------------------------------------------------------------------

## 🔐 Ролева система

Проектът използва **Role-based Authorization** чрез ASP.NET Core
Identity.

Поддържани роли:

-   `User`
-   `Admin`

Достъпът до административния панел е ограничен само за потребители с
роля **Admin**.

Във файла `appsettings.json` е добавен примерен администраторски имейл,
който може да бъде използван за вход като администратор (ако съответният
потребител съществува в базата данни).

------------------------------------------------------------------------

## 🏗️ Архитектура

Проектът следва стандартната MVC архитектура:

Controllers → Business Logic → Data Layer → Database\
Views → Razor Rendering\
ViewModels → Presentation Layer

------------------------------------------------------------------------

## 🧩 Основни функционалности

### 👤 Потребители

-   Регистрация
-   Вход / Изход
-   Ролева система (Admin / User)

### 📅 Запазване на час

-   Създаване на резервация
-   Редактиране
-   Изтриване
-   Преглед на „Моите часове"

### ⭐ Отзиви

-   Създаване
-   Редактиране
-   Преглед на всички отзиви

### 🛠️ Административен панел

Достъпен само за потребители с роля **Admin**.

------------------------------------------------------------------------

## 🗄️ База данни

Проектът използва **Entity Framework Core -- Code First**.

Основни модели: - Barber - BarberService - BarberBarberService -
Appointment - Review

Enum: - AppointmentStatus

------------------------------------------------------------------------

## ⚙️ Стартиране на приложението (Local Setup)

### Изисквания

-   .NET 10 SDK
-   SQL Server или LocalDB
-   Visual Studio 2022 / 2025

------------------------------------------------------------------------

### 1️⃣ Конфигурация

Провери файла:

`appsettings.json`

Там се намират:

-   Connection string към базата данни
-   Примерен администраторски имейл

Пример:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=RedjoBarbersDb;Trusted_Connection=True;"
}
```

------------------------------------------------------------------------

### 2️⃣ Автоматично прилагане на миграции

Проектът използва:

``` csharp
Database.MigrateAsync()
```

При стартиране на приложението:

-   Ако има съществуваща миграция, която не е приложена към базата
    данни,
-   Тя ще бъде автоматично приложена при стартиране,
-   Без необходимост от ръчно изпълнение на `Update-Database` в PMC.

⚠️ Ръчно изпълнение на `Add-Migration` е необходимо
само при създаване на нова миграция.

------------------------------------------------------------------------

### 3️⃣ Стартиране

През Visual Studio: F5

През CLI: dotnet run

Приложението ще се стартира на: https://localhost:{port}

------------------------------------------------------------------------

## 🎯 Цел на проекта

Проектът демонстрира: - MVC архитектура - CRUD операции - Работа с EF
Core - Role-based оторизация - Identity интеграция

------------------------------------------------------------------------

## ℹ️ Допълнителна информация

Този проект е създаден за реален барбършоп, собственост на близък човек.
Обектът съществува реално, както и посочената в проекта локация, имената
и линкът към социалната мрежа (добавен е един реален линк).

⚠️ Телефонните номера и снимките на бръснарите в проекта са примерни и
се използват единствено за демонстрационни цели.

------------------------------------------------------------------------

# 🇬🇧 Documentation (English)

## 📌 Overview

**Redjo Barbers** is a barber shop management web application built with
**ASP.NET Core MVC (.NET 10)** using **Entity Framework Core (Code
First)** and **ASP.NET Core Identity**.

The application for now is designed to run on a **local server** using SQL
Server / LocalDB.

------------------------------------------------------------------------

## 🔐 Role-Based Authorization

The project implements **Role-based authorization** using ASP.NET Core
Identity.

Supported roles:

-   `User`
-   `Admin`

The Admin Panel is accessible only to users with the **Admin** role.

The `appsettings.json` file contains a sample administrator email which
can be used for admin login (if the corresponding user exists in the
database).

------------------------------------------------------------------------

## 🛠️ Tech Stack

-   .NET 10
-   ASP.NET Core MVC
-   Entity Framework Core
-   ASP.NET Core Identity
-   SQL Server / LocalDB
-   Bootstrap 5
-   Razor Views

------------------------------------------------------------------------

## 🧩 Core Features

### 👤 Users

-   Registration
-   Login / Logout
-   Role-based system (Admin / User)

### 📅 Appointments

-   Create appointment
-   Edit appointment
-   Delete appointment
-   View personal appointments

### ⭐ Reviews

-   Create review
-   Edit review
-   View all reviews

### 🛠️ Admin Panel

Accessible only to users with the **Admin** role.

------------------------------------------------------------------------

## 🗄️ Database

The project uses **Entity Framework Core -- Code First**.

Main entities: - Barber - BarberService - BarberBarberService -
Appointment - Review

Enum: - AppointmentStatus

------------------------------------------------------------------------

## ⚙️ Running the Application (Local Setup)

### Requirements

-   .NET 10 SDK
-   SQL Server or LocalDB
-   Visual Studio 2022 / 2025

------------------------------------------------------------------------

### 1️⃣ Configuration

Check the:

`appsettings.json`

It contains:

-   Database connection string
-   Sample administrator email

------------------------------------------------------------------------

### 2️⃣ Automatic Migration Handling

The project uses:

``` csharp
Database.MigrateAsync()
```

On application startup:

-   If there is an existing migration that has not been applied,
-   It will automatically be applied to the database,
-   No need to manually run `Update-Database`.

⚠️ Manual migration command `Add-Migration` is required only when creating a new
migration.

------------------------------------------------------------------------

### 3️⃣ Run the Application

Visual Studio: F5

CLI: dotnet run

The application will start at: https://localhost:{port}

------------------------------------------------------------------------

## 🎯 Project Purpose

This project demonstrates: - ASP.NET Core MVC architecture - CRUD
operations - EF Core Code First approach - Identity authentication -
Role-based authorization

------------------------------------------------------------------------

## ℹ️ Additional Information

This project was developed for a real barber shop owned by a close
relative. The physical location, names, and the social media link
included in the project (only one real link is provided) correspond to a
real existing business.

⚠️ The phone numbers and barber images included in the project are
sample data and are used strictly for demonstration purposes.
