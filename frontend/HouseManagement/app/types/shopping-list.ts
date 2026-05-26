export interface ShoppingList {
  id: string
  name: string
  notes?: string
  isCompleted: boolean
  items: ShoppingListItem[]
}

export interface ShoppingListItem {
  id: string
  name: string
  quantity: number
  isCompleted: boolean
}

export interface CreateShoppingListDto {
  name: string
}

export interface UpdateShoppingListDto {
  name: string
  notes?: string
  isCompleted: boolean
}

export interface CreateShoppingItemDto {
  name: string
  quantity: number
  unit: string
  shoppingListId: string
}

export interface UpdateShoppingListItemDto {
  name: string
  quantity: number
  notes?: string
  isCompleted: boolean
}

export interface AddSuggestionRequest {
  name: string
}

export interface SearchShoppingSuggestion {
  term: string
  page: number
  limit: number
}

export interface ShoppingSuggestion {
  id: string
  name: string
  usageCount: number
}

export interface UpdateBatchShoppingListItemsRequest {
  itemsToAdd: {
    id: string
    name: string
    quantity: number
  }[]
  itemsToUpdate: {
    id: string
    quantity?: number
    isCompleted?: boolean
  }[]
  itemsToRemove: {
    id: string
  }[]
}
