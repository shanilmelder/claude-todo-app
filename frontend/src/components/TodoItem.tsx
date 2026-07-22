import { useState, type FormEvent, type KeyboardEvent } from 'react'
import type { Todo } from '../api/todoApi'

interface TodoItemProps {
  todo: Todo
  onToggle: (id: string) => void
  onDelete: (id: string) => void
  onEdit: (id: string, title: string) => void
}

export function TodoItem({ todo, onToggle, onDelete, onEdit }: TodoItemProps) {
  const [isEditing, setIsEditing] = useState(false)
  const [draftTitle, setDraftTitle] = useState(todo.title)

  function startEditing() {
    setDraftTitle(todo.title)
    setIsEditing(true)
  }

  function commitEdit() {
    const trimmed = draftTitle.trim()
    if (trimmed && trimmed !== todo.title) {
      onEdit(todo.id, trimmed)
    }
    setIsEditing(false)
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    commitEdit()
  }

  function handleKeyDown(event: KeyboardEvent<HTMLInputElement>) {
    if (event.key === 'Escape') {
      setDraftTitle(todo.title)
      setIsEditing(false)
    }
  }

  return (
    <li className={`todo-item${todo.isCompleted ? ' completed' : ''}`}>
      <input
        type="checkbox"
        checked={todo.isCompleted}
        onChange={() => onToggle(todo.id)}
        aria-label={`Mark "${todo.title}" as ${todo.isCompleted ? 'incomplete' : 'complete'}`}
      />
      <span className="todo-item-main">
        {isEditing ? (
          <form onSubmit={handleSubmit} className="todo-item-edit-form">
            <input
              type="text"
              value={draftTitle}
              onChange={(event) => setDraftTitle(event.target.value)}
              onKeyDown={handleKeyDown}
              onBlur={commitEdit}
              autoFocus
            />
          </form>
        ) : (
          <span className="todo-item-title">{todo.title}</span>
        )}
        {todo.isCompleted && (
          <span className="stamp" aria-hidden="true">
            Done
          </span>
        )}
      </span>
      <span className="todo-item-actions">
        <button type="button" onClick={startEditing}>
          Edit
        </button>
        <button type="button" onClick={() => onDelete(todo.id)}>
          Delete
        </button>
      </span>
    </li>
  )
}
