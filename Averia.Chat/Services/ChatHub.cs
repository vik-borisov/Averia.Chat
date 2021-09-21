#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Averia.Chat.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Averia.Chat.Services
{
    public class ChatHub : Hub
    {
        private const string UserIdKey = "userId";

        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <inheritdoc />
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];

            if (string.IsNullOrEmpty(userId) || _chatService.IsUserOnline(userId))
            {
                Context.Abort();
                return;
            }

            await base.OnConnectedAsync().ConfigureAwait(false);

            await OnUserJoin(userId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await OnUserLeft().ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        /// <summary>
        /// Отправка сообщения от клиента 
        /// </summary>
        /// <param name="text"></param>
        // ReSharper disable once UnusedMember.Global
        public async Task SendMessage(string text)
        {
            var userId = GetUserId();
            var message = await _chatService.SaveMessage(userId, text).ConfigureAwait(false);

            if (message != null)
            {
                await Clients.All.SendAsync("ReceiveMessages", new[] { message }).ConfigureAwait(false);
            }
        }

        private async Task OnUserJoin(string userId)
        {
            Context.Items[UserIdKey] = userId;

            _chatService.UserJoin(userId);

            await Clients.Others.SendAsync("UsersUpdate", new[] { new UserDto { UserId = userId, Status = EUserStatus.Join } }).ConfigureAwait(false);

            var onlineUsers = _chatService.UsersOnline().Select(u => new UserDto { UserId = u, Status = EUserStatus.Join });
            await Clients.Caller.SendAsync("UsersUpdate", onlineUsers).ConfigureAwait(false);

            var recentMessages = await _chatService.GetRecentMessages().ConfigureAwait(false);
            await Clients.Caller.SendAsync("ReceiveMessages", recentMessages).ConfigureAwait(false);
        }

        private async Task OnUserLeft()
        {
            var userId = GetUserId();

            _chatService.UserLeft(userId);

            await Clients.Others.SendAsync("UsersUpdate", new[] { new UserDto { UserId = userId, Status = EUserStatus.Left } }).ConfigureAwait(false);
        }

        private string GetUserId()
        {
            return Context.Items[UserIdKey]?.ToString() ?? throw new InvalidOperationException();
        }
    }
}