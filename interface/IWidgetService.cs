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
        Task<EditWidgetViewModel?> GetWidgetForEdit(int widgetId);
        Task<bool> UpdateWidget(EditWidgetViewModel model);
        Task<WidgetTeamViewModel?> GetTeamManagementData(int widgetId);
        Task<bool> AddUserToWidget(int widgetId, string userId, string role);
    }
}
