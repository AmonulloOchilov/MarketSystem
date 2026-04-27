using Application.DTOs.Request;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Application.Validators.Product;
using Domain.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Repositories;
using MarketSystem.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var elasticUsername = builder.Configuration["Elastic:Username"];
var elasticPassword = builder.Configuration["Elastic:Password"];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information().WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        IndexFormat = "market-logs-{0:yyyy.MM.dd}",
        AutoRegisterTemplate = true,
        ModifyConnectionSettings = x => x
            .BasicAuthentication(elasticUsername, elasticPassword)
    })
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("Test log from API");

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<MarketDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddControllers();

builder.Services.AddScoped<ExceptionMiddleware>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
