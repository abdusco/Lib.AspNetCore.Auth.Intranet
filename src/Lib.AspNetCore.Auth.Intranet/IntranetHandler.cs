using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

#nullable enable
namespace Lib.AspNetCore.Auth.Intranet
{
    public class IntranetHandler : AuthenticationHandler<IntranetOptions>
    {
        public IntranetHandler(IOptionsMonitor<IntranetOptions> options,
                               ILoggerFactory logger,
                               UrlEncoder encoder,
                               ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected new IntranetEvents Events
        {
            get => (IntranetEvents) base.Events!;
            set => base.Events = value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.AllowedIpRanges == null)
            {
                Logger.LogDebug("Allowed IP ranges isn't set");
                return AuthenticateResult.NoResult();
            }

            var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);
            await Events.MessageReceived(messageReceivedContext);
            if (messageReceivedContext.Result != null)
            {
                return messageReceivedContext.Result;
            }

            var ipAddress = messageReceivedContext.IpAddress ?? Context.Connection.RemoteIpAddress;
            if (ipAddress == null)
            {
                return AuthenticateResult.Fail(
                    new ArgumentNullException(nameof(IPAddress), "IP address cannot be null"));
            }

            var matchedRange = Options.AllowedIpRanges.FirstOrDefault(range => range.Contains(ipAddress));
            if (matchedRange == null)
            {
                return AuthenticateResult.Fail(new SecurityException($"IP {ipAddress} isn't matched by any range"));
            }

            var matchContext = new AddressMatchedContext(Context, Scheme, Options)
                {IpAddress = ipAddress, IpAddressRange = matchedRange};
            await Events.AddressMatched(matchContext);
            if (matchContext.Result != null)
            {
                return matchContext.Result;
            }

            Logger.LogDebug("Connection from {IpAddress} is allowed by the range {IpRange}", ipAddress,
                matchedRange.ToString());
            var identity = new ClaimsIdentity(Scheme.Name);

            var hostname = await GetHostnameAsync(ipAddress) ?? ipAddress.ToString();
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
            return authenticatedContext.Result!;
        }

        private async Task<string?> GetHostnameAsync(IPAddress ipAddress)
        {
            var hostnameTask = Dns.GetHostEntryAsync(ipAddress);
            var timeoutTask = Task.Delay(Options.HostnameResolutionTimeout, Context.RequestAborted);
            var completed = await Task.WhenAny(hostnameTask, timeoutTask);
            if (completed == hostnameTask)
            {
                return hostnameTask.Result.HostName;
            }

            Logger.LogWarning("Hostname resolution for {IpAddress} timed out after {Timeout}", ipAddress,
                Options.HostnameResolutionTimeout);
            return null;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.Append(HeaderNames.WWWAuthenticate, IntranetDefaults.AuthenticationScheme);
            return Task.CompletedTask;
        }
    }
}