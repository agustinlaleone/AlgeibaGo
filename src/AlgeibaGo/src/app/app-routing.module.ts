import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RedirectComponent } from './redirect/redirect.component';
import { HomeComponent } from './home/home.component';
import { ErrorsHandlerComponent } from './errors-handler/errors-handler.component';
import { ServererrorComponent } from './errors-handler/servererror/servererror.component';


const routes: Routes = [

  { path: '', component: HomeComponent },
  { path: 'error', component: ErrorsHandlerComponent },
  { path: 'servererror', component: ServererrorComponent },
  { path: ':string', component: RedirectComponent },
  { path: '**', component: ErrorsHandlerComponent }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
