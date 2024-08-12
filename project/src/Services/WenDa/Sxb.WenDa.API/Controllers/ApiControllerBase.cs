using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Sxb.Framework.AspNetCoreHelper;

namespace Sxb.WenDa.API.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        public Guid UserId => GetUserId();

        private Guid GetUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                //int.TryParse
                return User.Identity.GetID();
            }
            return default;
        }
    }
}
