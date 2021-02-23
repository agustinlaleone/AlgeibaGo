export class Route {
    Id: number;
    Route: string;
    RedirectUrl: string;
    isFavorite: boolean;
    VisitCount: number;
    Status: boolean;
    PageTitle: string;
    constructor() {
        this.Route = "";
        this.RedirectUrl = "";
    }
}