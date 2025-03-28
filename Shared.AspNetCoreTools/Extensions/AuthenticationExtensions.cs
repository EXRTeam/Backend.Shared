using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Shared.Infrastructure.Options;
using System.Text;

namespace Shared.AspNetCoreTools.Extensions;

public static class AuthenticationExtensions {
    public static AuthenticationBuilder AddJwtAuthentication(
        this AuthenticationBuilder builder,
        IConfiguration jwtOptionsSection) {
        var key = Encoding.UTF8.GetBytes(jwtOptionsSection["Key"]!);
        var issuer = jwtOptionsSection["Issuer"]!;
        var audience = jwtOptionsSection["Audience"]!;

        builder.Services.TryAddSingleton(new JwtAuthenticationOptions {
            Key = key,
            Issuer = issuer,
            Audience = audience,
        });

        builder
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                });

        return builder;
    }
}
