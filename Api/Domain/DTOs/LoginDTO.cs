using System.ComponentModel.DataAnnotations;

namespace minimals_api.Domain.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Senha is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Senha must be between 3 and 100 characters")]
    public string Senha { get; set; } = default!;
}