import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { RouterModule, Router } from '@angular/router';
import { Notyf } from 'notyf';



@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  userRegisterForm: FormGroup;
  notyf;

  roles = [
    'FactoryManager',
    'QualityController',
    'ProductionSupervisor',
    'Technician',
    'PackagingStaff',
    'MaterialStaff',
    'SalesRepresentative'
  ]

  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router) {
    this.userRegisterForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      firstName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
    this.notyf = new Notyf();
  }

  onSubmit() {
    if (this.userRegisterForm.valid) {
      const registerData = this.userRegisterForm.value;

      registerData.role = this.roles.indexOf(registerData.role);

      console.log(registerData);

      this.http.post('https://localhost:7051/api/Auth/registerNew', registerData).subscribe({
        next: () => {
          this.notyf.success('Registration Successful! Wait for authorization');
          this.router.navigateByUrl('/login');
          alert('Factory Manager has to approve your credentials, please wait for the approval mail.');
        },
        error: (err) => {
          this.notyf.error(`Registration Failed: ${err.error}`);
        }
      });
    } else {
      this.notyf.error('Please fill out all fields correctly.');
    }
  }
}
