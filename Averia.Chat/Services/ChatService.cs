using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Averia.Chat.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Averia.Chat.Services
{
    /// <summary>
    /// Сервис управления сообщениями и пользователями
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Получить последние сообщения
        /// </summary>
        /// <returns>Список N последних сообщений</returns>
        Task<IReadOnlyList<Message>> GetRecentMessages();

        /// <summary>
        /// Сохраняет сообщение в БД
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="text">Текст сообщения</param>
        /// <returns>Сохраненное сообщение</returns>
        Task<Message> SaveMessage(string userId, string text);

        /// <summary>
        /// Возвращает список пользователей в сети
        /// </summary>
        /// <returns>Список идентификаторов пользователей</returns>
        public IReadOnlyList<string> UsersOnline();

        /// <summary>
        /// Проверяет в сети ли пользователь
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>В сети или нет</returns>
        public bool IsUserOnline(string userId);

        /// <summary>
        /// Обрабатывает подключение пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        public void UserJoin(string userId);

        /// <summary>
        /// Обрабатывает отключение пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        public void UserLeft(string userId);
    }

    public class ChatService : IChatService
    {
        private readonly IConnectionMultiplexer _redisService;
        private readonly ConcurrentDictionary<string, bool> _onlineUsers = new();

        private readonly int _messageLimit;

        public ChatService(
            IConnectionMultiplexer redisService,
            IConfiguration configuration
        )
        {
            _redisService = redisService;

            _messageLimit = int.Parse(configuration["MessageLimit"]);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Message>> GetRecentMessages()
        {
            var db = _redisService.GetDatabase();
            var value = await db.SortedSetRangeByRankAsync("messages", stop: _messageLimit - 1, order: Order.Descending).ConfigureAwait(false);

            return value.Select(v => JsonConvert.DeserializeObject<Message>(v.ToString())).Reverse().ToArray();
        }

        /// <inheritdoc />
        public IReadOnlyList<string> UsersOnline()
        {
            return _onlineUsers.Keys.ToArray();
        }

        /// <inheritdoc />
        public bool IsUserOnline(string userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }

        /// <inheritdoc />
        public void UserJoin(string userId)
        {
            _onlineUsers[userId] = true;
        }

        /// <inheritdoc />
        public void UserLeft(string userId)
        {
            _onlineUsers.TryRemove(userId, out _);
        }

        /// <inheritdoc />
        public async Task<Message> SaveMessage(string userId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var db = _redisService.GetDatabase();

            var message = new Message
            {
                CreateDate = GetNow(),
                Text = text,
                UserId = userId
            };

            await db.SortedSetAddAsync($"messages", JsonConvert.SerializeObject(message), message.CreateDate).ConfigureAwait(false);

            return message;
        }

        private static long GetNow()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}