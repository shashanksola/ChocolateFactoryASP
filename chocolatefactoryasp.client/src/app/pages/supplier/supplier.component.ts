import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-supplier',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HttpClientModule],
  templateUrl: './supplier.component.html',
})
export class SupplierComponent implements OnInit {
  suppliers: any[] = [];
  supplierForm!: FormGroup;
  editMode: boolean = false;
  selectedSupplierId: string = '';
  apiUrl = 'https://localhost:7051api/Supplier';
  notyf = new Notyf();
  headers = { Authorization: `Bearer ${localStorage.getItem('token') || ''}` };


  constructor(private fb: FormBuilder, private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchSuppliers();
    this.initializeForm();
  }

  // Initialize Form
  initializeForm(): void {
    this.supplierForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      address: ['', [Validators.required, Validators.maxLength(500)]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      email: ['', [Validators.required, Validators.email]],
      notes: ['', Validators.maxLength(500)],
    });
  }

  // Fetch all suppliers
  fetchSuppliers(): void {
    this.http.get<any[]>(this.apiUrl, { headers: this.headers }).subscribe({
      next: (response) => (this.suppliers = response),
      error: () => this.notyf.error('Error fetching suppliers.'),
    });
  }

  // Add or Update Supplier
  saveSupplier(): void {
    if (this.supplierForm.invalid) return;

    const supplier = this.supplierForm.value;

    if (this.editMode) {
      // Update supplier
      this.http.put(`${this.apiUrl}/${this.selectedSupplierId}`, { ...supplier, supplierId: this.selectedSupplierId }, { headers: this.headers })
        .subscribe({
          next: () => {
            this.notyf.success('Supplier updated successfully!');
            this.fetchSuppliers();
            this.resetForm();
          },
          error: () => this.notyf.error('Error updating supplier.'),
        });
    } else {
      // Add new supplier
      this.http.post(this.apiUrl, supplier, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Supplier added successfully!');
          this.fetchSuppliers();
          this.resetForm();
        },
        error: () => this.notyf.error('Error adding supplier.'),
      });
    }
  }

  // Edit Supplier
  editSupplier(supplier: any): void {
    this.editMode = true;
    this.selectedSupplierId = supplier.supplierId;
    this.supplierForm.patchValue({
      name: supplier.name,
      address: supplier.address,
      phone: supplier.phone,
      email: supplier.email,
      notes: supplier.notes,
    });
  }

  // Delete Supplier
  deleteSupplier(id: string): void {
    if (confirm('Are you sure you want to delete this supplier?')) {
      this.http.delete(`${this.apiUrl}/${id}`, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Supplier deleted successfully!');
          this.fetchSuppliers();
        },
        error: () => this.notyf.error('Error deleting supplier.'),
      });
    }
  }

  // Reset Form
  resetForm(): void {
    this.editMode = false;
    this.selectedSupplierId = '';
    this.supplierForm.reset();
  }
}
