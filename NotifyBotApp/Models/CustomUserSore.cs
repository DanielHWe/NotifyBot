using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace NotifyBotApp.Models
{
    // <summary>
    /// Copy from https://github.com/leeenglestone/ASP.NET-Identity-Without-a-Database
    /// </summary>
    public class CustomUserSore<T> : IUserStore<T> where T : ApplicationUser
    {
        void IDisposable.Dispose()
        {
            // throw new NotImplementedException();

        }

        public Task CreateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<T> FindByNameAsync(string userName)
        {
            if (userName.Equals("root", StringComparison.Ordinal))
            {
                return (T) await Task.Run(() => new ApplicationUser()
                {
                    Id = "0",
                    UserName = userName
                });
            }

            return null;
        }
    }
}