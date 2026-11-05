using System.ComponentModel.DataAnnotations;

namespace NexoX.API.Models;

public class Tarefa
{
    public int TarefaId { get; set; }

    public int UsuarioId { get; set; }

    public int? CategoriaId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    public DateTime? DataVencimento { get; set; }

    public byte Prioridade { get; set; }

    public bool Concluida { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }
}