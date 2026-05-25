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

  const removeItem = async (listId: string, itemId: string) => {
    return await api<void>(`/ShoppingLists/${listId}/items/${itemId}`, { method: 'DELETE' })
  }

  const updateItem = async (listId: string, itemId: string, dto: UpdateShoppingListItemDto) => {
    return await api<void>(`/ShoppingLists/${listId}/items/${itemId}`, { method: 'PUT', body: dto })
  }

  const updateItemsBatch = async (id: string, dto: UpdateBatchShoppingListItemsRequest) => {
    return await api<ShoppingListItemDto[]>(`/ShoppingLists/upsert-batch-items/${id}`, {
      method: 'PUT',
      body: dto,
    })
  }

  return {
    getAll,
    getById,
    create,
    updateList,
    updateItemsBatch,
    toggleDeleted,
    remove,
    removeItem,
    updateItem,
  }
}
