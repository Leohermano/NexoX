
const API_URL = "http://localhost:5000/api";
const USER_ID = 1; // ID do usuário no SQL Server

const form = document.querySelector("#task-form");
const input = document.querySelector("#task-input");
const list = document.querySelector("#task-list");
const total = document.querySelector("#total");
const done = document.querySelector("#done");
const pending = document.querySelector("#pending");

const apiMessage = document.querySelector("#api-message");
const statusDot = document.querySelector(".status-dot");

let tasks = [];

// Comunicação com a API
async function apiRequest(url, options = {}) {
    const response = await fetch(`${API_URL}${url}`, options);

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message || `Erro HTTP ${response.status}`);
    }

    if (response.status === 204) return null;

    const contentType = response.headers.get("content-type");

    return contentType?.includes("application/json")
        ? response.json()
        : null;
}

// Verificar conexão com a API
async function checkAPI() {
    try {
        const data = await apiRequest("/db-teste");

        apiMessage.textContent = `API conectada • ${data.status}`;
        statusDot.style.background = "#20c6b4";
    } catch (error) {
        apiMessage.textContent = "Erro ao conectar com a API.";
        statusDot.style.background = "#e05252";
        console.error(error);
    }
}

// Buscar tarefas no SQL Server
async function loadTasks() {
    try {
        tasks = await apiRequest(`/tarefas?usuarioId=${USER_ID}`);
        renderTasks();
    } catch (error) {
        console.error("Erro ao carregar tarefas:", error);
        list.innerHTML =
            '<li class="empty">Não foi possível carregar as tarefas.</li>';
    }
}

// Atualizar os indicadores e a lista
function renderTasks() {
    list.innerHTML = "";

    if (tasks.length === 0) {
        list.innerHTML =
            '<li class="empty">Nenhuma tarefa por enquanto. Adicione uma acima!</li>';
    }

    tasks.forEach(task => {
        const li = document.createElement("li");

        if (task.concluida) {
            li.classList.add("completed");
        }

        // Checkbox de conclusão
        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.checked = task.concluida;
        checkbox.setAttribute("aria-label", "Concluir tarefa");

        checkbox.addEventListener("change", async () => {
            checkbox.disabled = true;

            try {
                await apiRequest(`/tarefas/${task.tarefaId}`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        usuarioId: USER_ID,
                        titulo: task.titulo,
                        descricao: task.descricao,
                        dataVencimento: task.dataVencimento,
                        prioridade: task.prioridade,
                        concluida: checkbox.checked,
                        categoriaId: task.categoriaId
                    })
                });

                await loadTasks();
            } catch (error) {
                console.error("Erro ao atualizar tarefa:", error);
                alert("Não foi possível atualizar a tarefa.");
                checkbox.checked = task.concluida;
                checkbox.disabled = false;
            }
        });

        // Título
        const label = document.createElement("span");
        label.textContent = task.titulo;

        // Botão de exclusão
        const deleteButton = document.createElement("button");
        deleteButton.type = "button";
        deleteButton.textContent = "Excluir";
        deleteButton.className = "task-delete";

        deleteButton.addEventListener("click", async () => {
            if (!confirm("Deseja realmente excluir esta tarefa?")) return;

            try {
                await apiRequest(
                    `/tarefas/${task.tarefaId}?usuarioId=${USER_ID}`,
                    { method: "DELETE" }
                );

                await loadTasks();
            } catch (error) {
                console.error("Erro ao excluir tarefa:", error);
                alert("Não foi possível excluir a tarefa.");
            }
        });

        li.append(checkbox, label, deleteButton);
        list.append(li);
    });

    const completed = tasks.filter(task => task.concluida).length;

    total.textContent = tasks.length;
    done.textContent = completed;
    pending.textContent = tasks.length - completed;
}

// Cadastrar tarefa
form.addEventListener("submit", async event => {
    event.preventDefault();

    const title = input.value.trim();

    if (!title) return;

    const submitButton = form.querySelector('button[type="submit"]');
    submitButton.disabled = true;

    try {
        await apiRequest("/tarefas", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                usuarioId: USER_ID,
                titulo: title,
                descricao: "",
                prioridade: 1
            })
        });

        input.value = "";
        await loadTasks();

    } catch (error) {
        console.error("Erro ao cadastrar tarefa:", error);
        alert("Não foi possível cadastrar a tarefa.");
    } finally {
        submitButton.disabled = false;
    }
});

// Data atual
const now = new Date();

document.querySelector("#today").textContent =
    new Intl.DateTimeFormat("pt-BR", {
        weekday: "long",
        day: "numeric",
        month: "long"
    }).format(now);

// Inicialização
async function initializeApp() {
    await checkAPI();
    await loadTasks();
}

initializeApp();
