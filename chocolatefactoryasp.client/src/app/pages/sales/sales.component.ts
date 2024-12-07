import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './sales.component.html',
})
export class SalesComponent implements OnInit {
  salesForm!: FormGroup;
  finishedGoods: any[] = []; // Approved products available for sales
  salesOrders: any[] = []; // Orders already placed
  loggedInUserId: string = ''; // Placeholder for SalesRepresentative ID
  headers: HttpHeaders;
  apiUrl = 'https://localhost:7051/api';

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

    this.loggedInUserId = localStorage.getItem('userId') || "";
  }

  // Simulate getting the logged-in SalesRepresentative's ID
  getLoggedInUserId(): void {
    const userData = localStorage.getItem('user');
    if (userData) {
      this.loggedInUserId = JSON.parse(userData).userId; // Assuming stored user info contains userId
    }
  }

  // Initialize Form
  initializeForm(): void {
    const today = new Date();
    const deliveryDate = new Date();
    deliveryDate.setDate(today.getDate() + 3); // Add 3 days to today

    this.salesForm = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      orderDate: [today.toISOString().split('T')[0], Validators.required], // Current date
      deliveryDate: [deliveryDate.toISOString().split('T')[0], Validators.required], // 3 days from today
    });
  }

  // Fetch approved products for sales
  fetchApprovedProducts(): void {
    this.http.get<any[]>(`${this.apiUrl}/Packaging`, { headers: this.headers }).subscribe({
      next: (data) => {
        console.log(data);
        this.finishedGoods = data.filter((item) => item.quantity > 0);
      },
      error: () => this.notyf.error('Error fetching finished goods'),
    });
  }

  // Fetch existing sales orders
  fetchSalesOrders(): void {
    this.http.get<any[]>(`${this.apiUrl}/Sales`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.salesOrders = data;
      },
      error: () => this.notyf.error('Error fetching sales orders'),
    });
  }

  // Submit Sales Order
  submitOrder(): void {
    if (this.salesForm.invalid) {
      this.notyf.error('Please fill in all required fields.');
      return;
    }

    let order = this.salesForm.value;
    order.status = "Sold";
    order.customerId = this.loggedInUserId;
    console.log(order);



    this.http.post(`${this.apiUrl}/Sales`, order, { headers: this.headers }).subscribe({
      next: () => {
        this.notyf.success('Order placed successfully!');
        this.fetchSalesOrders();
        this.fetchApprovedProducts();
        this.salesForm.reset();
        this.salesForm.patchValue({ orderDate: new Date().toISOString().split('T')[0] });
      },
      error: (err) => {
        console.error(err);
        this.notyf.error('Error placing order');
      },
    });
  }
}
