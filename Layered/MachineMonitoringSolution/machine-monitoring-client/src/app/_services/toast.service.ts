import { Injectable } from '@angular/core';
import { ToastRef, ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  constructor(private toastr: ToastrService) {}

  showSuccess(message: string, title?: string): void {
    this.toastr.success(message);
  }
}
