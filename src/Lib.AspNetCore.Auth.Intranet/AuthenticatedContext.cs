using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Lib.AspNetCore.Auth.Intranet
{
    public class AuthenticatedContext : ResultContext<IntranetOptions>
    {
        public AuthenticatedContext(HttpContext context, AuthenticationScheme scheme, IntranetOptions options) :
            base(context, scheme, options)
        {
        }
    }
}