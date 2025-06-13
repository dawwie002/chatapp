namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class UserWidgetViewModel
    {
        public Guid WidgetId { get; set; }
        public string WidgetName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
