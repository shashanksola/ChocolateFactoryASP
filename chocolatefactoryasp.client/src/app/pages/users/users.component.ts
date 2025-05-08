import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Notyf } from 'notyf';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
})
export class UsersComponent implements OnInit {
  notyf = new Notyf();
  users: any[] = [];
  filteredUsers: any[] = [];
  roles: string[] = [
    'Factory Manager',
    'Quality Controller',
    'Production Supervisor',
    'Technician',
    'Packaging Staff',
    'Material Staff',
    'Sales Representative'
  ];
  userForm!: FormGroup;
  selectedRole = 0;
  headers = {
    Authorization: `Bearer ${localStorage.getItem('token') || ''}`,
  };
  apiUrl = 'https://localhost:7051/api/User';

  constructor(private fb: FormBuilder, private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchAllUsers();

    this.userForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      firstName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  fetchUsers(role: number): void {
    this.http.get<any[]>(`${this.apiUrl}/${role}`, { headers: this.headers }).subscribe({
      next: (response) => {
        this.users = response;
        this.filteredUsers = response;
      },
      error: (error) => {
        console.error('Error fetching users:', error);
      },
    });
  }

  fetchAllUsers(): void {
    this.http.get<any>(`${this.apiUrl}`, { headers: this.headers }).subscribe({
      next: (response) => {
        this.users = response;
        this.filteredUsers = response;
      },
      error: (error) => {
        console.error('Error fetching users:', error);
      },
    });
  }

  filterByRole(event: Event): void {
    const selectedRole = (event.target as HTMLSelectElement).value;
    this.selectedRole = this.roles.indexOf(selectedRole);

    if (this.selectedRole != -1) this.fetchUsers(this.selectedRole);
    this.fetchAllUsers();
  }

  registerUser(): void {
    if (this.userForm.valid) {
      let newUser = this.userForm.value;
      newUser.role = this.roles.indexOf(newUser.role);

      this.http.post(`https://localhost:7051/api/Auth/register`, newUser, { headers: this.headers }).subscribe({
        next: () => {
          this.notyf.success('User registration successfull')
          this.userForm.reset();
          this.fetchAllUsers();
        },
        error: (error) => {
          console.log(error);
          this.notyf.error('Error registering user');
        },
      });
    }
  }

  deleteUser(username: string) {
    this.http.delete(`https://localhost:7051/api/User/${username}`, { headers: this.headers }).subscribe({
      next: () => {
        this.notyf.success('User deletion successfull')
        this.fetchAllUsers();
      },
      error: (error) => {
        console.log(error);
        this.notyf.error('Error deleting user');
      },
    });
  }

  activateUser(user: any) {
    console.log(user);
    this.http.patch<any>(`https://localhost:7051/api/User/setActive/${user.userId}`, null, { headers: this.headers }).subscribe({
      next: (response) => {
        this.notyf.success(response.message);
        this.fetchAllUsers();
      }, error: (err) => {
        console.error(err);
        this.notyf.error('Trouble setting user to active')
      }
    })
  }
}
