using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.Constants;
using QuickChat.MVC.Data;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Interface;
using QuickChat.MVC.Models;
using QuickChat.MVC.ViewModels.WidgetViewModels;


namespace QuickChat.MVC.Service
{
    public class WidgetService : IWidgetService
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public WidgetService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task Create(CreateWidgetViewModel widgetViewModel)
        {
            var userId = currentUserService.UserId;

            var widget = new Widget
            {
                Name = widgetViewModel.Name
            };

            // Dodaj widget do kontekstu
            context.Widgets.Add(widget);
            await context.SaveChangesAsync(); // potrzebne, by widget.Id został wygenerowany

            // powiązanie WidgetUser z rolą Owner
            var widgetUser = new WidgetUser
            {
                WidgetId = widget.Id,
                UserId = userId,
                Role = RoleNames.Owner
            };

            context.WidgetUsers.Add(widgetUser);
            await context.SaveChangesAsync();
        }

        public async Task<List<UserWidgetViewModel>> GetWidgetsWithRolesForCurrentUser()
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                return new List<UserWidgetViewModel>();

            var widgets = await context.WidgetUsers
                .Where(wu => wu.UserId == userId)
                .Select(wu => new UserWidgetViewModel
                {
                    WidgetId = wu.WidgetId,
                    WidgetName = wu.Widget.Name,
                    Role = wu.Role
                })
                .ToListAsync();

            return widgets;
        }

        public async Task<EditWidgetViewModel?> GetWidgetForEdit(Guid widgetId)
        {
            var widget = await context.Widgets.FindAsync(widgetId);

            if (widget == null)
                return null;

            return new EditWidgetViewModel
            {
                Id = widget.Id,
                Name = widget.Name
            };
        }

        public async Task<bool> UpdateWidget(EditWidgetViewModel model)
        {
            var widget = await context.Widgets.FindAsync(model.Id);

            if (widget == null)
                return false;

            widget.Name = model.Name;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<ManageTeamViewModel?> GetTeamManagementData(Guid widgetId)
        {
            var widget = await context.Widgets
                    .Include(w => w.WidgetUsers)
                    .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync(w => w.Id == widgetId);

            if (widget == null)
                return null;

            return new ManageTeamViewModel
            {
                WidgetId = widget.Id,
                TeamMembers = widget.WidgetUsers.Select(wu => new UserWidgetViewModel
                {
                    UserId = wu.UserId,
                    Email = wu.User.Email,
                    Role = wu.Role
                }).ToList()
            };
        }

        public async Task<bool> AddUserToWidget(Guid widgetId, string userId, string role)
        {
            var alreadyExists = await context.WidgetUsers.AnyAsync(wu => wu.WidgetId == widgetId && wu.UserId == userId);
            if (alreadyExists)
                return false;

            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return false;

            var widgetUser = new WidgetUser
            {
                WidgetId = widgetId,
                UserId = userId,
                Role = role
            };

            context.WidgetUsers.Add(widgetUser);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWidget(Guid widgetId)
        {
            var userId = currentUserService.UserId;

            var widgetUser = await context.WidgetUsers
                .FirstOrDefaultAsync(wu => wu.WidgetId == widgetId && wu.UserId == userId);

            if (widgetUser?.Role != RoleNames.Owner)
            {
                return false;
            }

            var widget = await this.context.Widgets
                .Include(w => w.WidgetUsers)
                .Include(w => w.Conversations)
                .FirstOrDefaultAsync(w => w.Id == widgetId);

            if (widget == null)
                return false;

            // ewentualnie usuń powiązania:
            this.context.WidgetUsers.RemoveRange(widget.WidgetUsers);
            this.context.Conversations.RemoveRange(widget.Conversations);
            this.context.Widgets.Remove(widget);

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUserFromWidget(Guid widgetId, string userId)
        {
            var widgetUser = await context.WidgetUsers
                .FirstOrDefaultAsync(wu => wu.WidgetId == widgetId && wu.UserId == userId);

            if (widgetUser == null || widgetUser.Role == RoleNames.Owner)
                return false;

            context.WidgetUsers.Remove(widgetUser);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeUserRole(Guid widgetId, string userId, string newRole)
        {
            if (!new[] { RoleNames.Agent, RoleNames.Admin }.Contains(newRole))
                return false;

            var widgetUser = await context.WidgetUsers
                .FirstOrDefaultAsync(wu => wu.WidgetId == widgetId && wu.UserId == userId);

            if (widgetUser == null || widgetUser.Role == RoleNames.Owner)
                return false;

            widgetUser.Role = newRole;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddCategoryToWidget(Guid widgetId, string categoryName)
        {
            var widget = await context.Widgets.FindAsync(widgetId);
            if (widget == null) return false;

            var category = new Category
            {
                Name = categoryName,
                WidgetId = widgetId
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Category>> GetCategoriesForWidget(Guid widgetId)
        {
            return await context.Categories
                .Where(c => c.WidgetId == widgetId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        public async Task<bool> DeleteCategory(Guid widgetId, string categoryName)
        {
            var widget = await context.Widgets
                .Include(w => w.Categories)
                .FirstOrDefaultAsync(w => w.Id == widgetId);

            if (widget == null)
                return false;

            var category = widget.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
                return false;

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
