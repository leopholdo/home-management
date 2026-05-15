import type { CreateShoppingListDto, ShoppingList } from '@home-management/types'
import { useApi } from './apiService'

export const useShoppingListService = () => {
  const api = useApi()

  const getAll = async () => {
    return await api<ShoppingList[]>('/ShoppingLists')
  }

  const getById = async (id: string) => {
    return await api<ShoppingList>(`/ShoppingLists/${id}`)
  }

  const create = async (dto: CreateShoppingListDto) => {
    return await api<ShoppingList>('/ShoppingLists', { method: 'POST', body: dto })
  }

  const remove = async (id: string) => {
    return await api<void>(`/ShoppingLists/${id}`, { method: 'DELETE' })
  }

  return {
    getAll,
    getById,
    create,
    remove
  }
}