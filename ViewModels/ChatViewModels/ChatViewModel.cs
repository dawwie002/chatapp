namespace QuickChat.MVC.ViewModels.ChatViewModels
{
    public class ChatViewModel
    {
        public Guid ConversationId { get; set; }
        public string UserId { get; set; } = default!;
    }

}
