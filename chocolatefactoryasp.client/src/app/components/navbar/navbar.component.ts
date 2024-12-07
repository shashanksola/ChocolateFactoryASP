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
  }
}
