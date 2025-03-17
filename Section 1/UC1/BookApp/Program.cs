using Microsoft.EntityFrameworkCore;
using NLog.Web;
using RepositoryLayer.Context;

var builder = WebApplication.CreateBuilder(args);

// Configure the database connection
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();   //generate json files
app.UseSwaggerUI(); // make ui 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
