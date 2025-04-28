# Recruitment Agency System
[![C#](https://img.shields.io/badge/C%23-12.0-%23239120?logo=c-sharp)](https://learn.microsoft.com/ru-ru/dotnet/csharp/)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-%235C2D91?logo=.net)
![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![MySQL Version](https://img.shields.io/badge/MySQL-8.0+-orange)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-%237952B3?logo=bootstrap)

[![JavaScript](https://img.shields.io/badge/JavaScript-ES6-%23F7DF1E?logo=javascript)](https://developer.mozilla.org/ru/docs/Web/JavaScript)
[![HTML5](https://img.shields.io/badge/HTML5-%23E34F26?logo=html5)](https://developer.mozilla.org/ru/docs/Web/HTML)
[![CSS3](https://img.shields.io/badge/CSS3-%231572B6?logo=css3)](https://developer.mozilla.org/ru/docs/Web/CSS)

> Веб-платформа для автоматизации подбора персонала, кандидатов и вакансий

# 📋 Системные требования
- **ОС**: Windows 10/11 или Linux (Ubuntu 20.04+)
- **Runtime**: .NET 8.0 SDK
- **СУБД**: MySQL 8.0+
- **Рекомендуемое ПО**:
  - Visual Studio 2022 или VS Code
  - MySQL Workbench (для управления БД)

# 🚀 Первый запуск

## 1. Клонирование репозитория
```bash
git clone https://github.com/Antip-An/RecruitmentAgency.Web.git
cd RecruitmentAgency.Web
```

## 2. Настройка подключения к БД 

Отредактируйте appsettings.json
```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=RecruitmentAgency;User=root;Password=ваш_пароль;"
}
```

Создайте БД в MySQL
```bash
CREATE DATABASE RecruitmentAgency;
```

## 3. Создание и применение миграций
```bash
dotnet build
dotnet ef migrations add Initial
dotnet ef database update
```

## 4. Запуск приложенния
```bash
dotnet run
```

Приложение доступно по адресу: http://localhost:5135

# 🔧 Тестовые данные

После первого запуска система автоматически создает:
- Тестового администратора:
  Логин: admin@example.com
  Пароль: Admin123!
- Пример компании "ТехноКорп"
- Тестовые вакансии

# ⚠️ Устранение проблем
Если миграции не применяются:
```bash
# Удалить последнюю миграцию
dotnet ef migrations remove

# Удалить папку с файлами миграции (при необходимости)
rm -r Migrations

# Создать заново
dotnet ef migrations add Initial
dotnet ef database update
```
