# NexoX — Organizador de Rotina

Projeto inicial de um organizador de rotina com front-end em HTML/CSS/JavaScript e API em C# com ASP.NET Core.

## Requisitos
- .NET 8 SDK ou superior
- Visual Studio Code ou Visual Studio
- Extensão Live Server (VS Code), para servir o front-end

## Iniciar a API
No terminal, entre na pasta `backend/NexoX.API` e execute:

```bash
dotnet run --urls http://localhost:5000
```

Teste no navegador:
- http://localhost:5000/
- http://localhost:5000/api/saude

## Iniciar o front-end
Abra `frontend/index.html` com a extensão Live Server e acesse o endereço informado pelo VS Code.

## Estado atual
Esta é a primeira versão de interface e conexão de teste. As tarefas ainda ficam somente na memória do navegador. Banco de dados, cadastro/login, isolamento de dados, API de tarefas, publicação e pagamentos serão implementados nas próximas etapas.
