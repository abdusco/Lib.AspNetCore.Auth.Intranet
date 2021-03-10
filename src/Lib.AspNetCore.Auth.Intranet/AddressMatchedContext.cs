using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NetTools;

namespace Lib.AspNetCore.Auth.Intranet
{
    public class AddressMatchedContext : ResultContext<IntranetAuthenticationOptions>
    {
        public AddressMatchedContext(HttpContext context, AuthenticationScheme scheme, IntranetAuthenticationOptions options) :
            base(context, scheme, options)
        {
        }

        public IPAddress IpAddress { get; set; }
        public IPAddressRange IpAddressRange { get; set; }
    }
}