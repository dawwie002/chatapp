using QuickChat.MVC.Models;

namespace QuickChat.MVC.Helpers
{
    public interface ICurrentUserService
    {
        string UserId { get; }

        Task<ApplicationUser> GetUser();
    }
}
