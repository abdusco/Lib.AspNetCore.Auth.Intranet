using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTools;
using Xunit;

namespace Lib.AspNetCore.Auth.Intranet.Tests
{
    public class IntranetTests
    {
        [Theory]
        [InlineData("1.1.1.0/24", "1.1.1.1", HttpStatusCode.OK)]
        [InlineData("127.0.0.1", "127.0.0.1", HttpStatusCode.OK)]
        [InlineData("1.1.1.0/24", "1.1.2.1", HttpStatusCode.Unauthorized)]
        [InlineData("127.0.0.1", "1.1.2.1", HttpStatusCode.Unauthorized)]
        [InlineData("127.0.0.1", null, HttpStatusCode.Unauthorized)]
        public async Task IpMatchWorks(string range, string ip, HttpStatusCode statusCode)
        {
            using var host = await CreateHost(o => { o.AllowedIpRanges.Add(IPAddressRange.Parse(range)); });

            var client = host.GetTestClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            if (ip != null)
            {
                request.Headers.Add("X-Forwarded-For", ip);
            }

            var response = await client.SendAsync(request);
            Assert.Equal(statusCode, response.StatusCode);
        }

        [Fact]
        public async Task ClaimsAreSet()
        {
            var ipAddress = "1.1.1.1";
            using var host = await CreateHost(o => { o.AllowedIpRanges.Add(IPAddressRange.Parse(ipAddress)); });

            var client = host.GetTestClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.Headers.Add("X-Forwarded-For", ipAddress);
            var response = await client.SendAsync(request);
            Assert.Equal(ipAddress, await response.Content.ReadAsStringAsync());
        }

        private async Task<IHost> CreateHost(Action<IntranetOptions> configure)
        {
            var host = new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.UseTestServer()
                        .ConfigureServices(collection =>
                            collection
                                .AddAuthentication(IntranetDefaults.AuthenticationScheme).AddIntranet(configure))
                        .Configure(applicationBuilder => applicationBuilder
                            .UseForwardedHeaders(new ForwardedHeadersOptions
                            {
                                ForwardedHeaders = ForwardedHeaders.All
                            })
                            .UseAuthentication()
                            .Use(async (context, next) =>
                            {
                                var result = await context.AuthenticateAsync();
                                if (!result.Succeeded)
                                {
                                    context.Response.StatusCode = 401;
                                    return;
                                }

                                await context.Response.WriteAsync(
                                    context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                            }));
                }).Build();
            await host.StartAsync();
            return host;
        }
    }
}