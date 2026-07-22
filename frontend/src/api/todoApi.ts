export interface Todo {
  id: string
  title: string
  isCompleted: boolean
  createdAt: string
}

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5224'

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`)
  }
  if (response.status === 204) {
    return undefined as T
  }
  return (await response.json()) as T
}

export async function fetchTodos(): Promise<Todo[]> {
  const response = await fetch(`${API_BASE}/api/todos`)
  return handleResponse<Todo[]>(response)
}

export async function createTodo(title: string): Promise<Todo> {
  const response = await fetch(`${API_BASE}/api/todos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title }),
  })
  return handleResponse<Todo>(response)
}

export async function updateTodoTitle(id: string, title: string): Promise<void> {
  const response = await fetch(`${API_BASE}/api/todos/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title }),
  })
  await handleResponse<void>(response)
}

export async function setTodoCompleted(id: string, isCompleted: boolean): Promise<void> {
  const response = await fetch(`${API_BASE}/api/todos/${id}/complete`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ isCompleted }),
  })
  await handleResponse<void>(response)
}

export async function deleteTodo(id: string): Promise<void> {
  const response = await fetch(`${API_BASE}/api/todos/${id}`, { method: 'DELETE' })
  await handleResponse<void>(response)
}
