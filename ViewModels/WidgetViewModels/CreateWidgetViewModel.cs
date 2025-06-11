using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class CreateWidgetViewModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Name { get; set; } = default!;
    }
}
