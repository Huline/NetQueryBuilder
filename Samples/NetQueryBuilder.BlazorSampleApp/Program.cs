using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using NetQueryBuilder.BlazorSampleApp;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseInMemoryDatabase("InMemoryDb");
    options.UseLazyLoadingProxies();
});
builder.Services.AddScoped<IQueryConfigurator, EfQueryConfigurator<MyDbContext>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using var scope = app.Services.CreateScope();
var provider = scope.ServiceProvider;
var dbContext = provider.GetRequiredService<MyDbContext>();
await dbContext.SeedDatabase();

app.Run();