#Введение
Решение состоит из 2 проектов:
* Averia.Chat - Серверная сторона + клиентское приложение
* Averia.Chat.Cli - Консольное приложение дял управления сервисом

#Averia.Chat
Для запуска требуется указать переменную среды окружения:
* "ASPNETCORE_ENVIRONMENT": "Development"

##Порты
* 5000 - http
* 5001 - grcp

## Развитие
Как я вижу дальнейшее развитие  проекта:
* горизонтальное масштабирование (можно добавить поддержку Redis Streams для обмена сообщениями и пользователями)
* Поддержка докера и виртуализации
* прикрутить аутентификацию
* добавить приватные сообщения

#Averia.Chat.Cli
по умолчанию утилита смотрит на http://localhost:5001. Для именения адреса нужно добавить к вызову параметр ```-a <урл сервера>```

* ./Averia.Chat.Cli watch - список сообщений через стриминг
* ./Averia.Chat.Cli ls - список пользователей
* ./Averia.Chat.Cli stop - остановка сервера

##Что вызвало вопросы:
```‣ /watch - выводит последние 20 сообщений чата в реальном времени```

Можно трактовать по разному:
* вывести через стриминг 20 последнйи сообщений (я так реализовал)
* подключиться к серверу, получиьт 20 сообщений и все последующие, пока не прервать выполнения утилиты (в таком случае нужно реализовывать паттерн Observer между сервисами ChatHub и ManagementService)
