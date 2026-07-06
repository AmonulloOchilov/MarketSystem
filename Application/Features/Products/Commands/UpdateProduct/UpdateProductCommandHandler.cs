using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IAppDbContext dbContext, ILogger<UpdateProductCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", request.ProductId);
        
        var name = request.Request.Name.Trim().ToLower();

        var exists = await _dbContext.Products
            .AnyAsync(p =>
                p.Id != request.ProductId &&
                p.Name.ToLower() == name, cancellationToken);

        if (exists)
        {
            _logger.LogWarning("Product already exists with name: {ProductName}", name);
            throw new ProductAlreadyExistsException();
        }
        
        var product = await _dbContext.Products.FirstOrDefaultAsync(p=>p.Id == request.ProductId, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with {ProductId} not found", request.ProductId);
            throw new ProductNotFoundException(request.ProductId);
        }

        product.Name = name;
        product.Price = request.Request.Price;
        product.Stock = request.Request.Stock;
        product.CategoryId = request.Request.CategoryId;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Product {ProductId} updated successfully. Name: {Name}, Price: {Price}",
            product.Id, product.Name, product.Price);
        
        return new ProductResponse()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId
        };
    }
}