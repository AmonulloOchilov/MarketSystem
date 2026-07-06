using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(int ProductId) : IRequest<ProductResponse>;