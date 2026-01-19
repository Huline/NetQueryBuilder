using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.AspNetCoreSampleApp.Data;
using NetQueryBuilder.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("QueryBuilderDemo");
    options.UseLazyLoadingProxies();
});

// Add NetQueryBuilder with EF Core integration (registers all required services)
builder.Services.AddNetQueryBuilder<AppDbContext>(options =>
{
    options.SessionTimeout = TimeSpan.FromMinutes(30);
    options.DefaultPageSize = 10;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseNetQueryBuilder(); // Handles session + static files
app.MapRazorPages();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.Run();
