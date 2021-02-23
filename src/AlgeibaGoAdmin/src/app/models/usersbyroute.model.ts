import { User } from './user.model';

export class UsersByRoute {
    Id: number;
    Route: string;
    isFavorite: boolean;
    RedirectUrl: string;
    VisitCount: number;
    Status: boolean;
    PageTitle: string;
    Users: User[];
    Exists: boolean;
}