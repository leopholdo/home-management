type ApiSuccess<T> = {
  success: true
  data: T
}

type ApiError = {
  success: false
  status: number
  message: string
  errors?: unknown
}

type ApiResponse<T> = ApiSuccess<T> | ApiError

export const useApi = () => {
  const config = useRuntimeConfig()

  return async <T>(
    url: string,
    options?: RequestInit & { body?: unknown }
  ): Promise<ApiResponse<T>> => {
    try {
      const response = await fetch(`${config.public.apiBase}${url}`, {
        headers: {
          'Content-Type': 'application/json',
        },
        ...options,
        body: options?.body ? JSON.stringify(options.body) : undefined,
      })

      const data = await response.json().catch(() => null)

      if (!response.ok) {
        return {
          success: false,
          status: response.status,
          message: data?.message || 'Unexpected error',
          errors: data?.errors || null,
        }
      }

      return {
        success: true,
        data: data as T,
      }
    } catch (error) {
      return {
        success: false,
        status: 500,
        message: 'Network error',
        errors: error,
      }
    }
  }
}
