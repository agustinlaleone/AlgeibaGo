export class User {
    Id: string;
    UserName: string;
    roles: string[];
    personName: string;
    personSurname: string;
    email: string;
    password: string;
    constructor() {
        this.Id = "0";
        this.UserName = "";
        this.roles = [];
        this.personName = "";
        this.personSurname = "";
        this.email = "";
        this.password = null;
    }
}