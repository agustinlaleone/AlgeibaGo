import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, Observable, of } from "rxjs";
import { catchError, finalize } from "rxjs/operators";
import { ApiService } from '../services/api.service';
import { User } from '../models/user.model';

export class UsersDataSource implements DataSource<User> {

    private lessonsSubject = new BehaviorSubject<User[]>([]);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();

    constructor(private service: ApiService) { }

    connect(collectionViewer: CollectionViewer): Observable<User[]> {
        return this.lessonsSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.lessonsSubject.complete();
        this.loadingSubject.complete();
    }

    loadUsers(userId, role, filter, sortOrder, pageIndex, pageSize) {
        this.loadingSubject.next(true);
        this.service.findUsers(userId, role, filter, sortOrder, pageIndex, pageSize)
        .pipe(
            catchError(() => of([])),
            finalize(() => this.loadingSubject.next(false))
        )
        .subscribe(users => 
            { 
                users.forEach(user => {
                    this.service.getUserRoles(user.id).subscribe( roles => {
                        user.roles = roles;
                    });
                })
                this.lessonsSubject.next(users)});
    } 
}