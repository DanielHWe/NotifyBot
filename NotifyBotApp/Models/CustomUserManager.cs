using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace NotifyBotApp.Models
{
    // <summary>
    /// Copy from https://github.com/leeenglestone/ASP.NET-Identity-Without-a-Database
    /// </summary>
    public class CustomUserManager : UserManager<ApplicationUser>
    {
        public CustomUserManager()
            : base(new CustomUserSore<ApplicationUser>())
        {

        }

        public CustomUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }

        public override Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var taskInvoke = Task<ApplicationUser>.Factory.StartNew(() =>
            {
                if (userName == "username" && password == "password")
                {
                    return new ApplicationUser { Id = "NeedsAnId", UserName = "UsernameHere" };
                }
                return null;
            });

            return taskInvoke;
        }
    }
}