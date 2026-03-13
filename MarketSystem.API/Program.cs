using Application.DTOs.Request;
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
app.MapGet("/", () => Results.Redirect("/swagger"));


app.MapGet("/categories", async (ICategoryService service) =>
{
    return await service.GetAllAsync();
});

app.MapGet("/categories/{id}", async (int id, ICategoryService service) =>
{
    var category = await service.GetByIdAsync(id);
    if (category == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(category);
    
});

app.MapPost("/categories", async (CreateCategoryRequest request, ICategoryService service) =>
{
    var created = await service.CreateAsync(request);
    return Results.Created($"/categories/{created.Id}", created);
});

app.MapPut("/categories", async (ICategoryService service, UpdateCategoryRequest request,int id) =>
{
    var updated = await service.UpdateAsync(id, request);
    if (updated == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(updated);
});

app.MapDelete("/categories/{id}", async (ICategoryService service, int id) =>
{
    var deleted = await service.DeleteAsync(id);
    if (deleted == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(deleted);
});



app.MapGet("/products", async (IProductService service) =>
{
    var result = await service.GetAllAsync();
    return Results.Ok(result);
});

app.MapGet("/products/{id}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    if (product == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(product);
});

app.MapPost("/products", async (CreateProductRequest request, IProductService service) =>
{
    var created = await service.CreateAsync(request);
    return Results.Created($"/products/{created.Id}", created);
});


app.Run();
