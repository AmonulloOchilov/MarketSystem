using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Reports.Queries.GetEmployeePerformance;

public record GetEmployeePerformanceQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<EmployeePerformanceResponse>>;