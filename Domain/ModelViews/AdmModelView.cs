using minimals_api.Domain.Enums;

namespace minimals_api.Domain.ModelViews;

public record AdmModelView
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;
}