namespace QuickChat.MVC.ViewModels.WidgetViewModels
{
    public class WidgetEditPanelViewModel
    {
        public Guid WidgetId { get; set; }
        public EditWidgetViewModel EditModel { get; set; } = default!;
        public ManageTeamViewModel TeamModel { get; set; } = default!;
        public CategoryViewModel CategoryModel { get; set; } = default!;

    }

}
