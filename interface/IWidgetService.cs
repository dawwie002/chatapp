using Microsoft.AspNetCore.Mvc;
using QuickChat.MVC.Helpers;
using QuickChat.MVC.Models;
using QuickChat.MVC.Service;
using QuickChat.MVC.ViewModels.WidgetViewModels;

namespace QuickChat.MVC.Interface
{
    public interface IWidgetService
    {
        Task Create(CreateWidgetViewModel widgetViewModel);
        Task<List<UserWidgetViewModel>> GetWidgetsWithRolesForCurrentUser();
        Task<EditWidgetViewModel?> GetWidgetForEdit(Guid widgetId);
        Task<bool> UpdateWidget(EditWidgetViewModel model);
        Task<ManageTeamViewModel?> GetTeamManagementData(Guid widgetId);
        Task<bool> AddUserToWidget(Guid widgetId, string userId, string role);



        Task<bool> DeleteWidget(Guid widgetId);


        Task<bool> RemoveUserFromWidget(Guid widgetId, string userId);
        Task<bool> ChangeUserRole(Guid widgetId, string userId, string newRole);


        Task<bool> AddCategoryToWidget(Guid widgetId, string categoryName);
        Task<List<Category>> GetCategoriesForWidget(Guid widgetId);
    }
}
