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


builder.Services.AddSignalR();

//repos
builder.Services
                //TODO: me li aggiunge tutti in automatico?
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services
    .AddSingleton<TorunamentCache>()
    .AddScoped<IStandingSubscriber, TournamentManager.Services.TournamentManager>()
    .AddScoped<IRawStandingSubscriber, RawStandingSubscriber>();

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MatchUpdateHub>("/matchUpdateHub");
});

app.Run();
