import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import type { Todo } from '../api/todoApi'
import { TodoItem } from './TodoItem'

const todo: Todo = {
  id: '1',
  title: 'Buy milk',
  isCompleted: false,
  createdAt: new Date().toISOString(),
}

describe('TodoItem', () => {
  it('calls onToggle with the todo id when the checkbox is clicked', async () => {
    const onToggle = vi.fn()
    const user = userEvent.setup()
    render(<TodoItem todo={todo} onToggle={onToggle} onDelete={vi.fn()} onEdit={vi.fn()} />)

    await user.click(screen.getByRole('checkbox'))

    expect(onToggle).toHaveBeenCalledWith('1')
  })

  it('calls onDelete with the todo id when the delete button is clicked', async () => {
    const onDelete = vi.fn()
    const user = userEvent.setup()
    render(<TodoItem todo={todo} onToggle={vi.fn()} onDelete={onDelete} onEdit={vi.fn()} />)

    await user.click(screen.getByRole('button', { name: /delete/i }))

    expect(onDelete).toHaveBeenCalledWith('1')
  })

  it('renders a completed todo with a checked checkbox', () => {
    render(
      <TodoItem
        todo={{ ...todo, isCompleted: true }}
        onToggle={vi.fn()}
        onDelete={vi.fn()}
        onEdit={vi.fn()}
      />,
    )

    expect(screen.getByRole('checkbox')).toBeChecked()
  })

  it('lets the user edit the title', async () => {
    const onEdit = vi.fn()
    const user = userEvent.setup()
    render(<TodoItem todo={todo} onToggle={vi.fn()} onDelete={vi.fn()} onEdit={onEdit} />)

    await user.click(screen.getByRole('button', { name: /edit/i }))
    const input = screen.getByRole('textbox')
    await user.clear(input)
    await user.type(input, 'Buy oat milk{Enter}')

    expect(onEdit).toHaveBeenCalledWith('1', 'Buy oat milk')
  })
})
