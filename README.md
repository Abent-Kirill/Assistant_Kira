# Assistant_Kira 🤖

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Telegram Bot](https://img.shields.io/badge/Telegram-Bot-26A5E4.svg)](https://core.telegram.org/bots)

Личный Telegram-ассистент, который агрегирует повседневные задачи в одном месте:
погода, новости, вакансии, курсы валют и управление Google Календарём.
Построен на ASP.NET Core с webhook-моделью (без polling).

## Содержание

- [Возможности](#возможности)
- [Стек технологий](#стек-технологий)
- [Архитектура](#архитектура)
- [Требования](#требования)
- [Конфигурация](#конфигурация)
- [Запуск](#запуск)
- [Команды бота](#команды-бота)
- [Структура проекта](#структура-проекта)
- [Roadmap](#roadmap)
- [Лицензия](#лицензия)

## Возможности

- 📅 **Google Календарь** — создание событий на естественном языке («завтра 10:00 …»), поиск и список ближайших событий
- 📰 **Новости** — заголовки и поиск по ключевому слову с постраничным перелистыванием
- 💼 **Вакансии** — лента Habr Career (C#, удалёнка) с перелистыванием
- 🌤 **Погода** — по запросу и автоматически каждое утро (Самара)
- 💱 **Валюты** — курсы интересующих валют и конвертация (`100 USD EUR`)
- 📎 **Файлы** — приём фото и документов с сохранением на сервер

## Стек технологий

| Категория | Технология |
|---|---|
| Платформа | .NET 8, ASP.NET Core |
| Telegram | [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) (webhook) |
| Диспетчеризация | MediatR |
| Логирование | Serilog (Console + rolling File) |
| Календарь | Google.Apis.Calendar.v3 |
| Дата/время | NodaTime |
| Внешние API | OpenWeatherMap, NewsAPI, Apilayer (Fixer), Habr Career RSS |

## Архитектура

```
Telegram → POST /api/telegram/update (TelegramController)
              ↓ разбор типа сообщения + regex
         MediatR.Send(Request)
              ↓
         Handler → внешний API → форматирование → ответ в Telegram
```

- **MediatR** — каждое намерение пользователя это `IRequest` в `Requests/` и обработчик в `Handlers/`
- **Named HttpClients** — каждый внешний API настроен в `Program.cs`, инжектится по имени
- **In-memory пагинация** — `NewsRepository` / `VacancyRepository` хранят позицию просмотра
- **Single-user** — авторизован только `ChatId` из конфигурации
- **Background service** — `GoodMorningService` шлёт утреннюю сводку в 10:00

## Требования

- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- Telegram-бот и токен от [@BotFather](https://t.me/BotFather)
- Публичный HTTPS-адрес для webhook (домен или туннель вроде [ngrok](https://ngrok.com/))
- API-ключи: [OpenWeatherMap](https://openweathermap.org/api), [NewsAPI](https://newsapi.org/), [Apilayer Fixer](https://apilayer.com/marketplace/fixer-api)
- Google Cloud service account с доступом к Calendar API (JSON-ключ)

## Конфигурация

Секреты задаются через `appsettings.json` (не коммитится) или [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

```bash
cd Assistant_Kira
dotnet user-secrets set "BotSettings:Token" "<telegram-bot-token>"
```

Полный набор параметров:

```json
{
  "BotSettings": {
    "Token": "<токен от BotFather>",
    "WebhookUrl": "https://your-domain.example",
    "ChatId": "<id единственного разрешённого чата>"
  },
  "ServicesApiKeys": {
    "Weather": "<ключ OpenWeatherMap>",
    "ApilayerCurrency": "<ключ Apilayer Fixer>",
    "NewsApi": "<ключ NewsAPI>"
  },
  "GoogleCalendar": {
    "Aunth": "<путь к JSON-ключу service account>",
    "Name": "<id календаря>"
  },
  "Paths": {
    "Files": "<папка для документов>",
    "Photos": "<папка для фото>"
  }
}
```

| Секция | Ключ | Назначение |
|---|---|---|
| `BotSettings` | `Token` / `WebhookUrl` / `ChatId` | Токен бота, базовый HTTPS-URL для webhook, разрешённый чат |
| `ServicesApiKeys` | `Weather` / `ApilayerCurrency` / `NewsApi` | Ключи внешних API |
| `GoogleCalendar` | `Aunth` / `Name` | Путь к JSON service account и id календаря |
| `Paths` | `Files` / `Photos` | Локальные папки для загрузок |

## Запуск

### Локально

```bash
dotnet build
dotnet run --project Assistant_Kira/Assistant_Kira.csproj
```

Бот регистрирует webhook на старте и удаляет его при остановке.
Swagger доступен в Development-среде на `/swagger`.

### Docker

За Kestrel (порт 5000) используется nginx как reverse proxy. Бот должен быть
доступен по HTTPS, иначе Telegram не сможет доставлять webhook-обновления.

```bash
dotnet publish -c Release -o ./bin/Release/net10.0/publish
docker build -t assistant-kira .
docker run -d -p 80:80 assistant-kira
```

> ⚠️ Разворачивание через `Docker/Podman` в разработке.

## Команды бота

| Команда | Действие |
|---|---|
| `Погода` | Погода в Самаре |
| `Курс` | Курсы валют (USD, EUR, RUB) |
| `Новости [запрос]` | Заголовки или поиск новостей (перелистывание кнопками) |
| `Вакансии` | Лента вакансий Habr Career |
| `100 USD EUR` | Конвертация валют |
| `<текст> завтра 10:00` | Создать событие в календаре |
| `календарь ближайшие <N>` | События на ближайшие N дней |
| `календарь найди <текст>` | Поиск события по тексту |

Также бот принимает фото и документы и сохраняет их в папки из `Paths`.

## Структура проекта

```
Assistant_Kira/
├── Controllers/    # Webhook-эндпоинты
├── Requests/       # MediatR-запросы (намерения)
├── Handlers/       # Обработчики запросов
├── Services/       # Внешние интеграции и фоновые сервисы
├── Repositories/   # In-memory пагинация
├── Models/ DTO/    # Доменные модели и контракты API
├── Options/        # Классы конфигурации (IOptions)
└── Program.cs      # Регистрация DI и pipeline
```

## Roadmap

- [ ] Хранение заметок
- [ ] Анализ физических активностей и финансов
- [x] Сводка дня
- [ ] Web/mobile-клиент (сейчас единственный клиент — Telegram)

## Лицензия

[MIT](LICENSE.txt) © 2024 Abent Kirill Alexeevich
