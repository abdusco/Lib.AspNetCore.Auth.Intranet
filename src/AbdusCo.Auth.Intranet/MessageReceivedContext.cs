using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AbdusCo.Auth.Intranet
{
    public class MessageReceivedContext: ResultContext<IntranetAuthenticationOptions>
    {
        public MessageReceivedContext(HttpContext context, AuthenticationScheme scheme, IntranetAuthenticationOptions authenticationOptions) : base(context, scheme, authenticationOptions)
        {
        }

        public IPAddress IpAddress { get; set; }
    }
}