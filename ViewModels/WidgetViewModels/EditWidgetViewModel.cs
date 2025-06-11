using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class EditWidgetViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nazwa widżetu")]
        public string Name { get; set; } = default!;
    }
}
