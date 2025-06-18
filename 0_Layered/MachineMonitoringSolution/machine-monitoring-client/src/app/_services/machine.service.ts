import { Injectable } from '@angular/core';
import { environment } from 'src/environnement/environment';
import { HttpClient } from '@angular/common/http';
import { Machine } from '../_models/machine';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MachineService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Machine[]> {
    return this.http.get<Machine[]>(this.baseUrl + 'machines');
  }

  getById(machineId: number): Observable<Machine> {
    return this.http.get<Machine>(this.baseUrl + 'machine/' + machineId);
  }

  delete(machineId: number) {
    return this.http.delete(this.baseUrl + 'machine/' + machineId);
  }

  getTotalProduction(machineId: number) {
    return this.http.delete(
      this.baseUrl + 'machine/totalproduction?id=' + machineId
    );
  }

  getForDashboard(machineId: number): Observable<Machine> {
    return this.http.get<Machine>(
      this.baseUrl + 'machine/dashboard/' + machineId
    );
  }
}
