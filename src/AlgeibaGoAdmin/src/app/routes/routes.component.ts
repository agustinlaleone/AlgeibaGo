import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { RoutesDataSource } from '../datasource/routeDatasource';
import { ApiService } from '../services/api.service';
import { Observable, Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Route } from '../models/route.model';
import { ToastrService } from 'ngx-toastr';
import { CustomValidator } from '../shared/validator';
import { RelRouteUser } from '../models/relrouteuser.model';
import { UsersByRoute } from '../models/usersbyroute.model';
import { Router, Routes } from '@angular/router';
import { enableRipple } from "@syncfusion/ej2-base";
import { ListViewComponent } from '@syncfusion/ej2-angular-lists';
import { ChartType, ChartOptions } from 'chart.js';
import { Label } from 'ng2-charts';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';
import { Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { AppConfigService } from '../services/AppConfigService';
import { RouteDataChart } from '../models/dataChart';

enableRipple(true);

@Component({
  selector: 'app-routes',
  templateUrl: './routes.component.html',
  styleUrls: ['./routes.component.css']
})

export class RoutesComponent implements OnInit, AfterViewInit {
  urlLocation: string;
  constructor(private environment: AppConfigService, private toastrService: ToastrService, private appConfigService: AppConfigService, private formBuilder: FormBuilder, private dataService: ApiService, private modalService: NgbModal, private router: Router, @Inject(DOCUMENT) document) {
    this.urlLocation = environment.config.pageURL + '/';
  }

  search = "";
  datachart: RouteDataChart[];
  objRoute: UsersByRoute;
  relRouteUser: RelRouteUser;
  form: FormGroup;
  editedRute: boolean;
  habMyLinks: boolean;
  currentSelectedPage: number;
  submitted = false;
  modalBody: String;
  modalTitle: String;
  btnMisLinkBorde: boolean;
  btnOtrosLinkBorde: boolean;
  btnMisFavoritos: boolean;
  roles: string[];
  role: string;
  selectedRoute: Route;
  routeList: any[];
  multipleRole = false;
  activeButtons = false;
  ownLink = false;
  countVisit: 0;
  countRoutes = 0;
  public data: Object[];
  dataSource: RoutesDataSource;
  displayedColumns: string[] = ['Route', 'RedirectUrl', 'VisitCount', 'actionsColumn'];
  totalSize: number;
  viewVisited: boolean;
  viewCountry: boolean;
  sizeOptions = [15, 25, 50];
  method = 1;

  urlProtocol: string;
  p: number = 1;
  @ViewChild('inputFilter') inputFilter: ElementRef;
  @ViewChild('modalRoute', { static: true }) modalRoute;
  @ViewChild('modalConfirm', { static: true }) modalConfirm;
  @ViewChild('list')
  listObj: ListViewComponent;
  @ViewChild('box', { static: true }) textboxEle: any;
  private modalRef;
  isAdmin: boolean;
  public userId: string;
  tempMyData: any;

  ngOnInit() {


    this.btnMisLinkBorde = true;
    this.btnOtrosLinkBorde = false;
    this.btnMisFavoritos = false;
    this.viewVisited = false;
    this.viewCountry = false;
    this.role = "ROLES";
    this.selectedRoute = new Route();
    this.habMyLinks = true;
    this.editedRute = false;
    this.setRolesButtonByUser();
    this.form = this.formBuilder.group({
      route: ['', Validators.required],
      title: [''],
      redirectUrl: ['', [Validators.required, CustomValidator.urlValidator]]
    });
    this.relRouteUser = new RelRouteUser();
    this.dataSource = new RoutesDataSource(this.dataService);
    this.selectedMyLinks(2);

    this.userId = sessionStorage.getItem('userId');

  }

  selectedMyLinks(n) {
    this.activeButtons = false;
    this.habMyLinks = true;

    if (n == 2) {
      this.viewCountry = true;
      this.viewVisited = true;
      this.btnMisLinkBorde = true;
      this.btnMisFavoritos = false;
      this.btnOtrosLinkBorde = false;
      this.ownLink = true;
    }
    else if (n == 3) {
      this.viewCountry = true;
      this.viewVisited = false;
      this.btnMisLinkBorde = false;
      this.btnMisFavoritos = false;
      this.btnOtrosLinkBorde = true;
      this.ownLink = false;
    } else if (n == 4) {
      this.viewCountry = true;
      this.viewVisited = false;
      this.btnMisLinkBorde = false;
      this.btnMisFavoritos = true;
      this.btnOtrosLinkBorde = false;
      this.ownLink = false;
    }
    this.currentSelectedPage = n;
    this.p = 1;
    this.getAllRoutes(n);
  }

  getAllRoutes(n) {
    this.dataSource.getRoutes(sessionStorage.getItem('userId'), sessionStorage.getItem('role'), '', 'asc', 0, 100000, n)
      .subscribe(x => {
        this.data = x;
        this.countRoutes = this.data.length;
        if (n == 2) this.tempMyData = this.data;
      });
  }

  stringNullOrEmpty(s: string) {
    return s === null || s === '' || s === undefined;
  }


  selectOption(e) {
    this.countVisit = 0;
    this.activeButtons = true;
    this.selectedRoute = e;
    this.countVisit = e.VisitCount;
    this.getChartData(this.selectedRoute.Id);
  }

  ngAfterViewInit() {
  }

  public pieChartOptions: ChartOptions;
  public pieChartLabelsCountry: Label[] = [];
  public pieChartDataCountry: number[] = [];
  public pieChartLabelsClick: Label[] = [];
  public pieChartDataClick: number[] = [];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [pluginDataLabels];
  public chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }
  public pieChartColors = [
    {
      backgroundColor: ['rgba(255,0,255,0.3)', 'rgba(0,255,0,0.3)'],
    },
  ];
  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }

  getChartData(id: number) {
    if (this.viewCountry) {
      this.pieChartLabelsCountry = [];
      this.pieChartDataCountry = [];
      this.pieChartLabelsClick = [];
      this.pieChartDataClick = [];
      this.dataService.getDataChartCountry(id).subscribe(res => {
        this.datachart = res;
        this.datachart.forEach(element => {
          this.pieChartLabelsCountry.push(element.name);
          this.pieChartDataCountry.push(element.y);
        });
        this.createChart();
      }, error => {
      });
    }
    if (this.viewVisited) {
      var sendRel = new RelRouteUser();
      sendRel.IdRelation = 0;
      sendRel.IdRoute = id;
      sendRel.IdUser = sessionStorage.getItem('userId');
      this.dataService.getDataChartClicks(sendRel).subscribe(res => {
        this.datachart = res;
        this.datachart.forEach(element => {
          this.pieChartLabelsClick.push(element.name);
          this.pieChartDataClick.push(element.y);
        });
        this.createChart();
      }, error => {
      });
    }


  }
  createChart() {
    this.pieChartOptions = {
      responsive: true,
      legend: {
        position: 'top',
      },
      plugins: {
        datalabels: {
          formatter: (value, ctx) => {
            const label = ctx.chart.data.labels[ctx.dataIndex];
            return label;
          },
        },
      }
    };
    this.pieChartType = 'pie';
  }

  loadRoutesPage(selectMethod) {
    if (selectMethod != 0) {
      this.method = selectMethod;
    }
    this.dataSource = new RoutesDataSource(this.dataService);
    this.dataSource.loadRoutes(
      sessionStorage.getItem('userId'),
      sessionStorage.getItem('role'),
      '',
      'asc',
      0,
      0,
      this.method);
    this.getRoutesCount(this.inputFilter.nativeElement.value, this.method);
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

  get formControls() { return this.form.controls; }

  getRandomRouteName(lenght: number): string {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < lenght; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
  }

  newRoute(modal, num) {
    this.editedRute = false;
    var routeName = this.getRandomRouteName(10);
    this.submitted = false;

    this.form = this.formBuilder.group({
      route: [routeName, Validators.required],
      title: [''],
      redirectUrl: ['', [Validators.required, CustomValidator.urlValidator]]
    });

    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title', backdrop: 'static', keyboard: false }).result.then((result) => {
      this.selectedMyLinks(num);
    }, (reason) => {
      this.selectedMyLinks(num);
    });
  }

  closeModal() {
    this.modalService.dismissAll();
  }

  editRoute(route, modal) {
    this.editedRute = true;
    this.submitted = false;
    this.objRoute = route;
    this.form = this.formBuilder.group({
      title: [this.objRoute.PageTitle],
      route: [this.objRoute.Route, Validators.required],
      redirectUrl: [this.objRoute.RedirectUrl, [Validators.required, CustomValidator.urlValidator]]
    });
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title', backdrop: 'static', keyboard: false }).result.then((result) => {
    });
  }

  deleteRoute(route, modal, num) {
    this.modalTitle = "Eliminar";
    this.modalBody = "¿Desea eliminar el registro?";
    debugger;
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      debugger;
      if (result == "OK") {
        debugger;
        if (route.VisitCount > 0) {
          this.modalTitle = "Confirmar";
          this.modalBody = "Si acepta, todos los registros de visitas también serán borrados";
        }
          this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
            if (result == "OK") {
              this.dataService.deleteRelRouteUser(route).subscribe((x) => {
                this.toastrService.success('La relaciones han sido eliminadas con éxito!', "Eliminar");
                this.dataService.deleteRoute(route).subscribe((x) => {
                  this.toastrService.success('La ruta ha sido eliminada con éxito!', "Eliminar");
                  this.modalService.dismissAll();
                  this.selectedMyLinks(num);
                }, error => {
                  this.toastrService.error(error.error, 'Error');
                });
                this.selectedMyLinks(num);
              });
             
            }
          });
        
      }
    });
  }

  changeStatus(route, modal) {
    var stringAdvise = route.Status ? "deshabilitada" : "habilitada";
    this.modalTitle = route.Status ? "Deshabilitar" : "Habilitar";
    this.modalBody = route.Status ? "¿Desea deshabilitar el registro?" : "¿Desea habilitar el registro?";
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {

      if (result == "OK") {
        route.Status = !route.Status;
        this.dataService.putRoute(route).subscribe((x) => {
          this.toastrService.success('La ruta ha sido ' + stringAdvise + ' con éxito!', this.modalTitle.toString());
          this.modalService.dismissAll();
        }, error => {
          this.toastrService.error(error.error, 'Error');
        });
      }
    });
  }

  saveRoute() {
    this.submitted = true;

    if (this.form.invalid) {
      return;
    }

    if (this.objRoute == null) {
      this.objRoute = new UsersByRoute();
      this.objRoute.Id = 0;
      this.objRoute.Status = true;
    }

    this.objRoute.Route = this.form.controls["route"].value;
    this.objRoute.RedirectUrl = this.form.controls["redirectUrl"].value;
    this.objRoute.PageTitle = this.form.controls["title"].value;

    if (this.objRoute.Id == 0) { // ALTA
      this.dataService.postRoute(this.objRoute).subscribe((x: Route) => {
        if (x.Id != 0) {
          this.relRouteUser.IdRelation = 0;
          this.relRouteUser.IdRoute = x.Id;
          this.relRouteUser.IdUser = sessionStorage.getItem('userId');
        }
        this.dataService.postRelRouteUser(this.relRouteUser).subscribe((x) => {
          this.toastrService.success('La relacion de ruta ha sido creada con éxito!', 'Alta Relacion');
          this.selectedMyLinks(this.currentSelectedPage);
        });
        this.toastrService.success('La ruta ha sido creada con éxito!', 'Alta');
        this.modalService.dismissAll();
        this.totalSize++;
      }, error => {
        this.toastrService.error(error.error, 'Error');
      });
    }
    else {
      this.dataService.putRoute(this.objRoute).subscribe((x) => {
        this.toastrService.success('La ruta ha sido modificada con éxito!', 'Modificación');
        this.modalService.dismissAll();
        this.selectedMyLinks(this.currentSelectedPage);

      }, error => {
        this.toastrService.error(error.error, 'Error');
      });
    }
  }

  setRolesButtonByUser() {
    var userId = sessionStorage.getItem('userId');
    this.role = sessionStorage.getItem('role');
    this.dataService.getUserRoles(userId).subscribe(rest => {
      this.roles = rest;
      if (this.roles.length > 1) {
        this.multipleRole = true;
      }

      this.isAdmin = (this.roles.filter(function (item) {
        return item == "Admin";
      }).length > 0) ? true : false;
    }, error => {
      this.toastrService.error("Hubo un problema al cargar los roles de usuario.");
    }
    );
  }

  selected(row: string) {
    this.role = row;
    sessionStorage.setItem('role', this.role);
    this.dataSource = new RoutesDataSource(this.dataService);
    this.relRouteUser = new RelRouteUser();
    this.loadRoutesPage(this.method);
    this.getRoutesCount(this.inputFilter.nativeElement.value, this.method);
  }

  redirectRoutes(dir: number) {
    if (dir == 1) {
      this.router.navigate(["relations"])
    } else {
      this.router.navigate(["roles"])
    }
  }

  changePage(event) {
    this.p = event;
  }

  containsUser(route: UsersByRoute) {
    var contains = (this.tempMyData != null && this.tempMyData.filter(function (item) {
      return item.Id == route.Id;
    }).length > 0);

    return contains;
  }

  updateFavorite(route: Route) {
    var sendRel = new RelRouteUser();
    sendRel.IdRelation = 0;
    sendRel.IdRoute = route.Id;
    sendRel.IdUser = sessionStorage.getItem('userId');
    this.dataService.updateFavorites(sendRel).subscribe(res => {
      if (res) {
        this.toastrService.success('La ruta se ha agregado a favoritos.');
        route.isFavorite = true;
      } else {
        this.toastrService.success('La ruta se ha eliminado de favoritos.');
        route.isFavorite = false;

        if (this.currentSelectedPage == 4) {
          this.activeButtons = false;
          this.selectedMyLinks(this.currentSelectedPage);
        }
      }
    }, error => {
      this.toastrService.error("Hubo un problema al modificar favoritos.");
    });
  }
}