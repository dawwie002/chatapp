using Microsoft.AspNetCore.SignalR;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Interface;

namespace QuickChat.MVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConversationService conversationService;
        private readonly ICurrentUserService currentUserService;

        public ChatHub(IConversationService conversationService, ICurrentUserService currentUserService)
        {
            this.conversationService = conversationService;
            this.currentUserService = currentUserService;
        }

        public async Task SendMessage(string conversationId, string user, string message)
        {
            var senderId = currentUserService.UserId;

            await conversationService.SaveMessage(Guid.Parse(conversationId), senderId, message);

            await Clients.Group(conversationId)
                .SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task MarkAsRead(string conversationId, string? userId)
        {
            await conversationService.MarkAsRead(Guid.Parse(conversationId), userId);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = currentUserService.UserId;

            var widgetIds = await conversationService.GetWidgetsForUser(userId); // agent ma dostęp do widżetów
            foreach (var widgetId in widgetIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"widget-{widgetId}");
            }

            await base.OnConnectedAsync();
        }

        public async Task NotifyConversationClosed(string conversationId)
        {
            await Clients.Group(conversationId).SendAsync("ConversationClosed", conversationId);
        }

    }
}
