using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class WidgetTeamViewModel
    {
        public int WidgetId { get; set; }

        [Required]
        [Display(Name = "ID użytkownika (GUID)")]
        public string NewUserId { get; set; } = default!;

        [Required]
        [Display(Name = "Rola")]
        public string NewUserRole { get; set; } = default!;

        public List<UserWidgetViewModel> TeamMembers { get; set; } = new();
    }
}
