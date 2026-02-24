import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './pages/auth/login/login.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { LandingComponent } from './pages/landing/landing.component';

const routes: Routes = [
  // Landing Page
  { path: '', component: LandingComponent },
  
  // Öffentlicher Login-Bereich
  { path: 'auth/login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },

  // Geschützter Bereich mit Dashboard und Unterseiten
  //{ path: 'protected', component: ProtectedListComponent, canActivate: [AuthGuard] },

  // Fallback bei ungültiger URL
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
