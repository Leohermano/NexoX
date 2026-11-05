using System.ComponentModel.DataAnnotations;

namespace NexoX.API.Models;

public class Compromisso
{
    public int CompromissoId { get; set; }

    public int UsuarioId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    [MaxLength(180)]
    public string? Local { get; set; }

    public DateTime DataCriacao { get; set; }
}