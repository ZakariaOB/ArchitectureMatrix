import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MachinePageComponent } from './machine-page/machine-page.component';
import { MachineDashboardComponent } from './machine-dashboard/machine-dashboard.component';
import { HTTP_INTERCEPTORS, HttpInterceptor, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { SpinnerComponent } from './spinner/spinner.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';

@NgModule({ declarations: [
        AppComponent,
        MachinePageComponent,
        MachineDashboardComponent,
        SpinnerComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule,
        NgxSpinnerModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot()], providers: [{ provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }, provideHttpClient(withInterceptorsFromDi())] })
export class AppModule { }
