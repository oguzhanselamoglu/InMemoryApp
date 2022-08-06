using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "Hello World!");

app.MapGet("/index", (IMemoryCache _memoryCache) =>
{
    string date = "";
    if (!_memoryCache.TryGetValue<string>("date",out date))
    {
        MemoryCacheEntryOptions opt = new();
        opt.AbsoluteExpiration = DateTime.Now.AddSeconds(30);
        //ömrünü 10 sn artırır
        //opt.SlidingExpiration = DateTime.Now.AddSeconds(10);
        date = DateTime.Now.ToString();
        _memoryCache.Set<string>("date",date,opt);
    }
    return date;
 });

app.MapGet("/show", (IMemoryCache _memoryCache) => _memoryCache.Get<string>("date"));

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}