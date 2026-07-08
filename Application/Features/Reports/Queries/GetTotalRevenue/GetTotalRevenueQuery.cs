using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Reports.Queries.GetTotalRevenue;

public record GetTotalRevenueQuery() : IRequest<TotalRevenueResponse>
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}