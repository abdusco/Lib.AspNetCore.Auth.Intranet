using System.Collections.Generic;
using System.Linq;
using Lib.AspNetCore.Auth.Intranet;
using NetTools;

namespace SampleApi
{
    internal class AppIntranetOptions : IntranetAuthenticationOptions
    {
        public List<string> IpRanges { get; set; } = new List<string>();
        public List<string> AssignedRoles { get; set; } = new List<string>();
        public override IList<IPAddressRange> AllowedIpRanges => IpRanges.Select(IPAddressRange.Parse).ToList();
    }
}