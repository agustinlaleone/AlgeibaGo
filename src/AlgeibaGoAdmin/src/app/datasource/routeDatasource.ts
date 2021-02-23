import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, Observable, of } from "rxjs";
import { catchError, finalize } from "rxjs/operators";
import { ApiService } from '../services/api.service';
import { UsersByRoute } from '../models/usersbyroute.model';

export class RoutesDataSource implements DataSource<UsersByRoute> {

    private routesSubject = new BehaviorSubject<UsersByRoute[]>([]);
    private loadingSubject = new BehaviorSubject<boolean>(false);
    public partialRouteCount: number;
    public loading$ = this.loadingSubject.asObservable();
    public listAll = [];

    constructor(private service: ApiService) { }

    connect(collectionViewer: CollectionViewer): Observable<UsersByRoute[]> {
        return this.routesSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.routesSubject.complete();
        this.loadingSubject.complete();
    }

    getRoutes(userId, role, filter, sortOrder, pageIndex, pageSize, method) {
        return this.service.findRoutes(userId, role, filter, sortOrder, pageIndex, pageSize, method);
    }

    loadRoutes(userId, role, filter, sortOrder, pageIndex, pageSize, method) {
        this.loadingSubject.next(true);
        this.service.findRoutes(userId, role, filter, sortOrder, pageIndex, pageSize, method)
            .pipe(
                catchError(() => of([])),
                finalize(() => this.loadingSubject.next(false))
            )
            .subscribe(routes => { this.listAll = routes, this.routesSubject.next(routes) });
    }
}