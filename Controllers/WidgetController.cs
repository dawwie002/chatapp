using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuickChat.MVC.Constants;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Interface;
using QuickChat.MVC.Service;
using QuickChat.MVC.ViewModels.WidgetViewModels;

namespace QuickChat.MVC.Controllers
{
    [Authorize]
    public class WidgetController : Controller
    {
        private readonly IWidgetService widgetService;
        private readonly ICurrentUserService currentUserService;

        public WidgetController(IWidgetService widgetService, ICurrentUserService currentUserService)
        {
            this.widgetService = widgetService;
            this.currentUserService = currentUserService;
        }

        private async Task<bool> HasEditPermission(Guid widgetId)
        {
            var role = await currentUserService.GetRoleInWidget(widgetId);
            return role == RoleNames.Admin || role == RoleNames.Owner;
        }

        private async Task<bool> HasOwnerPermission(Guid widgetId)
        {
            var role = await currentUserService.GetRoleInWidget(widgetId);
            return role == RoleNames.Owner;
        }


        public async Task<IActionResult> Index()
        {
            var widgets = await widgetService.GetWidgetsWithRolesForCurrentUser(); // lub GetWidgetsForCurrentUser()
            return View(widgets);
        }

        public IActionResult Create()
        {
            return View();
        }

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

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await HasEditPermission(id))
            {
                TempData["Error"] = "Nie masz uprawnień do edytowania tego widżetu.";
                return RedirectToAction("Index");
            }


            var widget = await widgetService.GetWidgetForEdit(id);
            if (widget == null)
                return NotFound();

            return View(widget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditWidgetViewModel model)
        {
            if (!await HasEditPermission(model.Id))
            {
                TempData["Error"] = "Nie masz uprawnień do edytowania tego widżetu.";
                return RedirectToAction("Index");
            }


            var success = await widgetService.UpdateWidget(model);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageTeam(Guid id)
        {
            if (!await HasEditPermission(id))
            {
                TempData["Error"] = "Nie masz uprawnień do edytowania tego widżetu.";
                return RedirectToAction("Index");
            }
            var vm = await widgetService.GetTeamManagementData(id);
            if (vm == null)
                return NotFound();

            ViewBag.Roles = new SelectList(new[] { RoleNames.Admin, RoleNames.Agent});
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageTeam(ManageTeamViewModel model)
        {
            if (!await HasEditPermission(model.WidgetId))
            {
                TempData["Error"] = "Nie masz uprawnień do edytowania tego widżetu.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { RoleNames.Admin, RoleNames.Agent});
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await HasOwnerPermission(id))
            {
                TempData["Error"] = "Tylko właściciel widgetu może go usunąć.";
                return RedirectToAction("Index");
            }

            var success = await widgetService.DeleteWidget(id);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(Guid widgetId, string userId)
        {
            var success = await widgetService.RemoveUserFromWidget(widgetId, userId);
            if (!success)
            {
                TempData["Error"] = "Nie udało się usunąć użytkownika z zespołu.";
            }
            return RedirectToAction(nameof(ManageTeam), new { id = widgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(Guid widgetId, string userId, string newRole)
        {
            var success = await widgetService.ChangeUserRole(widgetId, userId, newRole);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(ManageTeam), new { id = widgetId });
        }

        [HttpGet]
        public async Task<IActionResult> AddCategory(Guid widgetId)
        {
            if (!await HasEditPermission(widgetId))
            {
                TempData["Error"] = "Brak uprawnień.";
                return RedirectToAction("Index");
            }

            var vm = new CreateCategoryViewModel
            {
                WidgetId = widgetId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CreateCategoryViewModel model)
        {
            if (!await HasEditPermission(model.WidgetId))
            {
                TempData["Error"] = "Brak uprawnień.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return View(model);

            var success = await widgetService.AddCategoryToWidget(model.WidgetId, model.Name);
            if (!success)
            {
                ModelState.AddModelError("", "Nie udało się dodać kategorii.");
                return View(model);
            }

            return RedirectToAction("Edit", new { id = model.WidgetId });
        }

    }
}