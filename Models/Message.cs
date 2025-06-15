using System.ComponentModel.DataAnnotations.Schema;

namespace QuickChat.MVC.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = default!;
        public DateTime Date { get; set; }

        public string? SenderId { get; set; }

        [ForeignKey (nameof(SenderId))]
        public ApplicationUser? Sender { get; set; }

        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = default!;
    }
}
