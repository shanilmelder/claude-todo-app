import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'
import type { Todo } from './api/todoApi'

function jsonResponse(body: unknown, status = 200) {
  return new Response(body === undefined ? null : JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  })
}

describe('App', () => {
  let todos: Todo[]

  beforeEach(() => {
    todos = [{ id: '1', title: 'Existing', isCompleted: false, createdAt: new Date().toISOString() }]

    vi.stubGlobal(
      'fetch',
      vi.fn(async (input: RequestInfo | URL, init?: RequestInit) => {
        const url = typeof input === 'string' ? input : input.toString()
        const method = init?.method ?? 'GET'

        if (url.endsWith('/api/todos') && method === 'GET') {
          return jsonResponse(todos)
        }
        if (url.endsWith('/api/todos') && method === 'POST') {
          const { title } = JSON.parse(init!.body as string)
          const created: Todo = { id: '2', title, isCompleted: false, createdAt: new Date().toISOString() }
          todos = [...todos, created]
          return jsonResponse(created, 201)
        }
        if (url.includes('/complete') && method === 'PATCH') {
          return jsonResponse(undefined, 204)
        }
        if (method === 'DELETE') {
          return jsonResponse(undefined, 204)
        }

        throw new Error(`Unhandled request: ${method} ${url}`)
      }),
    )
  })

  afterEach(() => {
    vi.unstubAllGlobals()
  })

  it('loads and displays todos on mount', async () => {
    render(<App />)

    expect(await screen.findByText('Existing')).toBeInTheDocument()
  })

  it('adds a new todo', async () => {
    const user = userEvent.setup()
    render(<App />)
    await screen.findByText('Existing')

    await user.type(screen.getByLabelText(/new todo/i), 'New task')
    await user.click(screen.getByRole('button', { name: /add/i }))

    expect(await screen.findByText('New task')).toBeInTheDocument()
  })

  it('toggles a todo as complete', async () => {
    const user = userEvent.setup()
    render(<App />)
    await screen.findByText('Existing')

    await user.click(screen.getByRole('checkbox'))

    await waitFor(() => expect(screen.getByRole('checkbox')).toBeChecked())
  })

  it('deletes a todo', async () => {
    const user = userEvent.setup()
    render(<App />)
    await screen.findByText('Existing')

    await user.click(screen.getByRole('button', { name: /delete/i }))

    await waitFor(() => expect(screen.queryByText('Existing')).not.toBeInTheDocument())
  })
})
