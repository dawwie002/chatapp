using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.Data;
using QuickChat.MVC.Interface;
using QuickChat.MVC.Models;

namespace QuickChat.MVC.Service
{
    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext context;

        public ConversationService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Guid>> GetWidgetsForUser(string userId)
        {
            return await context.WidgetUsers
                .Where(wu => wu.UserId == userId)
                .Select(wu => wu.WidgetId)
                .ToListAsync();
        }

        public async Task<Conversation> GetOrCreateConversation(Guid widgetId, string? existingId)
        {
            if (Guid.TryParse(existingId, out var guid))
            {
                var existing = await context.Conversations.FirstOrDefaultAsync(c => c.Id == guid && !c.IsClosed);
                if (existing != null)
                    return existing;
            }

            var conversation = new Conversation
            {
                WidgetId = widgetId
            };

            context.Conversations.Add(conversation);
            await context.SaveChangesAsync();

            return conversation;
        }

        public async Task SaveMessage(Guid conversationId, string? senderId, string text)
        {
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Text = text,
                Date = DateTime.UtcNow
            };

            context.Messages.Add(message);

            var conv = await context.Conversations.FindAsync(conversationId);
            if (senderId == null)
                conv!.IsReadByAgent = false;
            else
                conv!.IsReadByClient = false;

            await context.SaveChangesAsync();
        }

        public async Task MarkAsRead(Guid conversationId, string? userId)
        {
            var conversation = await context.Conversations.FindAsync(conversationId);
            if (conversation == null) return;

            if (userId == null)
                conversation.IsReadByAgent = true;
            else
                conversation.IsReadByClient = true;

            await context.SaveChangesAsync();
        }


        public async Task<Conversation?> GetConversationById(Guid id)
        {
            return await context.Conversations.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

    }
}
