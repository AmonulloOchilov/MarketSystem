using Application.Exceptions;
using Application.Features.Products.Commands.UpdateProduct;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IAppDbContext dbContext, ILogger<DeleteProductCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", request.ProductId);
             
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.ProductId);
            throw new ProductNotFoundException(request.ProductId);
        }
        
        var isUsed = await _dbContext.OrderItems.AnyAsync(oi => oi.ProductId == request.ProductId, cancellationToken);
        
        if (isUsed)
        {
            throw new ProductInUseException(request.ProductId);
        }
        
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Product deleted successfully with ID: {ProductId}", request.ProductId);

        return true;
    }
}