using Microsoft.EntityFrameworkCore;

using System.Reflection;
using TournamentManager.Contexts;
using TournamentManager.Services;

using TournamentManager.Controllers;


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
    .AddSingleton<Scheduler>()
    .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
    .AddScoped<IScoreCalculator, TeamScoreCalculator>() //To restore the eurocup use MatchScoreCalculator
    .AddScoped<IMatchManager, MatchManager>()
    .AddScoped<IStandingManager, StandingManager>()
    .AddScoped<ISongRoller, SongRoller>()
    .AddScoped<IMatchUpdate, NotificationHub>()
    .AddScoped<ILogUpdate, NotificationHub>()
    .AddScoped<IScoreUpdate, NotificationHub>()
    .AddScoped<AuthorizationFilterAttribute>()
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

app.UseWebSockets();

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
    endpoints.MapHub<ScoreUpdateHub>("/scoreUpdateHub");
});

app.Run();
