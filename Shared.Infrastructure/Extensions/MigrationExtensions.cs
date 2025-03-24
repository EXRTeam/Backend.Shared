using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Extensions;

public static class MigrationExtensions {
    public static void ApplyMigrations(this IApplicationBuilder builder) {
        using var scope = builder.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        dbContext.Database.Migrate();
    }
}