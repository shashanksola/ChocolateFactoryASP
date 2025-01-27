import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
  sidebarStatus = false;

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

  constructor(private _router: Router) { }

  onSignOutClick() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
    localStorage.removeItem('username');

    this._router.navigateByUrl("/login");
  }
  role: string = "";
  username: string = "";

  ngOnInit() {
    this.role = localStorage.getItem('role') || "Role";
    this.username = localStorage.getItem('username') || "[Username]";

    const temp = [];

    for (let i = 0; i < this.pages.length; i++) {
      if (this.pages[i].roles.includes(this.role)) {
        temp.push(this.pages[i]);
      }
    }

    this.pages = temp;
  }

  toggleSidebar() {
    this.sidebarStatus = !this.sidebarStatus;
  }
}
