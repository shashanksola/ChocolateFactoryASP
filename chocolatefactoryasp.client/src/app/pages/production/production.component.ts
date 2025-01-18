import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-production',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './production.component.html',
})
export class ProductionComponent implements OnInit {
  productionForm!: FormGroup;
  editMode = false;
  filteredSchedules: any[] = [];
  supervisors: any[] = [];
  recipes: any[] = [];
  ingredients: any[] = [];
  rawMaterials: any[] = [];
  selectedRecipeIngredients: any[] = [];
  editingScheduleId: string | null = null;
  status = ['Scheduled', 'In Progress', 'Completed', 'Cancelled'];
  shifts = ['Morning', 'Evening', 'Night'];
  units = ['Kilogram',
    'Gram',
    'Liter',
    'Milliliter',
    'Piece']

  apiUrl = 'https://chocolatefactoryaspserver20250118211324.azurewebsites.net/api';
  notyf = new Notyf();
  headers: HttpHeaders;
  editingScheduleStatus: any;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    // Initialize headers with Bearer token
    const token = localStorage.getItem('token') || '';
    this.headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    this.fetchSupervisors();
    this.fetchRecipes();
    this.fetchRawMaterials();
    this.fetchSchedules();
  }

  // Initialize the form
  initializeForm(): void {
    this.productionForm = this.fb.group({
      recipeName: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: [''],
      shift: ['', Validators.required],
      supervisorId: ['', Validators.required],
      status: [0, Validators.required],
    },
      {
        validators: this.dateValidator
      });
  }

  dateValidator(control: AbstractControl): ValidationErrors | null {
    const startDate = new Date(control.get('startDate')?.value);
    const endDate = new Date(control.get('endDate')?.value);
    if (startDate > endDate) {
      return { invalidDeliveryDate: true }; // Custom error
    }
    return null;
  }
  // Fetch all supervisors
  fetchSupervisors(): void {
    this.http.get<any[]>(`${this.apiUrl}/User/2`, { headers: this.headers }).subscribe({
      next: (data) => (this.supervisors = data),
      error: () => this.notyf.error('Error fetching supervisors'),
    });
  }

  // Fetch all recipes
  fetchRecipes(): void {
    this.http.get<any[]>(`${this.apiUrl}/Recipe`, { headers: this.headers }).subscribe({
      next: (data) => (this.recipes = data),
      error: () => this.notyf.error('Error fetching recipes'),
    });
  }

  // Fetch all raw materials
  fetchRawMaterials(): void {
    this.http.get<any[]>(`${this.apiUrl}/RawMaterial`, { headers: this.headers }).subscribe({
      next: (data) => (this.rawMaterials = data),
      error: () => this.notyf.error('Error fetching raw materials'),
    });
  }

  // Fetch all production schedules
  fetchSchedules(): void {
    this.http.get<any[]>(`${this.apiUrl}/Production`, { headers: this.headers }).subscribe({
      next: (data) => (this.filteredSchedules = data),
      error: () => this.notyf.error('Error fetching schedules'),
    });
  }

  // Get supervisor name from ID
  getSupervisorName(supervisorId: string): string {
    const supervisor = this.supervisors.find((s) => s.userId === supervisorId);
    return supervisor ? supervisor.username : 'Unknown';
  }

  // Handle Recipe Selection
  onRecipeChange(): void {
    const selectedRecipeName = this.productionForm.get('recipeName')?.value;

    if (selectedRecipeName) {
      const selectedRecipe = this.recipes.find((r) => r.name === selectedRecipeName);
      if (selectedRecipe) {
        this.selectedRecipeIngredients = selectedRecipe.ingredients;

        // Map ingredients to availability
        this.ingredients = this.selectedRecipeIngredients.map((ingredient: any) => {
          const rawMaterial = this.rawMaterials.find(
            (rm) => rm.name.toLowerCase() === ingredient.ingredientName.toLowerCase()
          );

          return {
            ingredientName: ingredient.ingredientName,
            requiredQuantity: ingredient.quantity,
            availableQuantity: rawMaterial ? rawMaterial.stockQuantity : 0,
            unit: ingredient.unit,
          };
        });
      }
    }
  }

  completeSchedule(schedule: any) {
    console.log(schedule);
    this.http
      .patch(`${this.apiUrl}/Production/${schedule.scheduleId}`, schedule, {
        headers: this.headers,
      })
      .subscribe({
        next: () => {
          this.notyf.success('Schedule Completed successfully!');
          this.fetchSchedules();
        },
        error: (err) => {
          console.error(err);
          this.notyf.error('Error completing schedule.');
        },
      });
  }

  // Edit an existing schedule
  editSchedule(schedule: any): void {
    this.editMode = true;
    this.editingScheduleId = schedule.scheduleId;
    this.editingScheduleStatus = schedule.status;

    this.productionForm.patchValue({
      recipeName: schedule.recipeName,
      startDate: schedule.startDate.split('T')[0],
      endDate: schedule.endDate ? schedule.endDate.split('T')[0] : '',
      shift: schedule.shift,
      supervisorId: schedule.supervisorId,
      status: schedule.status,
    });

    this.onRecipeChange(); // Update ingredients table
  }

  // Save Production Schedule
  saveSchedule(): void {
    if (this.productionForm.invalid) {
      this.notyf.error('Please fill in all required fields.');
      return;
    }

    // Construct the schedule payload
    const productionSchedule = {
      recipeName: this.productionForm.value.recipeName,
      startDate: new Date(this.productionForm.value.startDate).toISOString(),
      endDate: this.productionForm.value.endDate
        ? new Date(this.productionForm.value.endDate).toISOString()
        : null,
      shift: parseInt(this.productionForm.value.shift),
      supervisorId: this.productionForm.value.supervisorId,
      status: parseInt(this.productionForm.value.status),
    };

    console.log('Submitting payload:', productionSchedule);

    if (this.editingScheduleStatus === 2 && localStorage.getItem('role') !== 'FactoryManager') {
      this.notyf.error(`You don't have the permission to change completed productions`)
      this.resetForm();
      return;
    }

    if (this.editMode && this.editingScheduleId) {
      // Update existing schedule
      this.http
        .put(`${this.apiUrl}/Production/${this.editingScheduleId}`, productionSchedule, {
          headers: this.headers,
        })
        .subscribe({
          next: () => {
            this.notyf.success('Schedule updated successfully!');
            this.resetForm();
            this.fetchSchedules();
          },
          error: (err) => {
            console.error(err);
            this.notyf.error('Error updating schedule.');
          },
        });
    } else {
      // Add new schedule
      this.http
        .post(`${this.apiUrl}/Production`, productionSchedule, { headers: this.headers })
        .subscribe({
          next: () => {
            this.notyf.success('Schedule added successfully!');
            this.resetForm();
            this.fetchSchedules();
          },
          error: (err) => {
            console.error(err);
            this.notyf.error('Error adding schedule.');
          },
        });
    }
  }


  // Reset Form
  resetForm(): void {
    this.editMode = false;
    this.editingScheduleId = null;
    this.productionForm.reset();
    this.ingredients = [];
  }
}
