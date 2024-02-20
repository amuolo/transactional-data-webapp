using Microsoft.EntityFrameworkCore;

namespace App.Data;

public class DbCatalogContext : DbContext
{
    public DbCatalogContext (DbContextOptions<DbCatalogContext> options)
        : base(options)
    {
    }

    public DbSet<Model.Transaction> TransactionalData { get; set; } = default!;
}
