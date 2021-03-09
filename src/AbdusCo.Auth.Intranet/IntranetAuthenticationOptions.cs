using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using NetTools;

namespace AbdusCo.Auth.Intranet
{
    public class IntranetAuthenticationOptions : AuthenticationSchemeOptions
    {
        public IntranetAuthenticationOptions()
        {
            Events = new IntranetAuthenticationEvents();
        }

        public new IntranetAuthenticationEvents Events
        {
            get => (IntranetAuthenticationEvents) base.Events;
            set => base.Events = value;
        }
        public virtual IList<IPAddressRange> AllowedIpRanges { get; set; } = new List<IPAddressRange>();
    }
}