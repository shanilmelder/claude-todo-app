import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { AddTodoForm } from './AddTodoForm'

describe('AddTodoForm', () => {
  it('calls onAdd with the trimmed title and clears the input on submit', async () => {
    const onAdd = vi.fn()
    const user = userEvent.setup()
    render(<AddTodoForm onAdd={onAdd} />)

    const input = screen.getByLabelText(/new todo/i)
    await user.type(input, '  Buy milk  ')
    await user.click(screen.getByRole('button', { name: /add/i }))

    expect(onAdd).toHaveBeenCalledWith('Buy milk')
    expect(input).toHaveValue('')
  })

  it('does not call onAdd when the input is blank', async () => {
    const onAdd = vi.fn()
    const user = userEvent.setup()
    render(<AddTodoForm onAdd={onAdd} />)

    await user.click(screen.getByRole('button', { name: /add/i }))

    expect(onAdd).not.toHaveBeenCalled()
  })
})
