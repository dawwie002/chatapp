using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.Data;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Hubs;

namespace QuickChat.MVC.Controllers
{
    [Authorize]
    public class AgentController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICurrentUserService currentUserService;

        public AgentController(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            this.dbContext = dbContext;
            this.currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> CloseConversation(Guid id)
        {
            var conversation = await dbContext.Conversations.FindAsync(id);
            if (conversation == null)
                return NotFound();

            conversation.IsClosed = true;
            conversation.ClosedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            // 🔔 Powiadom klienta (drugą stronę)
            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ChatHub>>();
            await hubContext.Clients.Group(id.ToString()).SendAsync("ConversationClosed", id.ToString());

            return RedirectToAction("ChatPanel");
        }


        [HttpPost]
        public async Task<IActionResult> AssignCategoryInline(Guid ConversationId, Guid CategoryId)
        {
            var conversation = await dbContext.Conversations.FindAsync(ConversationId);
            if (conversation == null) return NotFound();

            conversation.CategoryId = CategoryId;
            await dbContext.SaveChangesAsync();

            return RedirectToAction("ChatPanel", new { id = ConversationId });
        }


        public async Task<IActionResult> ChatPanel()
        {
            var userId = currentUserService.UserId;

            var widgetIds = await dbContext.WidgetUsers
                .Where(wu => wu.UserId == userId)
                .Select(wu => wu.WidgetId)
                .ToListAsync();

            var conversations = await dbContext.Conversations
                .Where(c => widgetIds.Contains(c.WidgetId) && !c.IsClosed)
                .Include(c => c.Widget)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View("ChatPanel", conversations);
        }

        public async Task<IActionResult> GetConversationPartial(Guid id)
        {
            var conversation = await dbContext.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Widget)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conversation == null)
                return NotFound();

            var categories = await dbContext.Categories
                .Where(c => c.WidgetId == conversation.WidgetId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewBag.Categories = categories;

            return PartialView("_ChatPartial", conversation);
        }

        public async Task<IActionResult> ChatPanelSidebarPartial()
        {
            var userId = currentUserService.UserId;

            var widgetIds = await dbContext.WidgetUsers
                .Where(w => w.UserId == userId)
                .Select(w => w.WidgetId)
                .ToListAsync();

            var conversations = await dbContext.Conversations
               .Where(c => widgetIds.Contains(c.WidgetId) && !c.IsClosed)
                .Include(c => c.Widget)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return PartialView("_ChatPanelSidebarPartial", conversations);
        }

    }
}
