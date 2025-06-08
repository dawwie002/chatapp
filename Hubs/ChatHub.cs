using Microsoft.AspNetCore.SignalR;
using QuickChat.MVC.Data;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Models;

namespace QuickChat.MVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICurrentUserService currentUserService;

        public ChatHub(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            this.dbContext = dbContext;
            this.currentUserService = currentUserService;
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var NowDate = DateTime.Now;

            var date = NowDate.ToShortDateString();
            var time = NowDate.ToShortTimeString();

            string senderId = currentUserService.UserId;

            var messageToAdd = new Message()
            {
                Text = message,
                Date = NowDate,
                SenderId = senderId,
                ReceiverId = receiverId,
            };

            await this.dbContext.AddAsync(messageToAdd);
            await this.dbContext.SaveChangesAsync();

            List<string> users = new List<string>()
            {
                receiverId,senderId
            };

            await Clients.Users(users).SendAsync("ReceiveMessage", message, date, time, senderId);
        }
    }
}
