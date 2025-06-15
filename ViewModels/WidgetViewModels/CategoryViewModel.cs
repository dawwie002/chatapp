using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class CategoryViewModel
    {
        [Required(ErrorMessage = "Pole nie może być puste.")]
        public Guid WidgetId { get; set; }

        [Required(ErrorMessage = "Pole nie może być puste.")]
        [MaxLength(30)]
        [Display(Name = "Nazwa kategorii")]
        public string Name { get; set; } = default!;

        public List<string> ExistingCategories { get; set; } = new();
    }
}
