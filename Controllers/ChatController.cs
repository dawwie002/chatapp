using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuickChat.MVC.Hubs;
using QuickChat.MVC.Interface;

namespace QuickChat.MVC.Controllers
{
    public class ChatController : Controller
    {
        private readonly IConversationService conversationService;
        private readonly IHubContext<ChatHub> hubContext;

        public ChatController(IConversationService conversationService, IHubContext<ChatHub> hubContext)
        {
            this.conversationService = conversationService;
            this.hubContext = hubContext;
        }

        public async Task<IActionResult> Index(Guid widgetId)
        {
            var conversationIdCookie = Request.Cookies["ConversationId"];
            var conversation = await conversationService.GetOrCreateConversation(widgetId, conversationIdCookie);

            if (conversationIdCookie != conversation.Id.ToString())
            {
                Response.Cookies.Append("ConversationId", conversation.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(2),
                    HttpOnly = true,
                    IsEssential = true
                });

                // 🔔 Tu jest wysyłka SignalR do agenta
                await hubContext.Clients.Group($"widget-{widgetId}")
                    .SendAsync("NewConversation", widgetId);
            }

            ViewBag.ConversationId = conversation.Id;
            ViewBag.WidgetId = widgetId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Close(Guid id)
        {
            var conversation = await conversationService.GetConversationById(id);
            if (conversation == null || conversation.IsClosed)
                return NotFound();

            conversation.IsClosed = true;
            conversation.ClosedAt = DateTime.UtcNow;
            await conversationService.SaveChangesAsync();

            // Powiadom klienta (drugą stronę)
            await hubContext.Clients.Group(id.ToString()).SendAsync("ConversationClosed", id.ToString());

            return RedirectToAction("Index", new { widgetId = conversation.WidgetId });
        }

    }
}
