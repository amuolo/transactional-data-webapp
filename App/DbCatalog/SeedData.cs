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
            if (context.TransactionalData.Any())
                return;

            context.TransactionalData.AddRange(Initialization.TransactionalData());

            context.SaveChanges();
        }
    }
}
