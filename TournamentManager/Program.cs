using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
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

//// Generic Repository
builder.Services.AddScoped<IGenericRepository<Match>, GenericRepository<Match>>()
    .AddScoped<IGenericRepository<Division>, GenericRepository<Division>>()
    .AddScoped<IGenericRepository<Phase>, GenericRepository<Phase>>()
    .AddScoped<IGenericRepository<Round>, GenericRepository<Round>>()
    .AddScoped<IGenericRepository<Song>, GenericRepository<Song>>()
    .AddScoped<IGenericRepository<Player>, GenericRepository<Player>>()
    .AddScoped<IGenericRepository<Standing>, GenericRepository<Standing>>();
    //TODO: me li aggiunge tutti in automatico?
    //.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services
    .AddSingleton<IRawStandingSubscriber, RawStandingSubscriber>()
    .AddSingleton<IStandingSubscriber, TournamentManager.Services.TournamentManager>();

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

app.MapControllers();

app.Run();
