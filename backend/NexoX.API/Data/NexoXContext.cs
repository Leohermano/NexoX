using Microsoft.EntityFrameworkCore;
using NexoX.API.Models;

namespace NexoX.API.Data;

public class NexoXContext : DbContext
{
    public NexoXContext(DbContextOptions<NexoXContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Compromisso> Compromissos => Set<Compromisso>();
}