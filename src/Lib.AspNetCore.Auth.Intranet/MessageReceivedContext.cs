using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Lib.AspNetCore.Auth.Intranet
{
    public class MessageReceivedContext: ResultContext<IntranetOptions>
    {
        public MessageReceivedContext(HttpContext context, AuthenticationScheme scheme, IntranetOptions options) : base(context, scheme, options)
        {
        }

        public IPAddress IpAddress { get; set; }
    }
}