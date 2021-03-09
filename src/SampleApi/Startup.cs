using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AbdusCo.Auth.Intranet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
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
                    options.Events.OnAuthenticated = context =>
                    {
                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(
                            intranetOptions.AssignedRoles
                                .Select(r => new Claim(ClaimTypes.Role, r))
                                .Concat(context.Principal.Claims), context.Scheme.Name));

                        return Task.CompletedTask;
                    };
                    options.AllowedIpRanges = intranetOptions.AllowedIpRanges;
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