using System;
using Microsoft.AspNetCore.Authentication;

namespace Lib.AspNetCore.Auth.Intranet
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddIntranet(this AuthenticationBuilder builder,
                                                        Action<IntranetOptions> configureOptions)
            => AddIntranet(builder, IntranetDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddIntranet(this AuthenticationBuilder builder,
                                                        string authenticationScheme,
                                                        Action<IntranetOptions> configureOptions)
            => AddIntranet(builder,
                authenticationScheme: authenticationScheme,
                displayName: null,
                configureOptions: configureOptions);

        public static AuthenticationBuilder AddIntranet(this AuthenticationBuilder builder,
                                                        string authenticationScheme,
                                                        string displayName,
                                                        Action<IntranetOptions> configureOptions)
        {
            return builder.AddScheme<IntranetOptions, IntranetHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );
        }
    }
}