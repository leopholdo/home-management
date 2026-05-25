import type {
  CreateShoppingListDto,
  UpdateShoppingListDto,
  ShoppingList,
  AddSuggestionRequest,
} from '@home-management/types'
import { useApi } from './apiService'

export const useShoppingSuggestionService = () => {
  const api = useApi()

  const get = async (dto: SearchShoppingSuggestion) => {
    const query = new URLSearchParams()
    if (dto.term) {
      query.append('term', dto.term)
    }

    if (dto.limit) {
      query.append('limit', String(dto.limit))
    }

    const url = query.toString()
      ? `/ShoppingSuggestions?${query.toString()}`
      : '/ShoppingSuggestions'

    return await api<ShoppingList[]>(url)
  }

  const add = async (dto: AddSuggestionRequest) => {
    return await api<ShoppingList>('/ShoppingSuggestions/addSuggestion', {
      method: 'POST',
      body: dto,
    })
  }

  return {
    get,
    add,
  }
}
