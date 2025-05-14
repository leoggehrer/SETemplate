//@CodeCopy
import { Component } from '@angular/core';
import { environment } from '@environment/environment';
import { AuthService } from '@app-services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = '';
  password = '';
  error = '';
  returnUrl = '/';

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute) {

  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    if (!environment.requireLogin) {
      this.router.navigateByUrl(this.returnUrl);
    }
  }

  public onLogin() {
    if (this.auth.login(this.username, this.password)) {
      localStorage.setItem('auth', 'true');
      this.router.navigateByUrl(this.returnUrl);
    } else {
      this.error = 'Login fehlgeschlagen';
    }
  }
}
