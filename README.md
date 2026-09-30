# ExpenseTrackerApi

REST API для управления личными доходами и расходами.

## Technologies

* C#
* ASP.NET Core
* .NET 10
* Entity Framework Core
* SQLite
* JWT Authentication
* Swagger / OpenAPI
* LINQ
* Dependency Injection
* Custom Middleware

## Features

* Добавление доходов и расходов
* Получение списка транзакций
* Получение транзакции по ID
* Изменение транзакций
* Удаление транзакций
* Фильтрация по категории и типу
* Фильтрация по диапазону дат
* Расчёт общей суммы доходов и расходов
* Расчёт текущего баланса
* JWT-аутентификация
* Авторизация по роли `Admin`
* Обработка ошибок через custom middleware
* Валидация входных данных
* Entity Framework Core + SQLite

## API Endpoints

### Authentication

`POST /api/Auth/login`

Получение JWT-токена.

### Transactions

`GET /api/Transactions`

Получить список транзакций с возможностью фильтрации.

Поддерживаемые параметры:

* `category`
* `type`
* `from`
* `to`

`GET /api/Transactions/{id}`

Получить транзакцию по ID.

`POST /api/Transactions`

Создать новую транзакцию.

`PUT /api/Transactions/{id}`

Изменить существующую транзакцию.

`DELETE /api/Transactions/{id}`

Удалить транзакцию.

`GET /api/Transactions/summary`

Получить:

* общую сумму доходов;
* общую сумму расходов;
* текущий баланс.

## Validation

API проверяет:

* сумма должна быть больше `0`;
* тип должен быть `Income` или `Expense`;
* категория обязательна;
* описание не может превышать 500 символов;
* сумма одной транзакции не может превышать `1,000,000`.

## Authentication

Для защищённых endpoints используется JWT Bearer Authentication.

Для доступа к `/api/Transactions` необходимо авторизоваться и получить JWT-токен.

Swagger поддерживает кнопку **Authorize** для передачи Bearer-токена.

## Database

Проект использует SQLite.

Connection string:

```text
Data Source=transactions.db
```

Файл базы данных не хранится в Git-репозитории.

## Swagger

После запуска приложения Swagger доступен по адресу:

```text
https://localhost:7153/swagger
```

## Running the project

Клонировать репозиторий:

```bash
git clone https://github.com/MirosXzx/ExpenseTrackerApi.git
```

Перейти в директорию проекта:

```bash
cd ExpenseTrackerApi
```

Восстановить зависимости:

```bash
dotnet restore
```

Применить миграции:

```powershell
Update-Database
```

Запустить приложение:

```bash
dotnet run
```

После запуска открыть Swagger:

```text
https://localhost:7153/swagger
```

## Project Structure

```text
ExpenseTrackerApi
├── Controllers
├── Data
├── DTOs
├── Middleware
├── Migrations
├── Models
├── Services
├── Program.cs
├── appsettings.json
└── ExpenseTrackerApi.csproj
```

## Author

MirosXzx
