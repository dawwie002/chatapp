using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.Models;

namespace QuickChat.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<WidgetUser> WidgetUsers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
            .HasOne(c => c.Widget)
            .WithMany(w => w.Categories)
            .HasForeignKey(c => c.WidgetId)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
