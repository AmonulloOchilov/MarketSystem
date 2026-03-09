using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<MarketDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/categories", async (ICategoryService service) =>
{
    return await service.GetAllAsync();
});

app.MapGet("/categories{id}", (int id, ICategoryService service) =>
{
    service.GetByIdAsync(id);
});

app.MapPost("/categories", async (ICategoryService service, Category category) =>
{
    var created = await service.CreateAsync(category);
    return Results.Created($"/categories/{created.Id}", created);
});




app.MapGet("/products", async (IProductService service) =>
{
    return await service.GetAllAsync();
});

app.MapGet("/products{id}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    if (product == null)
    {
        Results.NotFound();
    }

    return Results.Ok(product);
});

app.MapPost("/products", async (Product product, IProductService service) =>
{
    var created = await service.CreateAsync(product);
    return Results.Created($"/products/{created.Id}", created);
});

//Finish the CRUD for both!!!

app.Run();
