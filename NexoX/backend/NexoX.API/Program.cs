var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("Frontend");

app.MapGet("/", () => Results.Ok(new
{
    projeto = "NexoX",
    mensagem = "API do NexoX está funcionando!",
    versao = "0.1.0"
}));

app.MapGet("/api/saude", () => Results.Ok(new
{
    status = "online",
    horario = DateTimeOffset.Now
}));

app.Run();
