using GameStore.Api.Services;
using GameStore.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IGameCatalogService, GameCatalogService>(client =>
{
    client.BaseAddress = new Uri("https://www.freetogame.com/api/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<IGameRepository, GameRepository>();

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields =
        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod |
        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode |
        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.Duration;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", () => Results.Redirect("/swagger"));
}
else
{
    app.MapGet("/", () => Results.Ok("GameStore API is running."));
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();