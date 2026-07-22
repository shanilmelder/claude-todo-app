import { useEffect, useState } from 'react'
import './App.css'
import {
  createTodo,
  deleteTodo,
  fetchTodos,
  setTodoCompleted,
  updateTodoTitle,
  type Todo,
} from './api/todoApi'
import { AddTodoForm } from './components/AddTodoForm'
import { TodoList } from './components/TodoList'

function App() {
  const [todos, setTodos] = useState<Todo[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false

    fetchTodos()
      .then((data) => {
        if (!cancelled) setTodos(data)
      })
      .catch(() => {
        if (!cancelled) setError("Couldn't load your list. Try refreshing.")
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [])

  async function handleAdd(title: string) {
    try {
      const created = await createTodo(title)
      setTodos((current) => [...current, created])
    } catch {
      setError("Couldn't add that task. Try again.")
    }
  }

  async function handleToggle(id: string) {
    const target = todos.find((todo) => todo.id === id)
    if (!target) return

    const nextCompleted = !target.isCompleted
    setTodos((current) =>
      current.map((todo) => (todo.id === id ? { ...todo, isCompleted: nextCompleted } : todo)),
    )

    try {
      await setTodoCompleted(id, nextCompleted)
    } catch {
      setTodos((current) =>
        current.map((todo) => (todo.id === id ? { ...todo, isCompleted: !nextCompleted } : todo)),
      )
      setError("Couldn't update that task. Try again.")
    }
  }

  async function handleEdit(id: string, title: string) {
    const previous = todos
    setTodos((current) => current.map((todo) => (todo.id === id ? { ...todo, title } : todo)))

    try {
      await updateTodoTitle(id, title)
    } catch {
      setTodos(previous)
      setError("Couldn't rename that task. Try again.")
    }
  }

  async function handleDelete(id: string) {
    const previous = todos
    setTodos((current) => current.filter((todo) => todo.id !== id))

    try {
      await deleteTodo(id)
    } catch {
      setTodos(previous)
      setError("Couldn't delete that task. Try again.")
    }
  }

  const openCount = todos.filter((todo) => !todo.isCompleted).length
  const doneCount = todos.length - openCount

  return (
    <main className="page">
      <div className="ticket">
        <header className="ticket-header">
          <p className="eyebrow">Field checklist</p>
          <h1>Task Log</h1>
        </header>

        <AddTodoForm onAdd={handleAdd} />

        {error && (
          <p className="error-banner" role="alert">
            {error}
          </p>
        )}

        {isLoading ? (
          <p className="status-line">Loading your list…</p>
        ) : (
          <TodoList todos={todos} onToggle={handleToggle} onDelete={handleDelete} onEdit={handleEdit} />
        )}

        <footer className="tally">
          <span>{openCount} open</span>
          <span aria-hidden="true">·</span>
          <span>{doneCount} done</span>
        </footer>
      </div>
    </main>
  )
}

export default App
