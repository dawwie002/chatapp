using Microsoft.AspNetCore.Mvc;
using QuickChat.MVC.Interface;
using System.Threading.Tasks;

namespace QuickChat.MVC.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await messageService.GetUsers();
            return View(users);
        }
        public async Task<IActionResult> Chat(string selectedUserId)
        {
            var chatViewModel = await this.messageService.GetMessages(selectedUserId);
            return View(chatViewModel);
        }
    }
}
