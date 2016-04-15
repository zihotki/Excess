using System.Web;
using Microsoft.AspNet.Identity;

namespace Excess.Web.Services
{
    public class UserService : IUserServices
    {
        public string UserId()
        {
            return HttpContext.Current.User.Identity.GetUserId();
        }
    }
}