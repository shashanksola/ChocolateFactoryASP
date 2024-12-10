import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { FactoryManagerComponent } from './components/factory-manager/factory-manager.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { WarehouseComponent } from './pages/warehouse/warehouse.component';
import { ProductionComponent } from './pages/production/production.component';
import { MaintenanceComponent } from './pages/maintenance/maintenance.component';
import { QualityControlComponent } from './pages/quality-control/quality-control.component';
import { RawMaterialsComponent } from './pages/raw-materials/raw-materials.component';
import { UsersComponent } from './pages/users/users.component';
import { PackagingComponent } from './pages/packaging/packaging.component';
import { RecipesComponent } from './pages/recipes/recipes.component';
import { SalesComponent } from './pages/sales/sales.component';
import { SupplierComponent } from './pages/supplier/supplier.component';
import { AuthGuard } from './auth/auth.guard';
import { RegisterComponent } from './components/register/register.component';

export const routes: Routes = [{
    path: '',
    component: HomeComponent
},
{
    path: 'login',
    component: LoginComponent
},
{
    path: 'register',
    component: RegisterComponent
},
{
    path: 'Console',
    component: FactoryManagerComponent,
    canActivate: [AuthGuard],
    children: [
        {
            path: '',
            component: RecipesComponent,
        },
        {
            path: 'dashboard',
            component: DashboardComponent,
        },
        {
            path: 'warehouse',
            component: WarehouseComponent,
        },
        {
            path: 'production',
            component: ProductionComponent,
        },
        {
            path: 'maintenance',
            component: MaintenanceComponent,
        },
        {
            path: 'quality-control',
            component: QualityControlComponent,
        },
        {
            path: 'raw-materials',
            component: RawMaterialsComponent,
        },
        {
            path: 'users',
            component: UsersComponent,
        },
        {
            path: 'packaging',
            component: PackagingComponent,
        },
        {
            path: 'recipes',
            component: RecipesComponent,
        },
        {
            path: 'sales',
            component: SalesComponent,
        },
        {
            path: 'supplier',
            component: SupplierComponent
        },
        {
            path: '**',
            component: NotFoundComponent
        },
    ]
},
{
    path: '**',
    component: NotFoundComponent
}];
