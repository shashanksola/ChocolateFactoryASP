import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from "../navbar/navbar.component";
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-factory-manager',
  standalone: true,
  imports: [NavbarComponent, RouterOutlet, RouterModule],
  templateUrl: './factory-manager.component.html',
  styleUrl: './factory-manager.component.css'
})

export class FactoryManagerComponent implements OnInit {
  pages = [
    {
      id: 1,
      name: 'Dashboard',
      roles: ['FactoryManager'],
      path: 'dashboard',
    },
    {
      id: 2,
      name: 'Warehouse',
      roles: ['FactoryManager', 'ProductionSupervisor', 'PackagingStaff', 'MaterialStaff'],
      path: 'warehouse',
    },
    {
      id: 6,
      name: 'Raw Materials',
      roles: ['FactoryManager', 'MaterialStaff', 'ProductionSupervisor', 'PackagingStaff'],
      path: 'raw-materials',
    },
    {
      id: 10,
      name: 'Recipes',
      roles: ['FactoryManager', 'QualityController', 'ProductionSupervisor', 'Technician', 'PackagingStaff', 'MaterialStaff', 'SalesRepresentative'],
      path: 'recipes',
    },
    {
      id: 3,
      name: 'Production',
      roles: ['FactoryManager', 'ProductionSupervisor', 'QualityController'],
      path: 'production',
    },
    {
      id: 5,
      name: 'Quality Control',
      roles: ['FactoryManager', 'QualityController', 'PackagingStaff'],
      path: 'quality-control',
    },
    {
      id: 9,
      name: 'Packaging',
      roles: ['FactoryManager', 'PackagingStaff', 'SalesRepresentative'],
      path: 'packaging',
    },

    {
      id: 11,
      name: 'Sales',
      roles: ['FactoryManager', 'SalesRepresentative'],
      path: 'sales',
    },
    {
      id: 7,
      name: 'Users',
      roles: ['FactoryManager'],
      path: 'users',
    },
    {
      id: 12,
      name: 'Supplier',
      roles: ['FactoryManager'],
      path: 'supplier',
    },
    {
      id: 4,
      name: 'Maintenance',
      roles: ['FactoryManager', 'QualityController', 'ProductionSupervisor', 'Technician', 'PackagingStaff', 'MaterialStaff', 'SalesRepresentative'],
      path: 'maintenance',
    },
  ];

  ngOnInit(): void {
    const role = localStorage.getItem('role') || 'none';

    const temp = [];

    for (let i = 0; i < this.pages.length; i++) {
      if (this.pages[i].roles.includes(role)) {
        temp.push(this.pages[i]);
      }
    }

    this.pages = temp;
  }
}
