using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Shared.AspNetCoreTools.Extensions;

public static class HttpContextExtensions {
    public static Task SignIn(
        this HttpContext context,
        Guid userId,
        string username,
        string role,
        bool rememberMe) {
        var claimsPrincipal = CreateClaims(userId, username, role);

        var authProperties = new AuthenticationProperties() {
            IsPersistent = rememberMe,
            ExpiresUtc = DateTime.UtcNow.AddDays(7)
        };

        return context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);
    }

    private static ClaimsPrincipal CreateClaims(
        Guid userId,
        string username,
        string role) {
        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, userId.ToString()!),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return claimsPrincipal;
    }
}
