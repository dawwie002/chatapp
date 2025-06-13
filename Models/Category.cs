using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; } = default!;
    }
}
