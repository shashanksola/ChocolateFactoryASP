import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { NgxChartsModule, Color, ScaleType } from '@swimlane/ngx-charts';
import { Notyf } from 'notyf';
import { jsPDF } from 'jspdf';
import html2canvas from 'html2canvas';  // Import html2canvas

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgxChartsModule, HttpClientModule],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  headers: HttpHeaders;
  apiUrl = 'https://localhost:7051/api';
  notyf = new Notyf();

  productionData: any[] = [];
  warehouseStockData: any[] = [];
  qualityStatusData: any[] = [];
  salesData: any[] = [];

  view: [number, number] = [700, 400];

  colorScheme: Color = {
    name: 'customScheme',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#5AA454', '#E44D25', '#CFC0BB', '#7aa3e5']
  };

  constructor(private http: HttpClient) {
    const token = localStorage.getItem('token') || '';
    this.headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  ngOnInit(): void {
    this.fetchProductionData();
    this.fetchWarehouseData();
    this.fetchQualityData();
    this.fetchSalesData();
  }

  fetchProductionData(): void {
    this.http.get<any[]>(`${this.apiUrl}/Production`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.productionData = data.map((item) => ({
          name: item.recipeName,
          value: item.status === 1 ? 1 : 0,
        }));
      },
      error: () => this.notyf.error('Error fetching production data'),
    });
  }

  fetchWarehouseData(): void {
    this.http.get<any[]>(`${this.apiUrl}/Warehouse`, { headers: this.headers }).subscribe({
      next: (data) => {
        this.warehouseStockData = data.map((warehouse) => ({
          name: warehouse.name,
          value: warehouse.currentStockLevel,
        }));
      },
      error: () => this.notyf.error('Error fetching warehouse data'),
    });
  }

  fetchQualityData(): void {
    this.http.get<any[]>(`${this.apiUrl}/Quality`, { headers: this.headers }).subscribe({
      next: (data) => {
        const approved = data.filter((item) => item.status === 0).length;
        const rejected = data.filter((item) => item.status === 1).length;

        this.qualityStatusData = [
          { name: 'Approved', value: approved },
          { name: 'Rejected', value: rejected },
        ];
      },
      error: () => this.notyf.error('Error fetching quality data'),
    });
  }

  fetchSalesData(): void {
    this.http.get<any[]>(`${this.apiUrl}/Sales`, { headers: this.headers }).subscribe({
      next: (data) => {
        const salesMap: { [key: string]: number } = {};

        data.forEach((order) => {
          if (salesMap[order.status]) {
            salesMap[order.status] += order.quantity;
          } else {
            salesMap[order.status] = order.quantity;
          }
        });

        this.salesData = Object.keys(salesMap).map((status) => ({
          name: status,
          value: salesMap[status],
        }));
      },
      error: () => this.notyf.error('Error fetching sales data'),
    });
  }

  downloadPDF(): void {
    const element = document.getElementById('dashboard-info');

    // Use html2canvas to render the HTML into a canvas
    html2canvas(element!).then((canvas) => {
      const imgData = canvas.toDataURL('image/png');
      const pdf = new jsPDF();

      // Add the image to the PDF
      pdf.addImage(imgData, 'PNG', 0, 0, 210, 297);  // A4 size
      pdf.save('dashboard-summary.pdf');  // Save as PDF
    });
  }
}
