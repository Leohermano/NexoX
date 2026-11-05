
using Microsoft.EntityFrameworkCore;
using NexoX.API.Data;
using NexoX.API.Models;
using NexoX.API.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(
            "http://127.0.0.1:5500",
            "http://localhost:5500"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddDbContext<NexoXContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

app.UseCors("Frontend");

app.MapGet("/", () => Results.Ok(new
{
    projeto = "NexoX",
    mensagem = "API funcionando!",
    versao = "0.3.0"
}));

// TESTE DE CONEXÃO
app.MapGet("/api/db-teste", async (NexoXContext db) =>
{
    var conectado = await db.Database.CanConnectAsync();

    return conectado
        ? Results.Ok(new { status = "online" })
        : Results.Problem("Banco indisponível");
});

// GRUPO DE ROTAS DE TAREFAS
var tarefas = app.MapGroup("/api/tarefas");

// READ - Listar tarefas
tarefas.MapGet("", async (
    int usuarioId,
    NexoXContext db) =>
{
    var lista = await db.Tarefas
        .AsNoTracking()
        .Where(t => t.UsuarioId == usuarioId)
        .OrderBy(t => t.Concluida)
        .ThenBy(t => t.DataVencimento)
        .ToListAsync();

    return Results.Ok(lista);
});

// READ - Buscar tarefa por ID
tarefas.MapGet("/{id:int}", async (
    int id,
    int usuarioId,
    NexoXContext db) =>
{
    var tarefa = await db.Tarefas
        .AsNoTracking()
        .FirstOrDefaultAsync(t =>
            t.TarefaId == id &&
            t.UsuarioId == usuarioId);

    return tarefa is null
        ? Results.NotFound(new { mensagem = "Tarefa não encontrada." })
        : Results.Ok(tarefa);
});

// CREATE - Cadastrar tarefa
tarefas.MapPost("", async (
    CriarTarefaRequest request,
    NexoXContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Titulo))
        return Results.BadRequest(new
        {
            mensagem = "O título é obrigatório."
        });

    if (request.Titulo.Length > 120)
        return Results.BadRequest(new
        {
            mensagem = "O título deve ter no máximo 120 caracteres."
        });

    if (request.Prioridade < 1 || request.Prioridade > 3)
        return Results.BadRequest(new
        {
            mensagem = "Prioridade deve ser 1, 2 ou 3."
        });

    var usuarioExiste = await db.Usuarios
        .AnyAsync(u => u.Id == request.UsuarioId);

    if (!usuarioExiste)
        return Results.BadRequest(new
        {
            mensagem = "Usuário não encontrado."
        });

    if (request.CategoriaId.HasValue)
    {
        var categoriaExiste = await db.Categorias.AnyAsync(c =>
            c.CategoriaId == request.CategoriaId &&
            c.UsuarioId == request.UsuarioId);

        if (!categoriaExiste)
            return Results.BadRequest(new
            {
                mensagem = "Categoria inválida."
            });
    }

    var tarefa = new Tarefa
    {
        UsuarioId = request.UsuarioId,
        CategoriaId = request.CategoriaId,
        Titulo = request.Titulo.Trim(),
        Descricao = request.Descricao,
        DataVencimento = request.DataVencimento,
        Prioridade = request.Prioridade,
        Concluida = false,
        DataCriacao = DateTime.UtcNow
    };

    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/tarefas/{tarefa.TarefaId}?usuarioId={tarefa.UsuarioId}",
        tarefa
    );
});

// UPDATE - Atualizar tarefa
tarefas.MapPut("/{id:int}", async (
    int id,
    AtualizarTarefaRequest request,
    NexoXContext db) =>
{
    var tarefa = await db.Tarefas.FirstOrDefaultAsync(t =>
        t.TarefaId == id &&
        t.UsuarioId == request.UsuarioId);

    if (tarefa is null)
        return Results.NotFound(new
        {
            mensagem = "Tarefa não encontrada."
        });

    if (string.IsNullOrWhiteSpace(request.Titulo) ||
        request.Titulo.Length > 120)
        return Results.BadRequest(new
        {
            mensagem = "Informe um título válido."
        });

    if (request.Prioridade < 1 || request.Prioridade > 3)
        return Results.BadRequest(new
        {
            mensagem = "Prioridade deve ser 1, 2 ou 3."
        });

    if (request.CategoriaId.HasValue)
    {
        var categoriaExiste = await db.Categorias.AnyAsync(c =>
            c.CategoriaId == request.CategoriaId &&
            c.UsuarioId == request.UsuarioId);

        if (!categoriaExiste)
            return Results.BadRequest(new
            {
                mensagem = "Categoria inválida."
            });
    }

    tarefa.Titulo = request.Titulo.Trim();
    tarefa.Descricao = request.Descricao;
    tarefa.DataVencimento = request.DataVencimento;
    tarefa.Prioridade = request.Prioridade;
    tarefa.Concluida = request.Concluida;
    tarefa.CategoriaId = request.CategoriaId;
    tarefa.DataAtualizacao = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(tarefa);
});

// DELETE - Excluir tarefa
tarefas.MapDelete("/{id:int}", async (
    int id,
    int usuarioId,
    NexoXContext db) =>
{
    var tarefa = await db.Tarefas.FirstOrDefaultAsync(t =>
        t.TarefaId == id &&
        t.UsuarioId == usuarioId);

    if (tarefa is null)
        return Results.NotFound(new
        {
            mensagem = "Tarefa não encontrada."
        });

    db.Tarefas.Remove(tarefa);

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = "Tarefa excluída com sucesso!"
    });
});

app.Run();
