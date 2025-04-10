using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Infrastructure.Options;

namespace Shared.AspNetCoreTools.Extensions;

public static class AuthenticationExtensions {
    public static AuthenticationBuilder AddJwtAuthentication(
        this AuthenticationBuilder builder,
        Action<JwtAuthenticationOptions> configure) {
        var jwtOptions = new JwtAuthenticationOptions();
        configure(jwtOptions);

        builder.Services.Configure<JwtAuthenticationOptions>(options => {
            options.Issuer = jwtOptions.Issuer;
            options.Audience = jwtOptions.Audience;
            options.Key = jwtOptions.Key;

            options.AccessTokenLifetimeInMinutes = jwtOptions.AccessTokenLifetimeInMinutes;
            options.RefreshTokenLifetimeInDays = jwtOptions.RefreshTokenLifetimeInDays;
        });

        builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.Key),
                ValidateLifetime = true,
            });

        return builder;
    }
}
