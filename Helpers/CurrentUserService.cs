using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickChat.MVC.Data;
using QuickChat.MVC.Models;
using System.Security.Claims;

namespace QuickChat.MVC.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }
        public string UserId {
            get
            {
                var id = this.httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
                if(string.IsNullOrEmpty(id))
                {
                    id = this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                return id;
            }
        }

        public async Task<ApplicationUser> GetUser()
        {
            var user  = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            return user;
        }

        public async Task<string?> GetRoleInWidget(Guid widgetId)
        {
            var userId = UserId;

            if (string.IsNullOrEmpty(userId))
                return null;

            var role = await dbContext.WidgetUsers
                .Where(wu => wu.WidgetId == widgetId && wu.UserId == userId)
                .Select(wu => wu.Role)
                .FirstOrDefaultAsync();

            return role;
        }

    }
}
