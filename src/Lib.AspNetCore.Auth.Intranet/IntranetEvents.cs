using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lib.AspNetCore.Auth.Intranet
{
    public class IntranetEvents
    {
        /// <summary>
        /// Invoked right after authentication handler has been called. You can set IP address here 
        /// </summary>
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked after IP address is matched by an IP range specified in options
        /// </summary>
        public Func<AddressMatchedContext, Task> OnAddressMatched { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked after a <see cref="ClaimsPrincipal"/> has been created. You can modify principal here.
        /// </summary>
        public Func<AuthenticatedContext, Task> OnAuthenticated { get; set; } = context => Task.CompletedTask;

        public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);
        public virtual Task AddressMatched(AddressMatchedContext context) => OnAddressMatched(context);
        public virtual Task Authenticated(AuthenticatedContext context) => OnAuthenticated(context);
    }
}