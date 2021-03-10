# Lib.AspNetCore.Auth.Intranet

This is a small library for authenticating requests coming from inside an intranet (or any IP range) that integrates
with ASP.NET Core auth stack seamlessly.

## Use case

If have an app in your organization that's normally protected by an auth scheme, but you want to allow certain machines
to access the app without any restrictions based on where the connection is coming from. For example you may want to
allow requests coming from CI/CD machines to certain controllers.

## Installation

Install the package from Nuget:

```
dotnet nuget add package Lib.AspNetCore.Auth.Intranet
``` 

## Usage

Set up authentication in your Startup class and add intranet authentication using `AddIntranet` extension method.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
    });
    
    services.AddControllers();
    
    var intranetOptions = Configuration.GetSection("IntranetAuth").Get<AppIntranetOptions>();
    services.AddAuthentication()
        .AddIntranet();
}
```

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

Once the request IP is matched by any of the ranges, a `ClaimsPrincipal` is created with following claims:

- Name: Hostname resolved for the IP.  
  Hostname resolution times out after 1 second, you can adjust it with `options.HostnameResolutionTimeout` option.
- NameIdentifier: IP address

You can hook up to `options.Events.OnAuthenticated` and modify the claims principal.

```c#
services.AddAuthentication()
    .AddIntranet(options =>
    {
        options.Events.OnAuthenticated = context =>
        {
            // assign admin role
            context.Principal = new ClaimsPrincipal(new ClaimsIdentity(
                new []{new Claim(ClaimTypes.Role, "admin")}.Concat(context.Principal.Claims), 
                context.Scheme.Name
            ));

            return Task.CompletedTask;
        };
    });
```

Then you can decorate your controllers or actions with `[Authorize]` attribute

```c#
[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [Authorize(AuthenticationSchemes = IntranetAuthenticationDefaults.AuthenticationScheme)]
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

- Whitelist and blacklist IP ranges
- Support for delegates

## Thanks

- J. Sakamoto for [IpAddressRange](https://github.com/jsakamoto/ipaddressrange) library