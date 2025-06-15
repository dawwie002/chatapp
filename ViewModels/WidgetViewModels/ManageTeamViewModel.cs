using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class ManageTeamViewModel
    {
        public Guid WidgetId { get; set; }

        [Required(ErrorMessage = "Pole nie może być puste.")]
        [Display(Name = "Identyfikator użytkownika [Guid]")]
        public string NewUserId { get; set; } = default!;

        [Required]
        [Display(Name = "Rola")]
        public string NewUserRole { get; set; } = default!;

        public List<UserWidgetViewModel> TeamMembers { get; set; } = new();
        public IEnumerable<SelectListItem> RolesSelectList { get; set; } = new List<SelectListItem>();

    }
}
