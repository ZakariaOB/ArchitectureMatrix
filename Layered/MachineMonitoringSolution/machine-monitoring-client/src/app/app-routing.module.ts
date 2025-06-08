import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MachinePageComponent } from './machine-page/machine-page.component';
import { MachineDashboardComponent } from './machine-dashboard/machine-dashboard.component';

const routes: Routes = [
  { path: '', component: MachinePageComponent },
  { path: 'dashboard/:id', component: MachineDashboardComponent },
  {
    path: '**',
    redirectTo: '/index.html' // Redirect to index.html
  }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
