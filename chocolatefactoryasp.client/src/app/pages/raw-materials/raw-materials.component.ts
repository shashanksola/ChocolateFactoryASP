import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-raw-material',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HttpClientModule],
  templateUrl: './raw-materials.component.html',
})
export class RawMaterialsComponent implements OnInit {
  warehouses: any[] = [];
  suppliers: any[] = []; // List of suppliers
  units: string[] = ['Kilogram', 'Gram', 'Liter', 'Milliliter', 'Piece']; // Unit options
  filteredMaterials: any[] = [];
  rawMaterialForm!: FormGroup;
  editMode: boolean = false;
  selectedWarehouse: string = '';

  apiUrl = 'https://localhost:7051api/RawMaterial';
  warehouseUrl = 'https://localhost:7051api/Warehouse';
  supplierUrl = 'https://localhost:7051api/Supplier';
  notyf = new Notyf();
  headers = { Authorization: `Bearer ${localStorage.getItem('token') || ''}` };

  constructor(private fb: FormBuilder, private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchWarehouses();
    this.fetchSuppliers();
    this.fetchMaterials();

    // Initialize form
    this.rawMaterialForm = this.fb.group({
      warehouseName: ['', Validators.required],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      unit: ['', Validators.required],
      supplierName: ['', Validators.required],
      costPerUnit: [0, [Validators.required, Validators.min(0)]],
    });
  }

  // Fetch warehouses
  fetchWarehouses(): void {
    this.http.get<any[]>(this.warehouseUrl, { headers: this.headers }).subscribe({
      next: (response) => (this.warehouses = response),
      error: () => this.notyf.error('Error fetching warehouses'),
    });
  }

  // Fetch suppliers
  fetchSuppliers(): void {
    this.http.get<any[]>(this.supplierUrl, { headers: this.headers }).subscribe({
      next: (response) => {
        this.suppliers = response.map((user) => user.name); // Assuming 'name' exists on user object
      },
      error: () => this.notyf.error('Error fetching suppliers'),
    });
  }

  // Fetch raw materials
  fetchMaterials(): void {
    this.http.get<any[]>(this.apiUrl, { headers: this.headers }).subscribe({
      next: (response) => (this.filteredMaterials = response),
      error: () => this.notyf.error('Error fetching raw materials'),
    });
  }

  // Filter raw materials by warehouse
  filterByWarehouse(): void {
    if (this.selectedWarehouse === '') {
      this.http.get<any[]>(`${this.apiUrl}`, { headers: this.headers }).subscribe({
        next: (response) => {
          this.filteredMaterials = response
          console.log(this.filteredMaterials)
        },
        error: () => this.notyf.error('Error getting all raw materials'),
      });
    } else {

      this.http.get<any[]>(`${this.apiUrl}/warehouse/${this.selectedWarehouse}`, { headers: this.headers }).subscribe({
        next: (response) => (this.filteredMaterials = response),
        error: () => this.notyf.error('Error filtering raw materials'),
      });
    }
  }

  // Save raw material (Add or Edit)
  saveRawMaterial(): void {
    const material = this.rawMaterialForm.value;
    material.unit = this.units.indexOf(material.unit);
    console.log(material);

    this.http.post(this.apiUrl, material, { headers: this.headers }).subscribe({
      next: () => {
        this.notyf.success('Raw material saved successfully!');
        this.fetchMaterials();
        this.resetForm();
      },
      error: () => this.notyf.error('Error saving raw material'),
    });
  }

  resetForm(): void {
    this.editMode = false;
    this.rawMaterialForm.reset();
  }

  editMaterial(material: any): void {
    this.editMode = true;
    this.rawMaterialForm.patchValue(material);
  }

  // Delete raw material
  async deleteMaterial(name: any): Promise<void> {
    if (confirm(`Are you sure you want to delete ${name.name}?`)) {
      try {
        await this.http.delete(`${this.apiUrl}/batch/${name.rawMaterialBatchId}`, { headers: this.headers }).toPromise();
        this.notyf.success('Raw material deleted successfully!');
        this.fetchMaterials();
      } catch (error) {
        this.notyf.error('Error deleting raw material.');
        console.error(error);
      }
    }
  }
}
