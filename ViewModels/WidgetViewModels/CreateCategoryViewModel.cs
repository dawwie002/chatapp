using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Pole nie może być puste.")]
        public Guid WidgetId { get; set; }

        [Required(ErrorMessage = "Pole nie może być puste.")]
        [MaxLength(30)]
        [Display(Name = "Nazwa kategorii")]
        public string Name { get; set; } = default!;
    }
}
