using Microsoft.EntityFrameworkCore;
using App.Data;
using App.DbCatalog;
using Posting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// (Console: SqlLocalDb create "TransactionalDataDB", Add-Migration InitialCreate, Update-Database)
 builder.Services.AddDbContext<DbCatalogContext>(options => options.UseInMemoryDatabase("Default"));
//builder.Services.AddDbContext<DbCatalogContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DbCatalogContext") 
//                            ?? throw new InvalidOperationException("Connection string not found.")));

builder.Services.AddSignalR();

builder.Services.AddSingleton<PostBox>()
                .AddHostedService<PostOffice>();

builder.WebHost.UseUrls(Contract.Url);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.Initialize();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios,
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapHub<MessageHub>(Contract.MessageHubPath);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
