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

export interface CreateShoppingItemDto {
  name: string
  quantity: number
  unit: string
  shoppingListId: string
}