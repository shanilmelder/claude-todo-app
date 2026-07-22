import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import type { Todo } from '../api/todoApi'
import { TodoList } from './TodoList'

const todos: Todo[] = [
  { id: '1', title: 'First', isCompleted: false, createdAt: new Date().toISOString() },
  { id: '2', title: 'Second', isCompleted: true, createdAt: new Date().toISOString() },
]

describe('TodoList', () => {
  it('renders a list item for each todo', () => {
    render(<TodoList todos={todos} onToggle={vi.fn()} onDelete={vi.fn()} onEdit={vi.fn()} />)

    expect(screen.getByText('First')).toBeInTheDocument()
    expect(screen.getByText('Second')).toBeInTheDocument()
  })

  it('shows an empty state when there are no todos', () => {
    render(<TodoList todos={[]} onToggle={vi.fn()} onDelete={vi.fn()} onEdit={vi.fn()} />)

    expect(screen.getByText(/no todos yet/i)).toBeInTheDocument()
  })
})
