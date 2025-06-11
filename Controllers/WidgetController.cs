using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuickChat.MVC.Constants;
using QuickChat.MVC.Interface;
using QuickChat.MVC.Service;
using QuickChat.MVC.ViewModels.WidgetViewModels;

namespace QuickChat.MVC.Controllers
{
    public class WidgetController : Controller
    {
        private readonly IWidgetService widgetService;

        public WidgetController(IWidgetService widgetService)
        {
            this.widgetService = widgetService;
        }

        public async Task<IActionResult> Index()
        {
            var widgets = await widgetService.GetWidgetsWithRolesForCurrentUser(); // lub GetWidgetsForCurrentUser()
            return View(widgets);
        }

            [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateWidgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await this.widgetService.Create(model);

            //TODO
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var widget = await widgetService.GetWidgetForEdit(id);
            if (widget == null)
                return NotFound();

            return View(widget);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditWidgetViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await widgetService.UpdateWidget(model);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageTeam(int id)
        {
            var vm = await widgetService.GetTeamManagementData(id);
            if (vm == null)
                return NotFound();

            ViewBag.Roles = new SelectList(new[] { RoleNames.Owner, RoleNames.Agent});
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageTeam(WidgetTeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { RoleNames.Owner, RoleNames.Agent});
                model.TeamMembers = (await widgetService.GetTeamManagementData(model.WidgetId))?.TeamMembers ?? new();
                return View(model);
            }

            var success = await widgetService.AddUserToWidget(model.WidgetId, model.NewUserId, model.NewUserRole);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Nie udało się dodać użytkownika (może już istnieje lub nieprawidłowy GUID).");
            }

            return RedirectToAction(nameof(ManageTeam), new { id = model.WidgetId });
        }

    }
}
