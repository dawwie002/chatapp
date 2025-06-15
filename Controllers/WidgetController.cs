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
            var widgets = await widgetService.GetWidgetsWithRolesForCurrentUser();
            ViewBag.CurrentUserId = currentUserService.UserId;
            return View(widgets);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditPanel(Guid id)
        {
            if (!await HasEditPermission(id))
            {
                TempData["Error"] = "Brak uprawnień.";
                return RedirectToAction("Index");
            }

            var teamModel = await widgetService.GetTeamManagementData(id);
            teamModel.RolesSelectList = new SelectList(new[] { RoleNames.Admin, RoleNames.Agent });

            var categories = await widgetService.GetCategoriesForWidget(id);
            var categoryModel = new CategoryViewModel
            {
                WidgetId = id,
                ExistingCategories = categories.Select(c => c.Name).ToList() // 🔁 tu konwersja
            };


            var vm = new WidgetEditPanelViewModel
            {
                WidgetId = id,
                EditModel = await widgetService.GetWidgetForEdit(id),
                TeamModel = teamModel,
                CategoryModel = categoryModel // ✅ TO jest kluczowe
            };

            return View("EditPanel", vm);
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

            return RedirectToAction(nameof(EditPanel), new { id = model.Id });
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

            return RedirectToAction(nameof(EditPanel), new { id = model.WidgetId });
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
            return RedirectToAction(nameof(EditPanel), new { id = widgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(Guid widgetId, string userId, string newRole)
        {
            var success = await widgetService.ChangeUserRole(widgetId, userId, newRole);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(EditPanel), new { id = widgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
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

            return RedirectToAction(nameof(EditPanel), new { id = model.WidgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid widgetId, string categoryName)
        {
            if (!await HasEditPermission(widgetId))
            {
                TempData["Error"] = "Brak uprawnień.";
                return RedirectToAction(nameof(Index));
            }

            var success = await widgetService.DeleteCategory(widgetId, categoryName);
            if (!success)
            {
                TempData["Error"] = "Nie udało się usunąć kategorii.";
            }

            return RedirectToAction(nameof(EditPanel), new { id = widgetId });
        }

    }
}