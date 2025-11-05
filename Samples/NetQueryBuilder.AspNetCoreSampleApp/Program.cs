using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.AspNetCoreSampleApp.Data;
using NetQueryBuilder.AspNetCore.Extensions;
using NetQueryBuilder.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add NetQueryBuilder with session support
builder.Services.AddNetQueryBuilder(options =>
{
    options.SessionTimeout = TimeSpan.FromMinutes(30);
    options.DefaultPageSize = 10;
});

// Add Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("QueryBuilderDemo");
    options.UseLazyLoadingProxies();
});

// Add Query Configurator
builder.Services.AddScoped<NetQueryBuilder.Configurations.IQueryConfigurator, EfQueryConfigurator<AppDbContext>>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // IMPORTANT: Must be after UseRouting and before MapRazorPages
app.MapRazorPages();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.Run();
