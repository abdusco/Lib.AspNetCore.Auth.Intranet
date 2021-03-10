using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NetTools;

namespace Lib.AspNetCore.Auth.Intranet
{
    public class AddressMatchedContext : ResultContext<IntranetOptions>
    {
        public AddressMatchedContext(HttpContext context, AuthenticationScheme scheme, IntranetOptions options) :
            base(context, scheme, options)
        {
        }

        public IPAddress IpAddress { get; set; }
        public IPAddressRange IpAddressRange { get; set; }
    }
}