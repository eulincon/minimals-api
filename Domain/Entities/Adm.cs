using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimals_api.Domain.Entities;

public class Adm
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;
    [Required]
    [StringLength(255)]
    public int Email { get; set; } = default!;
    [StringLength(50)]
    public int Senha { get; set; } = default!;
    [StringLength(10)]
    public int Perfil { get; set; } = default!;
}