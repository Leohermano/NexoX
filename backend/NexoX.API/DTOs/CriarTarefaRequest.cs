namespace NexoX.API.DTOs;

public record CriarTarefaRequest(
    int UsuarioId,
    string Titulo,
    string? Descricao,
    DateTime? DataVencimento,
    byte Prioridade = 2,
    int? CategoriaId = null
);