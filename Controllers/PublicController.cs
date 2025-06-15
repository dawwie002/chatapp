using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.Data;
using QuickChat.MVC.ViewModels.WidgetViewModels;

namespace QuickChat.MVC.Controllers
{
    public class PublicController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public PublicController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> SelectWidget()
        {
            var widgets = await dbContext.Widgets
                .Select(w => new WidgetListItemViewModel
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .ToListAsync();

            return View(widgets);
        }

    }
}