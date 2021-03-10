using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lib.AspNetCore.Auth.Intranet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTools;

namespace SampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private class AppIntranetOptions
        {
            public List<string> IpRanges { get; set; } = new List<string>();
            public List<string> AssignedRoles { get; set; } = new List<string>();
            public List<IPAddressRange> AllowedIpRanges => IpRanges.Select(IPAddressRange.Parse).ToList();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            services.AddControllers();

            var intranetOptions = Configuration.GetSection("IntranetAuth").Get<AppIntranetOptions>();
            services.AddAuthentication()
                .AddIntranet(options =>
                {
                    options.AllowedIpRanges = intranetOptions.AllowedIpRanges;
                    options.HostnameResolutionTimeout = TimeSpan.FromSeconds(1);

                    options.Events.OnAuthenticated = context =>
                    {
                        var identity = (ClaimsIdentity) context.Principal.Identity;
                        identity.AddClaims(
                            intranetOptions.AssignedRoles.Select(r => new Claim(ClaimTypes.Role, r))
                        );
                        return Task.CompletedTask;
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}