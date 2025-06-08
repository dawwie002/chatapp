using QuickChat.MVC.ViewModels.MessageViewModels;

namespace QuickChat.MVC.Interface
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageUsersListViewModel>> GetUsers();
        Task<ChatViewModel> GetMessages(string selectedUserId);
    }
}
