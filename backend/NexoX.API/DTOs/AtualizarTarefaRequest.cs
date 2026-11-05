namespace NexoX.API.DTOs;

public record AtualizarTarefaRequest(
    int UsuarioId,
    string Titulo,
    string? Descricao,
    DateTime? DataVencimento,
    byte Prioridade,
    bool Concluida,
    int? CategoriaId
);