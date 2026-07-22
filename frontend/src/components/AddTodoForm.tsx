import { useState, type FormEvent } from 'react'

interface AddTodoFormProps {
  onAdd: (title: string) => void
}

export function AddTodoForm({ onAdd }: AddTodoFormProps) {
  const [title, setTitle] = useState('')

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const trimmed = title.trim()
    if (!trimmed) {
      return
    }
    onAdd(trimmed)
    setTitle('')
  }

  return (
    <form onSubmit={handleSubmit} className="add-todo-form">
      <label htmlFor="new-todo-input">New todo</label>
      <input
        id="new-todo-input"
        type="text"
        value={title}
        onChange={(event) => setTitle(event.target.value)}
        placeholder="What needs doing?"
      />
      <button type="submit">Add</button>
    </form>
  )
}
