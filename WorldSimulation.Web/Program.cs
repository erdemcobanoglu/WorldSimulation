using WorldSimulation.Application.Interfaces;
using WorldSimulation.Application.Service;
using WorldSimulation.Application.WorldMapService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Dependency Injection yapılandırması
builder.Services.AddScoped<IWorldMapService, WorldMapService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
// Not: IOceanEventService ve IWeatherSimulationEngine, runtime parametreye ihtiyaç duyduğu için burada değil

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
