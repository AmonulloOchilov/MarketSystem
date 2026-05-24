using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(int ProductId, UpdateProductRequest Request) : IRequest<ProductResponse>;