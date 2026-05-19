import type {
  CreateShoppingListDto,
  UpdateShoppingListDto,
  ShoppingList,
} from '@home-management/types'
import { useApi } from './apiService'

export const useShoppingListService = () => {
  const api = useApi()

  const getAll = async (isDeleted: boolean = false) => {
    const query = new URLSearchParams()

    if (isDeleted !== undefined) {
      query.append('isDeleted', String(isDeleted))
    }

    const url = query.toString() ? `/ShoppingLists?${query.toString()}` : '/ShoppingLists'

    return await api<ShoppingList[]>(url)
  }

  const getById = async (id: string) => {
    return await api<ShoppingList>(`/ShoppingLists/${id}`)
  }

  const create = async (dto: CreateShoppingListDto) => {
    return await api<ShoppingList>('/ShoppingLists', { method: 'POST', body: dto })
  }

  const updateList = async (id: string, dto: UpdateShoppingListDto) => {
    return await api<ShoppingList>(`/ShoppingLists/${id}`, { method: 'PUT', body: dto })
  }

  const toggleDeleted = async (id: string) => {
    return await api<void>(`/ShoppingLists/toggledeleted/${id}`, { method: 'DELETE' })
  }

  const remove = async (id: string) => {
    return await api<void>(`/ShoppingLists/${id}`, { method: 'DELETE' })
  }

  return {
    getAll,
    getById,
    create,
    updateList,
    toggleDeleted,
    remove,
  }
}
