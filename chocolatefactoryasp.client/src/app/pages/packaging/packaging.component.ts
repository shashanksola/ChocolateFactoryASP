import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-packaging',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './packaging.component.html',
})
export class PackagingComponent implements OnInit {
  packagingForm!: FormGroup;
  finishedGoods: any[] = [];
  approvedProducts: any[] = [];
  warehouses: any[] = [];
  apiUrl = 'https://chocolatefactoryaspserver20250118211324.azurewebsites.net/api';
  notyf = new Notyf();
  headers: HttpHeaders;
  role = '';

  constructor(private fb: FormBuilder, private http: HttpClient) {
    const token = localStorage.getItem('token') || '';
    this.headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  ngOnInit(): void {
    this.initializeForm();
    this.fetchApprovedProducts();
    this.fetchFinishedGoods();

    this.role = localStorage.getItem('role') || '';
  }

  // Initialize the form
  initializeForm(): void {
    this.packagingForm = this.fb.group({
      batchId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      packagingDate: [new Date().toISOString().split('T')[0], Validators.required],
      expiryDate: [''],
      warehouseLocation: ['', Validators.required],
    }, { validators: this.dateValidator }
    );
  }

  dateValidator(control: AbstractControl): ValidationErrors | null {
    const packagingDate = new Date(control.get('packagingDate')?.value);
    const expiryDate = new Date(control.get('expiryDate')?.value);
    if (packagingDate > expiryDate) {
      return { invalidDeliveryDate: true }; // Custom error
    }
    return null;
  }

  // Fetch approved products
  // Fetch approved products and exclude already packaged finished goods
  fetchApprovedProducts(): void {
    // Fetch finished goods first
    this.http.get<any[]>(`${this.apiUrl}/Packaging`, { headers: this.headers }).subscribe({
      next: (finishedGoodsData) => {
        this.finishedGoods = finishedGoodsData;

        // Fetch approved products
        this.http.get<any[]>(`${this.apiUrl}/Quality`, { headers: this.headers }).subscribe({
          next: (qualityData) => {
            // Filter products that are approved and not already packaged
            const finishedBatchIds = this.finishedGoods.map((good) => good.batchId);

            this.approvedProducts = qualityData.filter(
              (product) => product.status === 0 && !finishedBatchIds.includes(product.batchId)
            );

            console.log('Filtered Approved Products:', this.approvedProducts);
          },
          error: () => this.notyf.error('Error fetching approved products.'),
        });
      },
      error: () => this.notyf.error('Error fetching finished goods.'),
    });
  }


  // Fetch all finished goods
  fetchFinishedGoods(): void {
    this.http.get<any[]>(`${this.apiUrl}/Packaging`, { headers: this.headers }).subscribe({
      next: (data) => (this.finishedGoods = data),
      error: () => this.notyf.error('Error fetching finished goods.'),
    });
  }

  // Fetch warehouses with sufficient stock
  fetchWarehousesWithStock(quantity: number): void {
    this.http
      .get<any[]>(`${this.apiUrl}/Warehouse/sufficient-stock/${quantity}`, { headers: this.headers })
      .subscribe({
        next: (data) => {
          this.warehouses = data
          console.log(data);
        },
        error: () => this.notyf.error('Error fetching warehouses with sufficient stock.'),
      });
  }

  // Handle Batch ID Change
  onBatchIdChange(): void {
    const selectedBatchId = this.packagingForm.get('batchId')?.value;

    if (selectedBatchId) {
      this.http
        .get<number>(`${this.apiUrl}/Packaging/qunatity/${selectedBatchId}`, { headers: this.headers })
        .subscribe({
          next: (quantity) => {
            this.packagingForm.patchValue({ quantity }); // Auto-fill quantity
            this.fetchWarehousesWithStock(quantity); // Fetch warehouses based on stock
          },
          error: () => this.notyf.error('Error fetching quantity for selected batch.'),
        });
    }
  }

  // Submit Packaged Product
  submitFinishedGood(): void {
    if (this.packagingForm.invalid) {
      this.notyf.error('Please fill in all required fields.');
      return;
    }

    const finishedGood = this.packagingForm.value;

    this.http.post(`${this.apiUrl}/Packaging`, finishedGood, { headers: this.headers }).subscribe({
      next: () => {
        this.notyf.success('Finished good added successfully!');
        this.fetchFinishedGoods(); // Refresh table
        this.packagingForm.reset();
        this.packagingForm.patchValue({ packagingDate: new Date().toISOString().split('T')[0] });
        this.warehouses = []; // Clear warehouses dropdown
      },
      error: () => this.notyf.error('Error adding finished good.'),
    });
  }
}
