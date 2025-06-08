import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MachineService } from '../_services/machine.service';
import { Machine } from '../_models/machine';
import { ToastService } from '../_services/toast.service';
import { finalize, pipe, concat, concatMap } from 'rxjs';
import { LoaderService } from '../_services/loader.service';

@Component({
  selector: 'app-machine-page',
  templateUrl: './machine-page.component.html',
  styleUrls: ['./machine-page.component.scss'],
})
export class MachinePageComponent implements OnInit {
  machines: Machine[] = [];

  constructor(
    private router: Router,
    private machineService: MachineService,
    private toastService: ToastService,
    private loadingService: LoaderService
  ) {}

  ngOnInit(): void {
    this.getMachines();
  }

  navigateToDashboard(machineId: number) {
    this.router.navigateByUrl('/dashboard/' + machineId);
  }

  deleteMachine(machineId: number): void {
    this.loadingService.setLoading(true);
    this.machineService
      .delete(machineId)
      .pipe(
        concatMap(() => this.machineService.getAll()),
        finalize(() => this.loadingService.setLoading(false))
      )
      .subscribe((machines) => {
        this.machines = machines;
        this.toastService.showSuccess('Item deleted successfully.');
      });
  }

  getMachines(): void {
    this.loadingService.setLoading(true);
    this.machineService
      .getAll()
      .pipe(finalize(() => this.loadingService.setLoading(false)))
      .subscribe((machines) => {
        this.machines = machines;
      });
  }
}
