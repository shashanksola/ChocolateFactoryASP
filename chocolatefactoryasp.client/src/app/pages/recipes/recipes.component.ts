import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Notyf } from 'notyf';
import { RecipeRequest, Ingredient, Unit } from '../../models/recipe.model';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [HttpClientModule, ReactiveFormsModule, NgIf, NgFor],
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css'
})


export class RecipesComponent implements OnInit {
  recipeForm!: FormGroup;
  recipes: RecipeRequest[] = [];
  token = "";
  notyf = new Notyf();
  headers = {}
  url = 'https://chocolatefactoryaspserver20250118211324.azurewebsites.net/api/Recipe';
  units = ['Kilogram',
    'Gram',
    'Liter',
    'Milliliter',
    'Piece']
  role = '';
  constructor(private http: HttpClient, private fb: FormBuilder) { }


  ngOnInit(): void {
    this.token = localStorage.getItem('token') || "none";
    this.headers = {
      Authorization: `Bearer ${this.token}`
    };
    this.getAllRecipesAsync();

    this.recipeForm = this.fb.group({
      name: ['', Validators.required],
      quantityPerBatch: [1, [Validators.required, Validators.min(1)]],
      instructions: ['', [Validators.required, Validators.maxLength(1000)]],
      ingredients: this.fb.array([
        this.createIngredientGroup() // Initialize with one ingredient
      ])
    });

    this.role = localStorage.getItem('role') || '';
  }

  getAllRecipesAsync(): void {
    this.http.get<RecipeRequest[]>(this.url, { headers: this.headers }).subscribe({
      next: (response) => {
        // console.log('Response:', response);
        this.recipes = response;

        for (let recipe of this.recipes) {
          console.log(recipe.ingredients[0]);
        }
      },
      error: (error) => {
        console.error('Error fetching recipes:', error);

        const { status } = error;
        this.notyf.error(`Http Error ${status}`);
      }
    });
  }

  addRecipe(newRecipe: RecipeRequest): void {
    this.http.post(this.url, newRecipe, { headers: this.headers }).subscribe({
      next: (recipe) => {
        this.notyf.success(`Recipe added successfully`);
        this.getAllRecipesAsync();
      },
      error: (error) => {
        const { status } = error;
        this.notyf.error(`Error adding recipe: ${status}`);
      }
    });
  }

  deleteRecipeAsync(id: string) {
    this.http.delete(this.url + '/' + id, { headers: this.headers }).subscribe({
      next: (recipe) => {
        this.notyf.success(`Recipe deleted: ${recipe}`);
        this.getAllRecipesAsync();
      },
      error: (error) => {
        const { status } = error;
        this.notyf.error(`Error deleting recipe: ${status}`);
      }
    });
  }

  get ingredients(): FormArray {
    return this.recipeForm.get('ingredients') as FormArray;
  }

  // Create a new ingredient form group
  createIngredientGroup(): FormGroup {
    return this.fb.group({
      ingredientName: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(0)]],
      unit: ['', Validators.required]
    });
  }

  // Add a new ingredient
  addIngredient(): void {
    this.ingredients.push(this.createIngredientGroup());
  }

  // Remove an ingredient by index
  removeIngredient(index: number): void {
    this.ingredients.removeAt(index);
  }

  // Handle form submission
  onSubmit(): void {
    if (this.recipeForm.valid) {
      const recipe: any = this.recipeForm.value;


      recipe.ingredients = recipe.ingredients.map((ingredient: { unit: string; }) => ({
        ...ingredient,
        unit: parseInt(ingredient.unit, 10) // Convert the unit from string to number
      }));

      console.log(recipe);

      this.addRecipe(recipe);

      this.recipeForm.reset();
    }
  }
}
