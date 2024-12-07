export interface RecipeRequest {
    name: string;
    ingredients: Ingredient[];
    quantityPerBatch: number;
    instructions: string;
}

export interface Ingredient {
    ingredientName: string;
    quantity: number;
    unit: number;
}

export enum Unit {
    Kilogram,
    Gram,
    Liter,
    Milliliter,
    Piece
}