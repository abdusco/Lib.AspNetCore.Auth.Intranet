using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lib.AspNetCore.Auth.Intranet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleApi
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = IntranetDefaults.AuthenticationScheme)]
        [HttpGet]
        public Task<ActionResult> UserInfo()
        {
            return Task.FromResult<ActionResult>(Ok(User.Claims.ToList()
                .ToDictionary(claim => claim.Type, claim => claim.Value)));
        }
    }
}