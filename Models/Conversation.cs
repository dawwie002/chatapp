using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickChat.MVC.Models
{
    public class Conversation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        //public string? ClientIdentifier { get; set; }
        public string? AssignedAgentId { get; set; }
        public ApplicationUser? AssignedAgent { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }


        //zamykanie
        public DateTime? ClosedAt { get; set; }
        public bool IsClosed { get; set; } = false;
        public string? ClosedById { get; set; }
        [ForeignKey(nameof(ClosedById))]
        public ApplicationUser? ClosedBy { get; set; }


        //powiazanie z widget
        public int WidgetId { get; set; }
        public Widget Widget { get; set; } = default!;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}