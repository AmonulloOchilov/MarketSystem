using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request;

public class CreateCategoryRequest
{
    public string Name { get; set; } = null!;
}