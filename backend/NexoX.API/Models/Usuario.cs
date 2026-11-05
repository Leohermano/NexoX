using System.ComponentModel.DataAnnotations;

namespace NexoX.API.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; }

    public bool Ativo { get; set; }
}