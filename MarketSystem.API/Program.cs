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
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

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

app.MapPut("/products", async (IProductService service, UpdateProductRequest request, int id) =>
{
    var updated = await service.UpdateAsync(id, request);
    if (updated == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updated);
});

app.MapDelete("/products/{id}", async (IProductService service, int id) =>
{
    var deleted = await service.DeleteAsync(id);
    if (deleted == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(deleted);
});



app.MapGet("/customers", async (ICustomerService service) =>
{
    var result = await service.GetAllAsync();
    return Results.Ok(result);
});

app.MapGet("/customers/{id}", async (int id, ICustomerService service) =>
{
    var result = await service.GetByIdAsync(id);

    if (result == null)
        return Results.NotFound();

    return Results.Ok(result);
});

app.MapPost("/customers", async (CreateCustomerRequest request, ICustomerService service) =>
{
    var result = await service.CreateAsync(request);
    return Results.Created($"/customers/{result.Id}", result);
});

app.MapPut("/customers/{id}", async (int id, UpdateCustomerRequest request, ICustomerService service) =>
{
    var result = await service.UpdateAsync(id, request);

    if (result == null)
        return Results.NotFound();

    return Results.Ok(result);
});

app.MapDelete("/customers/{id}", async (int id, ICustomerService service) =>
{
    var result = await service.DeleteAsync(id);

    if (result == null)
        return Results.NotFound();

    return Results.Ok(result);
});



app.MapPost("/orders", async (
    CreateOrderRequest request,
    IOrderService service) =>
{
    var result = await service.CreateOrderAsync(request);
    return Results.Ok(result);
});

app.MapPost("/orders/{id}/items", async (
    int id,
    AddOrderItemRequest request,
    IOrderService service) =>
{
    await service.AddItemAsync(id, request);
    return Results.Ok();
});

app.MapGet("/orders/{id}", async (
    int id,
    IOrderService service) =>
{
    var result = await service.GetOrderAsync(id);

    if (result == null)
        return Results.NotFound();

    return Results.Ok(result);
});



app.MapGet("/admins", async (IAdminService service) =>
{
    return Results.Ok(await service.GetAllAsync());
});

app.MapPost("/admins", async (CreateAdminRequest request, IAdminService service) =>
{
    return Results.Ok(await service.CreateAsync(request));
});

app.MapPut("/admins/{id}", async (int id, UpdateAdminRequest request, IAdminService service) =>
{
    var result = await service.UpdateAsync(id, request);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("/admins/{id}", async (int id, IAdminService service) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});



app.MapGet("/employees", async (IEmployeeService service) =>
{
    return Results.Ok(await service.GetAllAsync());
});

app.MapPost("/employees", async (CreateEmployeeRequest request, IEmployeeService service) =>
{
    return Results.Ok(await service.CreateAsync(request));
});

app.MapPut("/employees/{id}", async (int id, UpdateEmployeeRequest request, IEmployeeService service) =>
{
    var result = await service.UpdateAsync(id, request);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("/employees/{id}", async (int id, IEmployeeService service) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});

app.Run();
