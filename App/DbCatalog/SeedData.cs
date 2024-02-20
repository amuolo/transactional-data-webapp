using App.Data;
using Microsoft.EntityFrameworkCore;
using Model.Initialization;

namespace App.DbCatalog;

public static class SeedData
{
    public static void Initialize(this IServiceProvider serviceProvider)
    {
        using (var context = new DbCatalogContext(serviceProvider.GetRequiredService<DbContextOptions<DbCatalogContext>>()))
        {
            if (!context.TransactionalData.Any())
            {
                context.TransactionalData.AddRange(Initialization.TransactionalData());
                context.SaveChanges();
            }

            if (!context.ActivityLogs.Any())
            {
                context.ActivityLogs.AddRange(Initialization.ActivityLog());
                context.SaveChanges();
            }
        }
    }
}
