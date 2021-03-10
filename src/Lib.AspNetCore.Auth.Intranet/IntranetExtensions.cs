using System;
using Microsoft.AspNetCore.Authentication;

namespace Lib.AspNetCore.Auth.Intranet
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddIntranet(this AuthenticationBuilder builder,
                                                        Action<IntranetAuthenticationOptions> configureOptions)
            => AddIntranet(builder, IntranetAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddIntranet(this AuthenticationBuilder builder,
                                                        string authenticationScheme,
                                                        Action<IntranetAuthenticationOptions> configureOptions)
            => AddIntranet(builder,
                authenticationScheme: authenticationScheme,
                displayName: null,
                configureOptions: configureOptions);

        public static AuthenticationBuilder AddIntranet(this AuthenticationBuilder builder,
                                                        string authenticationScheme,
                                                        string displayName,
                                                        Action<IntranetAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<IntranetAuthenticationOptions, IntranetAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );
        }
    }
}