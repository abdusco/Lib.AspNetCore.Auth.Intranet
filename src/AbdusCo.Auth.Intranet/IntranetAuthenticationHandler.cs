using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTools;

namespace AbdusCo.Auth.Intranet
{
    public class IntranetAuthenticationHandler : AuthenticationHandler<IntranetAuthenticationOptions>
    {
        public IntranetAuthenticationHandler(IOptionsMonitor<IntranetAuthenticationOptions> options,
                                             ILoggerFactory logger,
                                             UrlEncoder encoder,
                                             ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected new IntranetAuthenticationEvents Events
        {
            get => (IntranetAuthenticationEvents) base.Events;
            set => base.Events = value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);
            await Events.MessageReceived(messageReceivedContext);
            if (messageReceivedContext.Result != null)
            {
                return messageReceivedContext.Result;
            }

            var ipAddress = messageReceivedContext.IpAddress ?? Context.Connection.RemoteIpAddress;

            if (!Options.AllowedIpRanges.Any(range => range.Contains(ipAddress)))
            {
                return AuthenticateResult.Fail(new SecurityException($"IP {ipAddress} isn't matched by any range"));
            }

            var identity = new ClaimsIdentity(Scheme.Name);

            var hostname = await GetHostnameAsync(ipAddress);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, ipAddress.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, hostname));

            var principal = new ClaimsPrincipal(identity);
            var authenticatedContext = new AuthenticatedContext(Context, Scheme, Options) {Principal = principal};
            await Events.Authenticated(authenticatedContext);
            if (authenticatedContext.Result != null)
            {
                return authenticatedContext.Result;
            }

            principal = authenticatedContext.Principal;
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(authenticatedContext.Principal));
            }

            authenticatedContext.Success();
            return authenticatedContext.Result;
        }

        private async Task<string> GetHostnameAsync(IPAddress ipAddress)
        {
            var result = await Dns.GetHostEntryAsync(ipAddress);
            return result.HostName;
        }
    }
}