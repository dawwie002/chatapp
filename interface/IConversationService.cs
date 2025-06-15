using QuickChat.MVC.Models;

namespace QuickChat.MVC.Interface
{
    public interface IConversationService
    {
        Task<Conversation> GetOrCreateConversation(Guid widgetId, string? existingId);
        Task SaveMessage(Guid conversationId, string? senderId, string text);
        Task MarkAsRead(Guid conversationId, string? userId);
        Task<List<Guid>> GetWidgetsForUser(string userId);
        Task<Conversation?> GetConversationById(Guid id);
        Task SaveChangesAsync();

    }
}
