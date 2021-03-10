# Lib.AspNetCore.Auth.Intranet

This is a small library for authenticating requests coming from inside an intranet (or any IP range) that integrates
with ASP.NET Core auth stack seamlessly.

[![](https://img.shields.io/nuget/v/Lib.AspNetCore.Auth.Intranet.svg)][nuget]

## Use case

If have an app in your organization that's normally protected by an auth scheme, but you want to allow certain machines
to access the app without any restrictions based on where the connection is coming from. For example you may want to
allow requests coming from CI/CD machines to certain controllers.

## Installation

Install the package from [Nuget][nuget]:

```
dotnet nuget add package Lib.AspNetCore.Auth.Intranet
``` 

## Setup

Set up authentication in `ConfigureServices` method your Startup class and add intranet authentication
using `AddIntranet` extension method. Remember to enable auth middlewares in `Configure` method.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();    
    services.AddAuthentication()
        .AddIntranet();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();
    // add auth middlewares after routing but before endpoints
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
}
```

If the application will be running behind a reverse proxy (IIS, Caddy, nginx etc.) you might want to enable proxy
headers. Otherwise all requests will be originating from localhost (usually).

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
    });
    // ...
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseForwardedHeaders();
    // ...
}
```

## Configuration

Simply adding the scheme without any options isn't really useful. By default it works based on whitelist principle, so
you must specify any IP address you want to let in:

```c#
services.AddAuthentication()
    .AddIntranet(options =>
    {
        options.AllowedIpRanges.Add(IPAddressRange.Parse("10.10.0.0/16"));
        options.AllowedIpRanges.Add(IPAddressRange.Parse("127.0.0.1"));
    });
```

Refer to [IPAddressRange](https://github.com/jsakamoto/ipaddressrange) library to learn about the syntax. 
You can check the how CIDRs are resolved [using this tool](https://www.ipaddressguide.com/cidr).

Once the request IP is matched by any of the ranges, a `ClaimsPrincipal` is created and populated with following the claims:

- **Name**: Hostname resolved for the IP.  
  Hostname resolution times out after 1 second, you can adjust it with `options.HostnameResolutionTimeout` option.
  If it times out, IP address is used as the name.
- **NameIdentifier**: IP address

You can hook into `OnAuthenticated` event and modify the claims principal.

```c#
services.AddAuthentication()
    .AddIntranet(options =>
    {
        options.Events.OnAuthenticated = context =>
        {
            // assign admin role
            var identity = (ClaimsIdentity) context.Principal.Identity;
            identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            
            return Task.CompletedTask;
        };
    });
```

## Usage

Then you can decorate your controllers or actions with `[Authorize]` attribute

```c#
[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [Authorize(AuthenticationSchemes = IntranetDefaults.AuthenticationScheme)]
    [HttpGet]
    public Task<ActionResult> UserInfo()
    {
        return Task.FromResult<ActionResult>(
            Ok(User.Claims.ToList().ToDictionary(claim => claim.Type, claim => claim.Value))
        );
    }
}
```

Check out the sample project to see how it's all configured using `appsettings.json`.

## TODO

- [ ] Whitelist and blacklist IP ranges
- [ ] Support for delegates

## Thanks

- J. Sakamoto for [IPAddressRange](https://github.com/jsakamoto/ipaddressrange) library

## License

[Mozilla Public License Version 2.0](https://github.com/abdusco/Lib.AspNetCore.Auth.Intranet/blob/master/LICENSE.txt)

[nuget]: https://www.nuget.org/packages/Lib.AspNetCore.Auth.Intranet