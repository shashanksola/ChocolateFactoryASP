import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders, HttpClientModule } from '@angular/common/http';
import { Notyf } from 'notyf';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sales',
  standalone: true,
  templateUrl: './sales.component.html',
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule]
})
export class SalesComponent implements OnInit {
  salesForm!: FormGroup;
  finishedGoods: any[] = []; // Products for sale
  salesOrders: any[] = []; // Existing sales orders
  loggedInUserId: string = ''; // Placeholder for user ID
  headers: HttpHeaders;
  apiUrl = 'https://chocolatefactoryaspserver20250118211324.azurewebsites.net/api';
  notyf = new Notyf();

  constructor(private fb: FormBuilder, private http: HttpClient) {
    const token = localStorage.getItem('token') || '';
    this.headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  ngOnInit(): void {
    this.getLoggedInUserId();
    this.initializeForm();
    this.fetchApprovedProducts();
    this.fetchSalesOrders();
  }

  /** Get Logged-In User */
  getLoggedInUserId(): void {
    this.loggedInUserId = localStorage.getItem('userId') || '';
  }

  /** Validator for Delivery Date > Order Date */
  deliveryDateValidator(control: AbstractControl): ValidationErrors | null {
    const orderDate = new Date(control.get('orderDate')?.value);
    const deliveryDate = new Date(control.get('deliveryDate')?.value);
    if (deliveryDate <= orderDate) {
      return { invalidDeliveryDate: true }; // Custom error
    }
    return null;
  }

  /** Initialize Form with Validators */
  initializeForm(): void {
    const today = new Date();
    const deliveryDate = new Date(today);
    deliveryDate.setDate(today.getDate() + 3);

    this.salesForm = this.fb.group(
      {
        productId: ['', Validators.required],
        quantity: [1, [Validators.required, Validators.min(1)]],
        orderDate: [this.formatToISOString(today), Validators.required],
        deliveryDate: [this.formatToISOString(deliveryDate), Validators.required],
      },
      { validators: this.deliveryDateValidator }
    );
  }

  /** Convert Date to ISO String with UTC format */
  formatToISOString(date: Date): string {
    return date.toISOString();
  }

  /** Fetch Approved Products */
  fetchApprovedProducts(): void {
    this.http.get<any[]>(`${this.apiUrl}/Packaging`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.finishedGoods = data.filter((item) => item.quantity > 0);
      },
      error: () => this.notyf.error('Failed to fetch finished goods.'),
    });
  }

  /** Fetch Existing Sales Orders */
  fetchSalesOrders(): void {
    this.http.get<any[]>(`${this.apiUrl}/Sales`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.salesOrders = data;
      },
      error: () => this.notyf.error('Failed to fetch sales orders.'),
    });
  }

  /** Submit Sales Order */
  submitOrder(): void {
    console.log(this.salesForm);
    if (this.salesForm.invalid) {
      if (this.salesForm.errors?.['invalidDeliveryDate']) {
        this.notyf.error('Delivery date must be after the order date.');
      } else {
        this.notyf.error('Please fill in all required fields.');
      }
      return;
    }

    const order = {
      ...this.salesForm.value,
      orderDate: this.formatToISOString(new Date(this.salesForm.value.orderDate)),
      deliveryDate: this.formatToISOString(new Date(this.salesForm.value.deliveryDate)),
      status: 'Sold',
      customerId: this.loggedInUserId,
    };

    console.log(order);

    this.http.post(`${this.apiUrl}/Sales`, order, { headers: this.headers }).subscribe({
      next: () => {
        this.notyf.success('Order placed successfully!');
        this.fetchSalesOrders();
        this.fetchApprovedProducts();
        this.resetForm();
      },
      error: (err) => {
        console.error(err);
        this.notyf.error('Error placing order.');
      },
    });
  }

  /** Reset Form and Reinitialize Dates */
  resetForm(): void {
    const today = new Date();
    const deliveryDate = new Date(today);
    deliveryDate.setDate(today.getDate() + 3);

    this.salesForm.reset({
      productId: '',
      quantity: 1,
      orderDate: this.formatToISOString(today),
      deliveryDate: this.formatToISOString(deliveryDate),
    });
  }
}
