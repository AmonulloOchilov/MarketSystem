using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductResponse>;