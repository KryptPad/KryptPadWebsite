using KryptPadWebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KryptPadWebApp.Helpers
{
    class AuthorizedDeviceHelper
    {
        public static async Task<bool> AddAuthorizedDevice(string userId, Guid appId, string ipAddress)
        {
            using (var ctx = new ApplicationDbContext())
            {
                // Check if device is already authorized
                var authorizedId = ctx.AuthorizedDevices.Where(x => x.User.Id == userId && x.AppId == appId).FirstOrDefault();
                if (authorizedId != null)
                {
                    // This device is already authorized
                    return false;
                }

                // Get the user
                var user = ctx.Users.Find(userId);
                if (user != null)
                {

                    var authorizedDevice = new Models.Entities.AuthorizedDevice()
                    {
                        User = user,
                        AppId = appId,
                        AccessedFromIPAddress = ipAddress,
                        DateAuthorized = DateTime.UtcNow
                    };

                    // Add the authorized device
                    ctx.AuthorizedDevices.Add(authorizedDevice);

                    await ctx.SaveChangesAsync();

                    return true;

                }
            }

            return false;
        }
    }
}