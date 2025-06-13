using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickChat.MVC.Models
{
    public class Conversation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? AssignedAgentId { get; set; }
        public ApplicationUser? AssignedAgent { get; set; }

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        //zamykanie
        public DateTime? ClosedAt { get; set; }
        public bool IsClosed { get; set; } = false;

        //status wiadomosci lub konwersacji
        public bool IsReadByAgent { get; set; } = false;
        public bool IsReadByClient { get; set; } = false;


        //powiazanie z widget
        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; } = default!;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}