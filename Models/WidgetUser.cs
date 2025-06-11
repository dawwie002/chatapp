namespace QuickChat.MVC.Models
{
    public class WidgetUser
    {
        public int Id { get; set; }

        public int WidgetId { get; set; }
        public Widget Widget { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public string Role { get; set; } = default!; // Owner, Agent, Viewer
    }
}
