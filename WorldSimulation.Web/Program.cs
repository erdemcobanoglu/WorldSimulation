using WorldSimulation.Application.Interfaces;
using WorldSimulation.Application.Service;
using WorldSimulation.Application.WorldMapService;
using WorldSimulation.Infrastructure.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IWorldMapService, WorldMapService>();
builder.Services.AddScoped<IWeatherService, WeatherService>(); 
builder.Services.AddSingleton<IMapProvider, MapProvider>();

// ✅ CORS yapılandırması
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000") // React dev sunucusu
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

// ✅ CORS middleware aktif ediliyor
app.UseCors();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
