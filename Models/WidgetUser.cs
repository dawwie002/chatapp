using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.Models
{
    public class WidgetUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public string Role { get; set; } = default!; // Owner, Agent, Viewer
    }
}
