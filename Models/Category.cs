namespace QuickChat.MVC.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}
