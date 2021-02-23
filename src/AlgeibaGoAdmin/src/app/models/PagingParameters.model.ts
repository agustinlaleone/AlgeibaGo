export class PagingParameters {
    pageIndex: number;
    pageSize: number;
    filter: string;
    pageTotal: number;
    user: string;
    role: string;
    constructor(){
        this.pageIndex = 1;
        this.pageSize = 5;
        this.filter = "a";
        this.pageTotal = 0;
        this.user = "";
        this.role = "";
    }
}