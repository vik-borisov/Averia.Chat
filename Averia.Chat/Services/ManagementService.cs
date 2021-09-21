using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace Averia.Chat.Services
{
    /// <summary>
    /// Сервис удаленного управления системой
    /// </summary>
    public class ManagementService : Management.ManagementBase
    {
        private readonly IChatService _chatService;

        public ManagementService(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <inheritdoc />
        public override async Task Messages(EmptyRequest request, IServerStreamWriter<MessageReply> responseStream, ServerCallContext context)
        {
            var recentMessages = await _chatService.GetRecentMessages().ConfigureAwait(false);

            foreach (var message in recentMessages)
            {
                var messageReply = new MessageReply
                {
                    Message = new MessageDto
                    {
                        Text = message.Text,
                        CreateDate = message.CreateDate,
                        UserId = message.UserId
                    }
                };
                
                await responseStream.WriteAsync(messageReply).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override Task<EmptyReply> Stop(EmptyRequest request, ServerCallContext context)
        {
#pragma warning disable 4014
            StopServer();
#pragma warning restore 4014
            return Task.FromResult(new EmptyReply());
        }


        /// <inheritdoc />
        public override Task<UsersReply> Users(EmptyRequest request, ServerCallContext context)
        {
            var onlineUsers = _chatService.UsersOnline();
            var result = new UsersReply();
            result.UserIds.Add(onlineUsers);

            return Task.FromResult(result);
        }

        private async Task StopServer()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            Environment.Exit(1);
        }
    }
}