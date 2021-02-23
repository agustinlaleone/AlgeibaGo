import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../services/api.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MatPaginator } from '@angular/material/paginator';
import { Subject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CustomValidator } from '../shared/validator';
import { UsersDataSource } from '../datasource/userDataSource';
import { User } from '../models/user.model';


@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {

  constructor(private toastrService: ToastrService, private formBuilder: FormBuilder, private dataService: ApiService,
    private modalService: NgbModal, private router: Router) {
    this.createForm();
  }
  isNew: boolean;
  objUser: User;
  form: FormGroup;
  formPassword: FormGroup;
  submitted = false;
  dataSource2: string[];
  modalBody: String;
  modalTitle: String;
  roles: string[];
  role: string;
  allRoles: string[];
  userRoles: string[];
  multipleRole = false;
  isAdmin: boolean;

  //Table variables
  dataSource: UsersDataSource;
  displayedColumns: string[] = ['Users', 'Roles', 'actionsColumn'];
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  totalSize: number;
  sizeOptions = [15, 25, 50];

  @ViewChild('inputFilter', { static: true }) inputFilter: ElementRef;
  @ViewChild('modalRoute', { static: true }) modalRoute;
  @ViewChild('modalConfirm', { static: true }) modalConfirm;

  ngOnInit() {
    this.role = "ROLES";
    this.dataService.getAllRoles().subscribe(rest => {
      this.allRoles = rest;
    });
    this.setRolesButtonByUser();

    this.dataSource = new UsersDataSource(this.dataService);
    this.paginator.pageSize = 15;
    this.dataSource.loadUsers(sessionStorage.getItem('userId'), sessionStorage.getItem('role'), '', 'asc', 0, this.paginator.pageSize);
    this.getUsersCount(this.inputFilter.nativeElement.value);
    this.isAdmin = (this.role == "Admin") ? true : false;
  }

  ngAfterViewInit() {
    this.paginator.page
      .pipe(
        tap(() => this.loadUsersPage(''))
      )
      .subscribe();
  }

  search(){
    this.loadUsersPage(this.inputFilter.nativeElement.value);
  }

  createForm() {
    this.form = this.formBuilder.group({
      username: ['', Validators.required],
      personname: ['', [Validators.required]],
      personsurname: ['', [Validators.required]],
      email: ['', [Validators.required]],
      roles: [''],
      password: ['']
    });
    this.formPassword = this.formBuilder.group({
      password: ['', Validators.required],
      repitpassword: ['', [Validators.required]]
    });
  }
  getUsersCount(filter): Observable<number> {
    let result: number;
    var subject = new Subject<number>();
    this.dataService.getUsersCount(filter).subscribe(
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

  loadUsersPage(filter) {
    this.dataSource.loadUsers(
      sessionStorage.getItem('userId'),
      sessionStorage.getItem('role'),
      filter,
      'asc',
      this.paginator.pageIndex,
      this.paginator.pageSize);
  }

  get formControls() { return this.form.controls; }

  newUser(modal) {
    this.isNew = true;
    this.objUser = new User();
    this.objUser.roles = [""];
    this.userRoles = this.objUser.roles;
    this.submitted = false;
    this.form = this.formBuilder.group({
      username: [this.objUser.UserName, Validators.required],
      roles: [this.objUser.roles, [Validators.required, CustomValidator.urlValidator]],
      personname: [this.objUser.personName, Validators.required],
      personsurname: [this.objUser.personSurname, Validators.required],
      email: [this.objUser.email, Validators.required],
      password: [this.objUser.password, Validators.required]
    });

    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title', backdrop: 'static', keyboard: false }).result.then((result) => {

    }, (reason) => {

    });
  }

  closeModal() {
    this.modalService.dismissAll();
  }

  editRoles(user, modal) {
    this.isNew = false;
    this.submitted = false;
    this.objUser = new User();
    this.objUser.UserName = user.userName;
    this.objUser.Id = user.id;
    this.objUser.roles = user.roles;
    this.objUser.email = user.email;
    this.objUser.personName = user.personName;
    this.objUser.personSurname = user.personSurname;
    this.userRoles = this.objUser.roles;
    this.form = this.formBuilder.group({
      username: [this.objUser.UserName, Validators.required],
      roles: [this.objUser.roles, [Validators.required, CustomValidator.urlValidator]],
      personname: [this.objUser.personName, Validators.required],
      personsurname: [this.objUser.personSurname, Validators.required],
      email: [this.objUser.email, Validators.required],
      password: [this.objUser.password, Validators.required]
    });
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title', backdrop: 'static', keyboard: false }).result.then((result) => {
    });
  }

  saveRoles() {
    this.submitted = true;
    if (this.userRoles.length > 0) {
      this.objUser.UserName = this.form.get('username').value;
      this.objUser.email = this.form.get('email').value;
      if (this.isNew) {
        this.objUser.password = this.form.get('password').value;
      }
      this.objUser.personName = this.form.get('personname').value;
      this.objUser.personSurname = this.form.get('personsurname').value;
      if (this.isNew) {
        this.dataService.addUser(this.objUser).subscribe((x) => {
          this.toastrService.success('Los roles se han modificado con éxito!');
          this.modalService.dismissAll();
          this.dataSource = new UsersDataSource(this.dataService);
          this.loadUsersPage('');
          this.getUsersCount(this.inputFilter.nativeElement.value);
        }, error => {
          this.toastrService.error(error.error, 'Problema interno del servidor, intente de nuevo más tarde.')
        }
        );
      }
      else {
        this.dataService.updateUser(this.objUser).subscribe((x) => {
          this.toastrService.success('Los roles se han modificado con éxito!');
          this.modalService.dismissAll();
          this.dataSource = new UsersDataSource(this.dataService);
          this.loadUsersPage('');
          this.getUsersCount(this.inputFilter.nativeElement.value);
        }, error => {
          this.toastrService.error(error.error, 'El usuario está relacionado a Rutas, no se puede eliminar')
        }
        );
      }
    } else {
      this.toastrService.error('Error, debe haber al menos un rol')
    }

  }

  deleteUser(user, modal) {

    this.modalTitle = "Eliminar";
    this.modalBody = "¿Desea eliminar el usuario?";
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      if (result == "OK") {
        this.dataService.deleteUser(user.id).subscribe((x) => {
          this.toastrService.success('El usuario ha sido eliminado con éxito!', "Eliminar");
          this.modalService.dismissAll();
          this.paginator.pageIndex = 0;
          this.totalSize--;
          this.loadUsersPage('');
        }, error => {
          this.toastrService.error(error.error, 'Error');
        });
      }
    });
  }

  updatePassword(user, modal) {
    this.objUser = new User();
    this.objUser.Id = user.id;
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      if (result == "OK") {
        if (this.formPassword.get('password').value == this.formPassword.get('repitpassword').value) {
          var password = this.formPassword.get('password').value;
          this.objUser.password = password;
        } else {
          this.toastrService.error('Las contraseñas no son iguales. Intente de nuevo');
        }
        this.dataService.changePassword(this.objUser).subscribe((x) => {
          this.toastrService.success('La contraseña a sido modificada con éxito.');
          this.modalService.dismissAll();
          this.paginator.pageIndex = 0;
          this.loadUsersPage('');
        }, error => {
          this.toastrService.error(error.error, 'Error');
        });
      }
    });
  }

  setRolesButtonByUser() {
    var userId = sessionStorage.getItem('userId');
    this.role = sessionStorage.getItem('role');
    this.dataService.getUserRoles(userId).subscribe(rest => {
      this.roles = rest;
      if (this.roles.length > 1) {
        this.multipleRole = true;
      }
    }, error => {
      this.toastrService.error("Hubo un problema al cargar los roles de usuario.");
    });
  }

  selected(row: string) {
    this.role = row;
    sessionStorage.setItem('role', this.role);
    this.isAdmin = (this.role == "Admin") ? true : false;
    this.dataSource = new UsersDataSource(this.dataService);
    this.loadUsersPage('');
    this.getUsersCount(this.inputFilter.nativeElement.value);
  }

  redirectRoutes(dir: number) {
    if (dir == 1) {
      this.router.navigate([""])
    } else {
      this.router.navigate(["relations"])
    }
  }
}
