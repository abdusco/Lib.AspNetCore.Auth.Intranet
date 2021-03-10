using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lib.AspNetCore.Auth.Intranet.Tests
{
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Enables the <see cref="TestServer" /> service.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseTestServer(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IHostLifetime, NoopHostLifetime>();
                services.AddSingleton<IServer, TestServer>();
            });
        }

        private class NoopHostLifetime : IHostLifetime
        {
            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            Task IHostLifetime.WaitForStartAsync(CancellationToken cancellationToken)
            {
                return WaitForStartAsync(cancellationToken);
            }

            public Task WaitForStartAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}