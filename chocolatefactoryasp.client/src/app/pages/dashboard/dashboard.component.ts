import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { Notyf } from 'notyf';
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgxChartsModule, HttpClientModule, CommonModule],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  headers: HttpHeaders;
  apiUrl = 'https://localhost:7051api';
  notyf = new Notyf();

  productionData: any[] = [];
  warehouseStockData: any[] = [];
  qualityStatusData: any[] = [];
  salesData: any[] = [];

  view: [number, number] = [700, 400];

  // colorScheme: Color = {
  //   name: 'customScheme',
  //   selectable: true,
  //   group: ScaleType.Ordinal,
  //   domain: ['#5AA454', '#E44D25', '#CFC0BB', '#7aa3e5']
  // };

  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  showXAxisLabel = true;
  xAxisLabel = 'Prodcuts';
  showYAxisLabel = true;
  yAxisLabel = 'Production Count';

  colorScheme = {
    domain: ['#EAD2AC', '#8A6552', '#CFC0BB', '#7aa3e5']
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

        const groupedData = Array.from(
          this.productionData.reduce((acc: Map<string, { name: string; value: number }>, item) => {
            if (!acc.has(item.name)) {
              acc.set(item.name, { name: item.name, value: item.value });
            } else {
              acc.get(item.name)!.value += item.value;
            }
            return acc;
          }, new Map<string, { name: string; value: number }>())
        ).map(([_, value]) => value);

        this.productionData = groupedData;
      },
      error: () => this.notyf.error('Error fetching production data'),
    });

    console.log(this.productionData);
  }

  fetchWarehouseData(): void {
    this.http.get<any[]>(`${this.apiUrl}/Warehouse`, { headers: this.headers }).subscribe({
      next: (data) => {
        console.log(data);
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

        this.salesData = data.map((order) => {
          return {
            name: order.orderId,
            value: order.quantity
          }
        });

        console.log(data);

        console.log(this.salesData);
      },
      error: () => this.notyf.error('Error fetching sales data'),
    });
  }

  downloadPDF(): void {
    const pdf = new jsPDF();

    // Table for Production Data
    if (this.productionData.length > 0) {
      pdf.text('Production Data', 10, 10);
      autoTable(pdf, {
        startY: 15,
        head: [['Recipe Name', 'Production Status']],
        body: this.productionData.map((item) => [item.name, item.value.toString()]),
      });
    }

    // Table for Warehouse Data
    if (this.warehouseStockData.length > 0) {
      pdf.text('Warehouse Stock Data', 10, 80);
      autoTable(pdf, {
        startY: 25,
        head: [['Warehouse Name', 'Current Stock Level']],
        body: this.warehouseStockData.map((item) => [item.name, item.value.toString()]),
      });
    }

    // Table for Quality Data
    if (this.qualityStatusData.length > 0) {
      pdf.text('Quality Status Data', 10, 160);
      autoTable(pdf, {
        startY: 20,
        head: [['Status', 'Count']],
        body: this.qualityStatusData.map((item) => [item.name, item.value.toString()]),
      });
    }

    // Table for Sales Data
    if (this.salesData.length > 0) {
      pdf.text('Sales Data', 10, 240);
      autoTable(pdf, {
        startY: 10 + 15,
        head: [['Order ID', 'Quantity']],
        body: this.salesData.map((item) => [item.name, item.value.toString()]),
      });
    }

    // Save the PDF
    pdf.save('dashboard-data.pdf');
  }

}
