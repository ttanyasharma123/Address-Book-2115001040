using BusinessLayer.Helper;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using BusinessLayer.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using NLog.Web;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register database
builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add FluentValidation services
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddressBookEntryDTOValidator>());

builder.Services.AddScoped<IValidator<AddressBookEntryDTO>, AddressBookEntryDTOValidator>();


builder.Services.AddScoped<IAddRL, AddRL>();
//builder.Services.AddScoped<IAddBL, AddBL>();
builder.Services.AddScoped<IAddressBookService, AddressBookService>();


builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
