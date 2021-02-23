import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoutesComponent } from './routes/routes.component';
import { AuthGuard } from './services/auth.guard';
import { CallbackComponent } from './callback/callback.component';
import { RelationsComponent } from './relations/relations.component';
import { RolesComponent } from './roles/roles.component';

const routes: Routes = [
  { path: '', canActivate: [AuthGuard], component: RoutesComponent },
  { path: 'callback', component: CallbackComponent },
  { path: 'relations', canActivate: [AuthGuard], component: RelationsComponent},
  { path: 'roles', canActivate: [AuthGuard], component: RolesComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
