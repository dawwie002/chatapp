using QuickChat.MVC.Data;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Interface;
using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.ViewModels.MessageViewModels;

namespace QuickChat.MVC.Service
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public MessageService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }
        public async Task<ChatViewModel> GetMessages(string selectedUserId)
        {
            var currentUserId = this.currentUserService.UserId;

            var selectedUser = await this.context.Users.FirstOrDefaultAsync(i => i.Id == selectedUserId);
            var selectedUserName = "";
            if (selectedUser != null)
            {
                selectedUserName = selectedUser.UserName;
            }

            var chatViewModel = new ChatViewModel()
            {
                CurrentUserId = currentUserId,
                ReceiverId = selectedUserId,
                ReceiverUserName = selectedUserName,
            };

            var messages = await this.context.Messages.Where(i =>
                (i.SenderId == currentUserId || i.SenderId == selectedUserId) &&
                (i.ReceiverId == currentUserId || i.ReceiverId == selectedUserId))
                .Select(i => new UserMessagesListViewModel()
                {
                    Id = i.Id,
                    Text = i.Text,
                    Date = i.Date.ToShortDateString(),
                    Time = i.Date.ToShortTimeString(),
                    IsCurrentUserSentMessage = i.SenderId == currentUserId,
                })
                .ToListAsync();

            chatViewModel.Messages = messages;
            return chatViewModel;
        }


        public async Task<IEnumerable<MessageUsersListViewModel>> GetUsers()
        {
            var currentUserId = this.currentUserService.UserId;

            var users = await this.context.Users
                .Where(i => i.Id != currentUserId)
                .Select(i => new MessageUsersListViewModel()
                {
                    Id = i.Id,
                    UserName = i.UserName,
                    LastMessage = this.context.Messages
                    .Where(m => (m.SenderId == currentUserId || m.SenderId == i.Id) && 
                                (m.ReceiverId == currentUserId || m.ReceiverId == i.Id))
                    .OrderByDescending(m => m.Id)
                    .Select(m => m.Text)
                    .FirstOrDefault()
                })
                .ToListAsync();
            return users;
        }

    }
}
