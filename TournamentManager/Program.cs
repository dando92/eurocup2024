using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TournamentManager.Contexts;
using TournamentManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TournamentDbContext>(options =>
{
    var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    options.UseSqlite($"Data Source={exeDir}/DB/db.db");
});

builder.Services.Configure<StandingServiceConfiguration>(builder.Configuration.GetSection("StandingServiceConfiguration"));

builder.Services.AddSignalR();

builder.Services
    .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
    .AddScoped<TournamentManager.Services.TournamentManager>()
    .AddScoped<IMatchUpdate, NotificationHub>()
    .AddSingleton<TournamentCache>()
    .AddScoped<IStandingSubscriber, TournamentManager.Services.TournamentManager>()
    .AddScoped<IRawStandingSubscriber, RawStandingSubscriber>()
    .AddSingleton<IHostedService, StandingService>();

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
});


app.Run();
