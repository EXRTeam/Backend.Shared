using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shared.AspNetCoreTools.Extensions;

public static class AuthenticationExtensions {
    public static AuthenticationBuilder AddJwtAuthentication(
        this AuthenticationBuilder builder,
        IConfiguration jwtOptionsSection) 
        => builder
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptionsSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtOptionsSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptionsSection["Key"]!)),
                    ValidateLifetime = true,
                });
}
