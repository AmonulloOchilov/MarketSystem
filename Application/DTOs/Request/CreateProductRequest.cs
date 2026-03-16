using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request;

public class CreateProductRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}