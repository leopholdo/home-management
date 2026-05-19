export interface Snack {
  show: boolean
  message: string
  error: boolean
  timeout: number
}

export const useSnack = () => {
  const snackQueue = useState<Snack[]>('snackQueue', () => [])

  const activeSnack = useState<Snack>('activeSnack', () => ({
    show: false,
    message: '',
    error: false,
    timeout: 2000,
  }))

  function processQueue() {
    if (activeSnack.value.show) return

    const nextSnack = snackQueue.value.shift()

    if (!nextSnack) return

    activeSnack.value = nextSnack
  }

  function show(
    message: string,
    options?: {
      error?: boolean
      timeout?: number
      color?: string
    }
  ) {
    const snack: Snack = {
      show: true,
      message,
      error: options?.error ?? false,
      timeout: options?.timeout ?? 2000,
      color: options?.color ?? '',
    }

    snackQueue.value.push(snack)

    processQueue()
  }

  function success(message: string, timeout = 2000) {
    show(message, {
      error: false,
      timeout,
      color: 'success',
    })
  }

  function error(message: string, timeout = 3000) {
    show(message, {
      error: true,
      timeout,
      color: 'error',
    })
  }

  function close() {
    activeSnack.value.show = false

    nextTick(() => {
      processQueue()
    })
  }

  return {
    activeSnack,
    show,
    success,
    error,
    close,
  }
}
