import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-warehouse',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HttpClientModule],
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.css'],
})
export class WarehouseComponent implements OnInit {
  warehouses: any[] = [];
  warehouseForm!: FormGroup;
  editMode: boolean = false;
  apiUrl = 'https://localhost:7051api/Warehouse';
  notyf = new Notyf();
  headers = {
    Authorization: `Bearer ${localStorage.getItem('token') || ''}`,
  };
  role = '';

  constructor(private fb: FormBuilder, private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchWarehouses();

    // Initialize the form
    this.warehouseForm = this.fb.group({
      location: ['', [Validators.required, Validators.maxLength(200)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      capacity: [1, [Validators.required, Validators.min(1)]],
      currentStockLevel: [0, [Validators.min(0)]],
    });

    this.role = localStorage.getItem('role') || '';
  }

  // Fetch all warehouses
  fetchWarehouses(): void {
    this.http.get<any[]>(this.apiUrl, { headers: this.headers }).subscribe({
      next: (response) => {
        this.warehouses = response;
        console.log(this.warehouses);
      },
      error: (error) => {
        this.notyf.error('Error fetching warehouses.');
        console.error(error);
      },
    });
  }

  // Add or update warehouse
  saveWarehouse(): void {
    const warehouse = this.warehouseForm.value;

    if (this.editMode) {
      // Update warehouse
      this.http.put(this.apiUrl, warehouse, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Warehouse updated successfully!');
          this.fetchWarehouses();
          this.resetForm();
        },
        error: (error) => {
          this.notyf.error('Error updating warehouse.');
          console.error(error);
        },
      });
    } else {
      // Add warehouse
      this.http.post(this.apiUrl, warehouse, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Warehouse added successfully!');
          this.fetchWarehouses();
          this.resetForm();
        },
        error: (error) => {
          this.notyf.error('Error adding warehouse.');
          console.error(error);
        },
      });
    }
  }

  // Edit warehouse
  editWarehouse(warehouse: any): void {
    this.editMode = true;
    this.warehouseForm.patchValue(warehouse);
  }

  // Delete warehouse
  deleteWarehouse(name: string): void {
    if (confirm(`Are you sure you want to delete the warehouse: ${name}?`)) {
      this.http.delete(`${this.apiUrl}/${name}`, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Warehouse deleted successfully!');
          this.fetchWarehouses();
        },
        error: (error) => {
          this.notyf.error('Error deleting warehouse.');
          console.error(error);
        },
      });
    }
  }

  // Reset form
  resetForm(): void {
    this.editMode = false;
    this.warehouseForm.reset();
  }
}
