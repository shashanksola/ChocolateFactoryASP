import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Notyf } from 'notyf';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { RouterModule, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})


export class LoginComponent {
  userLoginForm: FormGroup;
  notyf;

  constructor(private fb: FormBuilder, private http: HttpClient, private _router: Router) {
    this.userLoginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });

    this.notyf = new Notyf();
  }


  onSubmit() {
    if (this.userLoginForm.valid) {
      const loginData = this.userLoginForm.value;
      this.http.post('https://localhost:7051/api/Auth/login', loginData).subscribe({
        next: (response) => {
          this.notyf.success(`Login Successful, Welcom ${loginData.username}`)

          const { token } = response as { token: string };

          localStorage.setItem('token', token);

          const decodedToken: any = jwtDecode(token);
          let role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

          let userId = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

          localStorage.setItem('role', role);
          localStorage.setItem('userId', userId);
          localStorage.setItem('username', loginData.username);

          this._router.navigateByUrl('Console');
        },
        error: (error) => {
          this.notyf.error(`Login Unsuccessful : ${error.error}`)
        }
      });
    } else {
      this.notyf.error("Login Unsuccessfull, Please Enter Valid Credentials")
    }
  }
}
