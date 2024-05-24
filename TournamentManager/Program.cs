using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

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
builder.Services.AddScoped<IGenericRepository<Match>, GenericRepository<Match>>();
builder.Services.AddScoped<IGenericRepository<Division>, GenericRepository<Division>>();
builder.Services.AddScoped<IGenericRepository<Phase>, GenericRepository<Phase>>();
builder.Services.AddScoped<IGenericRepository<Round>, GenericRepository<Round>>();
builder.Services.AddScoped<IGenericRepository<Song>, GenericRepository<Song>>();
builder.Services.AddScoped<IGenericRepository<Player>, GenericRepository<Player>>();
builder.Services.AddScoped<IGenericRepository<Standing>, GenericRepository<Standing>>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddSingleton<ITournamentInfoContainer, DbTournamentInfoContainer>();

builder.Services.AddSingleton<TournamentManager.Services.TournamentManager>();

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
