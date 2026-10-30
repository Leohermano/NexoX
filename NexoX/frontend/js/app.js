const form = document.querySelector("#task-form");
const input = document.querySelector("#task-input");
const list = document.querySelector("#task-list");
const total = document.querySelector("#total");
const done = document.querySelector("#done");
const pending = document.querySelector("#pending");
const tasks = [];

function renderTasks() {
  list.innerHTML = "";
  if (tasks.length === 0) {
    list.innerHTML = '<li class="empty">Nenhuma tarefa por enquanto. Adicione uma acima!</li>';
  }
  tasks.forEach((task, index) => {
    const li = document.createElement("li");
    if (task.completed) li.classList.add("completed");
    const checkbox = document.createElement("input");
    checkbox.type = "checkbox";
    checkbox.checked = task.completed;
    checkbox.setAttribute("aria-label", "Concluir tarefa");
    checkbox.addEventListener("change", () => {
      tasks[index].completed = checkbox.checked;
      renderTasks();
    });
    const label = document.createElement("span");
    label.textContent = task.title;
    li.append(checkbox, label);
    list.append(li);
  });
  const completed = tasks.filter(task => task.completed).length;
  total.textContent = tasks.length;
  done.textContent = completed;
  pending.textContent = tasks.length - completed;
}

form.addEventListener("submit", event => {
  event.preventDefault();
  const title = input.value.trim();
  if (!title) return;
  tasks.push({ title, completed: false });
  input.value = "";
  renderTasks();
});

const now = new Date();
document.querySelector("#today").textContent = new Intl.DateTimeFormat("pt-BR", {
  weekday: "long", day: "numeric", month: "long"
}).format(now);

fetch("http://localhost:5000/api/saude")
  .then(response => {
    if (!response.ok) throw new Error("API indisponível");
    return response.json();
  })
  .then(data => {
    document.querySelector("#api-message").textContent = `API conectada • ${data.status}`;
    document.querySelector(".status-dot").style.background = "#20c6b4";
  })
  .catch(() => {
    document.querySelector("#api-message").textContent = "API ainda não conectada. Inicie o back-end.";
  });

renderTasks();
