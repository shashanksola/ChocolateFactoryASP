import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-maintenance',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './maintenance.component.html',
})
export class MaintenanceComponent implements OnInit {
  maintenanceForm!: FormGroup;
  maintenanceRecords: any[] = [];
  equipmentList: string[] = [
    'Boiler',
    'Conveyor Belt',
    'Packaging Machine',
    'Mixing Machine',
    'Cooling System',
    'Oven',
    'Storage Tank',
    'Pump',
    'Generator',
  ];
  technicians: any[] = []; // Holds users with Technician role
  editMode = false;
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
    this.fetchMaintenanceRecords();
    this.fetchTechnicians();

    this.role = localStorage.getItem('role') || '';
  }

  // Initialize form
  initializeForm(): void {
    this.maintenanceForm = this.fb.group({
      equipmentName: ['', Validators.required],
      maintenanceDate: ['', Validators.required],
      technician: ['', Validators.required],
      nextScheduledDate: ['', Validators.required],
      details: ['', Validators.maxLength(500)],
    }, { validators: this.dateValidator });
  }

  dateValidator(control: AbstractControl): ValidationErrors | null {
    const maintenanceDate = new Date(control.get('maintenanceDate')?.value);
    const nextScheduledDate = new Date(control.get('nextScheduledDate')?.value);
    if (maintenanceDate > nextScheduledDate) {
      return { invalidDeliveryDate: true }; // Custom error
    }
    return null;
  }

  // Fetch maintenance records
  fetchMaintenanceRecords(): void {
    this.http.get<any[]>(`${this.apiUrl}/Maintenance`, { headers: this.headers }).subscribe({
      next: (data) => (this.maintenanceRecords = data),
      error: () => this.notyf.error('Error fetching maintenance records.'),
    });
  }

  // Fetch technicians (users with Technician role)
  fetchTechnicians(): void {
    this.http.get<any[]>(`${this.apiUrl}/User/3`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.technicians = data;
        if (data.length === 0) {
          this.notyf.error('There are no technicians, Hire Some!!');
        }
      },
      error: () => this.notyf.error('Error fetching technicians.'),
    });
  }

  // Submit or Update Maintenance Record
  submitMaintenanceRecord(): void {
    if (this.maintenanceForm.invalid) {
      this.notyf.error('Please fill in all required fields.');
      return;
    }

    const record = this.maintenanceForm.value;

    if (this.editMode) {
      // Update existing record
      this.http.put(`${this.apiUrl}/Maintenance`, record, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Maintenance record updated successfully!');
          this.resetForm();
          this.fetchMaintenanceRecords();
        },
        error: () => this.notyf.error('Error updating record.'),
      });
    } else {
      // Add new record
      this.http.post(`${this.apiUrl}/Maintenance`, record, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Maintenance record added successfully!');
          this.resetForm();
          this.fetchMaintenanceRecords();
        },
        error: () => this.notyf.error('Error adding record.'),
      });
    }
  }

  // Edit an existing maintenance record
  editMaintenanceRecord(record: any): void {
    this.editMode = true;
    this.maintenanceForm.patchValue(record);
  }

  // Delete a maintenance record
  deleteMaintenanceRecord(id: string): void {
    if (confirm('Are you sure you want to delete this record?')) {
      this.http.delete(`${this.apiUrl}/Maintenance/${id}`, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('Record deleted successfully!');
          this.fetchMaintenanceRecords();
        },
        error: () => this.notyf.error('Error deleting record.'),
      });
    }
  }

  // Reset form
  resetForm(): void {
    this.editMode = false;
    this.maintenanceForm.reset();
  }
}
