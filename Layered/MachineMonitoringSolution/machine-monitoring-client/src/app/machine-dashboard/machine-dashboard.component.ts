import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Machine } from '../_models/machine';
import { MachineService } from '../_services/machine.service';
import { Subscription, finalize, interval } from 'rxjs';
import { LoaderService } from '../_services/loader.service';

@Component({
  selector: 'app-machine-dashboard',
  templateUrl: './machine-dashboard.component.html',
  styleUrls: ['./machine-dashboard.component.scss'],
})
export class MachineDashboardComponent implements OnDestroy {
  machine: Machine | null = null;
  machineRefreshSubscription: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private machineService: MachineService,
    private loadingService: LoaderService
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      const machineId = +params.get('id')!;
      this.getMachineData(machineId);
    });
    this.startRefreshMachineDataTimer();
  }

  getMachineData(machineId: number): void {
    this.loadingService.setLoading(true);
    this.machineService
      .getForDashboard(machineId)
      .pipe(finalize(() => this.loadingService.setLoading(false)))
      .subscribe((machine) => {
        this.machine = machine;
      });
  }

  private startRefreshMachineDataTimer(): void {
    this.machineRefreshSubscription = interval(5000).subscribe(() => {
      if (this.machine?.machineId) {
        this.getMachineData(this.machine?.machineId);
      }
    });
  }

  ngOnDestroy(): void {
    this.machineRefreshSubscription?.unsubscribe();
  }
}
