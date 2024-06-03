using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Net;
using System.Reflection;
using TournamentManager.Contexts;
using TournamentManager.Services;
using TournamentManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TournamentDbContext>(options =>
{
    var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    options.UseSqlite($"Data Source={exeDir}/DB/db.db;Pooling=False");
});

builder.Services.AddSignalR();

builder.Services
    .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
    .AddScoped<IMatchManager, MatchManager>()
    .AddScoped<IStandingManager, StandingManager>()
    .AddScoped<ISongRoller, SongRoller>()
    .AddScoped<IMatchUpdate, NotificationHub>()
    .AddScoped<ILogUpdate, NotificationHub>()
    .AddSingleton<ITournamentCache, TournamentCache>();

// cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MatchUpdateHub>("/matchUpdateHub");
    endpoints.MapHub<LogUpdateHub>("/logUpdateHub");
});

app.UseWebSockets();
app.UseMiddleware<StandingService>();

app.Run();
