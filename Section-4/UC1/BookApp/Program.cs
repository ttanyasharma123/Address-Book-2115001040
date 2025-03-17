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
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using YourNamespace.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register database
builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register Redis Cache
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

// Add FluentValidation services
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddressBookEntryDTOValidator>());

builder.Services.AddScoped<IValidator<AddressBookEntryDTO>, AddressBookEntryDTOValidator>();


builder.Services.AddScoped<IAddRL, AddRL>();
//builder.Services.AddScoped<IAddBL, AddBL>();
builder.Services.AddScoped<IAddressBookService, AddressBookService>();

builder.Services.AddScoped<IAuthBL, AuthBL>();
builder.Services.AddScoped<IUserRL, UserRL>();

builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();

try
{
    builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Redis service registration failed: {ex.Message}");
}

builder.Services.AddSingleton<IRabbitMQPublisherService, RabbitMQPublisherService>();
builder.Services.AddSingleton<IRabbitMQConsumerService, RabbitMQConsumerService>();

// Start the Consumer Service
var serviceProvider = builder.Services.BuildServiceProvider();
var rabbitMQConsumer = serviceProvider.GetService<IRabbitMQConsumerService>();
Task.Run(() => rabbitMQConsumer.ConsumeMessages());


// JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
