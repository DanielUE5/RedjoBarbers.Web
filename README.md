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

-   [🇧🇬 Документация (Български)](#-документация-български)
    -   [Обща информация](#-обща-информация)
    -   [Използвани технологии](#-използвани-технологии)
    -   [Ролева система](#-ролева-система)
    -   [Архитектура](#-архитектура)
    -   [Основни функционалности](#-основни-функционалности)
    -   [База данни](#-база-данни)
    -   [Инсталация и стартиране](#-инсталация-и-стартиране)
    -   [Конфигурация](#-конфигурация)
    -   [Цел на проекта](#-цел-на-проекта)
-   [🇬🇧 Documentation (English)](#-documentation-english)
    -   [Overview](#-overview)
    -   [Tech Stack](#-tech-stack)
    -   [Role-Based Authorization](#-role-based-authorization)
    -   [Architecture](#-architecture-1)
    -   [Core Features](#-core-features)
    -   [Database](#-database-1)
    -   [Installation & Run](#-installation--run)
    -   [Project Purpose](#-project-purpose)

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

## ✨ Основни функционалности

### 👤 Потребители

-   Регистрация
-   Вход / Изход
-   Role-based система

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

-   Достъпен само за Admin
-   Управление на данни

------------------------------------------------------------------------

## 🗄️ База данни

Използва се **Entity Framework Core -- Code First**.

Основни модели:

-   Barber
-   BarberService
-   BarberBarberService
-   Appointment
-   Review

Enum: - AppointmentStatus

------------------------------------------------------------------------

## 🚀 Инсталация и стартиране

``` bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name
dotnet restore
dotnet ef database update
dotnet run
```

Application URL:

https://localhost:7137

------------------------------------------------------------------------

## ⚙️ Конфигурация

Файл: appsettings.json

``` json
"ConnectionStrings": {
  "DevConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RedjoBarbersDb;Trusted_Connection=True;"
}
```

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

**Redjo Barbers** is a barber shop management web application built with
**ASP.NET Core MVC (.NET 10)** using **Entity Framework Core (Code
First)** and **ASP.NET Core Identity**.

The application runs locally at:

https://localhost:7137

------------------------------------------------------------------------

## 🛠️ Tech Stack

-   .NET 10
-   ASP.NET Core MVC
-   Entity Framework Core (Code First)
-   ASP.NET Core Identity
-   SQL Server / LocalDB
-   Bootstrap 5
-   Razor Views

------------------------------------------------------------------------

## 🔐 Role-Based Authorization

Supported roles:

-   User
-   Admin

Only users with the **Admin** role can access the administrative
section.

------------------------------------------------------------------------

## 🏗️ Architecture

Controllers → Business Logic → Data Layer → Database\
Views → Razor Rendering\
ViewModels → Presentation Layer

------------------------------------------------------------------------

## ✨ Core Features

-   User registration and login
-   Role-based system (Admin / User)
-   Appointment management
-   Review management
-   Admin-only panel

------------------------------------------------------------------------

## 🗄️ Database

The project uses **Entity Framework Core -- Code First**.

Main entities:

-   Barber
-   BarberService
-   BarberBarberService
-   Appointment
-   Review

Enum: - AppointmentStatus

------------------------------------------------------------------------

## 🚀 Installation & Run

``` bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name
dotnet restore
dotnet ef database update
dotnet run
```

Application URL:

https://localhost:7137

------------------------------------------------------------------------

## 🎯 Project Purpose

This project demonstrates:

-   ASP.NET Core MVC architecture
-   CRUD operations
-   EF Core Code First approach
-   Identity authentication
-   Role-based authorization
