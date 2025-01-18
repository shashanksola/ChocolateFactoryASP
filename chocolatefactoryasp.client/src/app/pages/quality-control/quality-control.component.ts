import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-quality-control',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './quality-control.component.html',
})
export class QualityControlComponent implements OnInit {
  qualityForm!: FormGroup;
  completedProductions: any[] = [];
  filteredProductions: any[] = []; // Filtered list for Batch IDs
  qualityControllers: any[] = [];
  qualityChecks: any[] = [];
  qualityStatusOptions = ['Approved', 'Rejected', 'Pending'];
  apiUrl = 'https://chocolatefactoryaspserver20250118211324.azurewebsites.net/api';
  headers: HttpHeaders;
  notyf = new Notyf();
  role = '';

  constructor(private fb: FormBuilder, private http: HttpClient) {
    const token = localStorage.getItem('token') || '';
    this.headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  ngOnInit(): void {
    this.initializeForm();
    this.fetchCompletedProductions();
    this.fetchQualityControllers();
    this.fetchQualityChecks();

    this.role = localStorage.getItem('role') || "";
  }

  initializeForm(): void {
    this.qualityForm = this.fb.group({
      batchId: ['', Validators.required],
      qualityControllerId: ['', Validators.required],
      inspectionDate: ['', Validators.required],
      status: ['', Validators.required],
      testResults: ['', [Validators.required, Validators.maxLength(500)]],
    });
  }

  fetchCompletedProductions(): void {
    this.http.get<any[]>(`https://chocolatefactoryaspserver20250118211324.azurewebsites.net/Completed`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.completedProductions = data;
        this.filterBatchIds();
      },
      error: () => this.notyf.error('Error fetching completed productions.'),
    });
  }

  fetchQualityChecks(): void {
    this.http.get<any[]>(`${this.apiUrl}/Quality`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.qualityChecks = data;
        this.filterBatchIds(); // Re-filter when quality checks update
      },
      error: () => this.notyf.error('Error fetching quality checks.'),
    });
  }

  fetchQualityControllers(): void {
    this.http.get<any[]>(`${this.apiUrl}/User/1`, { headers: this.headers }).subscribe({
      next: (data) => (this.qualityControllers = data),
      error: () => this.notyf.error('Error fetching quality controllers.'),
    });
  }

  // Filter batch IDs that are not already in qualityChecks
  filterBatchIds(): void {
    const checkedBatchIds = new Set(this.qualityChecks.map((qc) => qc.batchId));
    this.filteredProductions = this.completedProductions.filter(
      (prod) => !checkedBatchIds.has(prod.scheduleId)
    );
  }

  submitQualityCheck(): void {
    if (this.qualityForm.invalid) {
      this.notyf.error('Please fill in all required fields.');
      return;
    }

    const qualityCheck = this.qualityForm.value;
    qualityCheck.status = this.qualityStatusOptions.indexOf(qualityCheck.status);
    console.log(qualityCheck);
    this.http.post(`${this.apiUrl}/Quality`, qualityCheck, { headers: this.headers }).subscribe({
      next: () => {
        this.notyf.success('Quality check added successfully!');
        this.fetchQualityChecks(); // Refresh list
        this.qualityForm.reset();
      },
      error: () => this.notyf.error('Error submitting quality check.'),
    });
  }
}
