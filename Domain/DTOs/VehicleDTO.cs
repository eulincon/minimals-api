using System.ComponentModel.DataAnnotations;

namespace minimals_api.Domain.DTOs;

#region vehicle
public class VehicleDTO : IValidatableObject
{
    [Required(ErrorMessage = "Nome is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome must be between 2 and 100 characters")]
    public string Nome { get; set; } = default!;

    [Required(ErrorMessage = "Marca is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Marca must be between 2 and 100 characters")]
    public string Marca { get; set; } = default!;

    [Required(ErrorMessage = "Ano is required")]
    [Range(1900, 2100, ErrorMessage = "Ano must be between 1900 and 2100")]
    public int Ano { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Additional custom validation logic can be added here
        if (string.IsNullOrWhiteSpace(Nome))
        {
            yield return new ValidationResult("Nome cannot be empty or whitespace", new[] { nameof(Nome) });
        }
        
        if (string.IsNullOrWhiteSpace(Marca))
        {
            yield return new ValidationResult("Marca cannot be empty or whitespace", new[] { nameof(Marca) });
        }
    }
}
#endregion