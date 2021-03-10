using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using NetTools;

namespace Lib.AspNetCore.Auth.Intranet
{
    public class IntranetOptions : AuthenticationSchemeOptions
    {
        public IntranetOptions()
        {
            Events = new IntranetEvents();
        }

        public new IntranetEvents Events
        {
            get => (IntranetEvents) base.Events;
            set => base.Events = value;
        }

        public virtual IList<IPAddressRange> AllowedIpRanges { get; set; } = new List<IPAddressRange>();

        public TimeSpan HostnameResolutionTimeout { get; set; } = TimeSpan.FromSeconds(1);
    }
}