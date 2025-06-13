using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.Models
{
    public class Widget
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // automatyczne generowanie GUID przy tworzeniu
        public string Name { get; set; } = default!; // np. nazwa sklepu
        public virtual ICollection<WidgetUser> WidgetUsers { get; set; } = new List<WidgetUser>();
        public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}