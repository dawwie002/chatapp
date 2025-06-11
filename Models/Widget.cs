using System.ComponentModel.DataAnnotations;

namespace QuickChat.MVC.Models
{
    public class Widget
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!; // np. nazwa sklepu
        public string PublicIdentifier { get; set; } = Guid.NewGuid().ToString(); // używany w skrypcie do osadzenia
        public virtual ICollection<WidgetUser> WidgetUsers { get; set; } = new List<WidgetUser>();
        public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}