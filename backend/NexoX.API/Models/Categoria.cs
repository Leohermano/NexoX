using System.ComponentModel.DataAnnotations;

namespace NexoX.API.Models;

public class Categoria
{
    public int CategoriaId { get; set; }

    public int UsuarioId { get; set; }

    [Required]
    [MaxLength(60)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(7)]
    public string? Cor { get; set; }
}