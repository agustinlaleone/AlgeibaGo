import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RoutesComponent } from './routes/routes.component';
import { RelationsComponent } from './relations/relations.component';
import { AppConfigService } from './services/AppConfigService';
import { ModalModule } from 'ngb-modal';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NavbarComponent } from './navbar/navbar.component';
import { OAuthModule, AuthConfig } from 'angular-oauth2-oidc';
import { CallbackComponent } from './callback/callback.component';
import { UserService } from './services/user.service';
import { AuthGuard } from './services/auth.guard';
import { FooterComponent } from './footer/footer.component';
import { AngularDualListBoxModule } from 'angular-dual-listbox';
import { RolesComponent } from './roles/roles.component';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { CookieService } from 'ngx-cookie-service';
import { ListViewModule } from '@syncfusion/ej2-angular-lists';
import { PagerModule } from '@syncfusion/ej2-angular-grids';
import { ClipboardModule } from 'ngx-clipboard';
import {NgxPaginationModule} from 'ngx-pagination';
import { ChartsModule } from 'ng2-charts';

import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FilterPipe } from './shared/filter';

const appInitializerFn = (appConfig: AppConfigService) => {
  return () => {
    return appConfig.loadAppConfig();
  }
};

@NgModule({
  declarations: [
    AppComponent,
    RoutesComponent,
    NavbarComponent,
    CallbackComponent,
    FooterComponent,
    RelationsComponent,
    RolesComponent,
    FilterPipe
  ],
  imports: [
    ChartsModule,
    NgbModule,
    OAuthModule.forRoot(),
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    FontAwesomeModule,
    ModalModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    ReactiveFormsModule,
    NgxDatatableModule,
    FormsModule,
    HttpClientModule,
    AngularDualListBoxModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    PagerModule,
    MatDatepickerModule,
    MatDialogModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatTableModule,
    MatTabsModule,
    ListViewModule,
    MatToolbarModule,
    MatTooltipModule,
    MatStepperModule,
    MatFormFieldModule,
    ClipboardModule,
    NgxPaginationModule,
    FontAwesomeModule,
    
   // IgxListModule,

  ],
  providers: [
    UserService,
    AuthGuard,
    CookieService, 
    AppConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFn,
      multi: true,
      deps: [AppConfigService]
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
