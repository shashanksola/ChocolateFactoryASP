import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  scrollToFooter() {
    document.getElementById('footer')?.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });
  }
}
