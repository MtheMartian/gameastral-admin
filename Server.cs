using GameStarBackend.Api.Config;
using GameStarBackend.Api.Models;
using GameStarBackend.Api.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;
using System.Runtime.Intrinsics.Arm;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to container.
builder.Services.Configure<GameInfoDatabaseSettings>(
    builder.Configuration.GetSection("GameInfoDatabase"));

builder.Services.AddSingleton<GamesService>();
builder.Services.AddSingleton<ReviewsService>();
builder.Services.AddSingleton<AdminsService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddSingleton<AuthProperties>();

//Cookie Scheme
//auth.CookieSchema();
builder.Services.AddAuthentication()
    .AddCookie("default", o =>
    {
        o.Cookie.Name = "_auth";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "_GSAdmin_session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
});

builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();

// CORS Stuff
var AllowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
        policy =>
        {
            policy.WithOrigins("https://gameastral-057014ee9b02.herokuapp.com", "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseAuthentication();
app.UseSession();

// Load .Env variables
DotNetEnv.Env.Load();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("I am running, don't worry.");

app.Run();



