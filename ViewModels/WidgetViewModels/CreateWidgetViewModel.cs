using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class CreateWidgetViewModel
    {
        [Required(ErrorMessage = "Pole nie może być puste.")]
        [MaxLength(20, ErrorMessage = "Maksymalnie 20 znaków.")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; } = default!;
    }
}