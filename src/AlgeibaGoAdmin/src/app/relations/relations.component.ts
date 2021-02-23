import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { RoutesDataSource } from '../datasource/routeDatasource';
import { ApiService } from '../services/api.service';
import { Observable, Subject, fromEvent, merge } from 'rxjs';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ModalManager } from 'ngb-modal';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators, FormArray, FormControl, ValidatorFn } from '@angular/forms';
import { Route } from '../models/route.model';
import { ToastrService } from 'ngx-toastr';
import { CustomValidator } from '../shared/validator';
import { NavbarComponent } from '../navbar/navbar.component';
import { RelRouteUser } from '../models/relrouteuser.model';
import { UsersByRoute } from '../models/usersbyroute.model';
import { Router } from '@angular/router';
import { User } from '../models/user.model';

@Component({
  selector: 'app-relations',
  templateUrl: './relations.component.html',
  styleUrls: ['./relations.component.css']  
})
export class RelationsComponent implements OnInit, AfterViewInit {

  constructor(private toastrService: ToastrService, private formBuilder: FormBuilder, private dataService: ApiService, private modalService: NgbModal, private router: Router) { }

  objRoute: UsersByRoute;
  relRouteUser: RelRouteUser;
  relations: RelRouteUser[];
  form: FormGroup;
  submitted = false;
  modalBody: String;
  modalTitle: String;
  roles: string[];
  role: string;
  listUsers: User[];
  listUsersSelected: User[];
  multipleRole = false;

  // table variables
  dataSource: RoutesDataSource;
  displayedColumns: string[] = ['Route', 'RedirectUrl', 'Users', 'actionsColumn'];
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  totalSize: number;
  sizeOptions = [10, 15, 25, 50];
  method = 1;
  
  @ViewChild('inputFilter', { static: true }) inputFilter: ElementRef;
  @ViewChild('modalRelation', { static: true }) modalRelation;
  @ViewChild('modalConfirm', { static: true }) modalConfirm;
  private modalRef;

  ngOnInit() {
    this.role = "ROLES";
    this.setRolesButtonByUser();
    this.form = this.formBuilder.group({
      route: [''],
      redirectUrl: [''],
      users: ['']
    });
    this.dataSource = new RoutesDataSource(this.dataService);
    this.relRouteUser = new RelRouteUser();
    this.paginator.pageSize = 10;
    this.dataSource.loadRoutes(sessionStorage.getItem('userId'), sessionStorage.getItem('role'),  '', 'asc', 0, this.paginator.pageSize, this.method);
    this.getRoutesCount(this.inputFilter.nativeElement.value, this.method);
    this.listUsersSelected = [];
  }

  ngAfterViewInit() {
    this.paginator.page
            .pipe(
                tap(() => this.loadRoutesPage(this.method))
            )
            .subscribe();
  }

  // Returns Routes count from DB to make paging
  getRoutesCount(filter, method): Observable<number> {
    let result: number;
    var subject = new Subject<number>();
    this.dataService.getRoutesCount(filter, method).subscribe(
      count => {
        result = count;
        this.totalSize = count;
        subject.next(result);
      },
      error => {
        return 0;
      }
    );

    return subject.asObservable();
  }

  loadRoutesPage(selectMethod) {
    if(selectMethod != 0){
      this.method = selectMethod;
    }
    this.dataSource.loadRoutes(
      sessionStorage.getItem('userId'),
      sessionStorage.getItem('role'),
      '',
      'asc',
      this.paginator.pageIndex,
      this.paginator.pageSize, 
      this.method);
      this.getRoutesCount(this.inputFilter.nativeElement.value, this.method);
  }

  get formControls() { return this.form.controls; }

  closeModal() {
    this.modalService.dismissAll();
    this.loadRoutesPage(this.method);
  }

  editRelation(route, modal) {
    this.submitted = false;
    this.objRoute = route;
    this.listUsersSelected= this.objRoute.Users;
    this.dataService.getAllUsers().subscribe(res =>{
      this.listUsers = res;
    });

    this.form = this.formBuilder.group({
      route: [this.objRoute.Route],
      redirectUrl: [this.objRoute.RedirectUrl],
      users: [this.objRoute.Users]
    });
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title', backdrop: 'static', keyboard: false }).result.then((result) => {
    });
  }

  saveRelation() {
    this.submitted = true;
    if (this.form.invalid) {
      return;
    }
    if (this.listUsersSelected.length > 0){
      this.dataService.saveRelations(this.listUsersSelected,this.objRoute.Id).subscribe((x) =>{
        this.toastrService.success('La relacion de ruta ha sido creada con éxito!', 'Alta Relacion');
        this.modalService.dismissAll();
        this.loadRoutesPage(this.method);
      }, error => {
        this.toastrService.error('Error interno del servidor, intente de nuevo más tarde')
      }
      );
    }else
    {
      this.toastrService.error('Error, debe haber al menos un user')
    }

  }

  setRolesButtonByUser(){
    var userId = sessionStorage.getItem('userId');
    this.role = sessionStorage.getItem('role');
    this.dataService.getUserRoles(userId).subscribe( rest => {
      this.roles = rest;
      if(this.roles.length > 1)
      {
        this.multipleRole = true;
      }
    }, error => {
      this.toastrService.error("Hubo un problema al cargar los roles de usuario.");
    }
    );
  }

  selected(row: string){
    this.role = row;
    sessionStorage.setItem('role', this.role);
    this.dataSource = new RoutesDataSource(this.dataService);
    this.relRouteUser = new RelRouteUser();
    this.loadRoutesPage(this.method);
    this.getRoutesCount(this.inputFilter.nativeElement.value, this.method);
  }

  redirectRoutes(dir: number){
    if(dir == 1){
      this.router.navigate([""])
    } else {
      this.router.navigate(["roles"])
    }
  }
  
}
