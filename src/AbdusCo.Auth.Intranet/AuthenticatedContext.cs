using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AbdusCo.Auth.Intranet
{
    public class AuthenticatedContext : ResultContext<IntranetAuthenticationOptions>
    {
        public AuthenticatedContext(HttpContext context, AuthenticationScheme scheme, IntranetAuthenticationOptions authenticationOptions) :
            base(context, scheme, authenticationOptions)
        {
        }
    }
}