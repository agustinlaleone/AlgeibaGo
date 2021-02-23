export class ProfileUser {
    public given_name: string;
    public name: string;
    public preferred_username: string;
    public sub: string;
    public nombre: string;
    public apellido: string;
    public address: string;
    public email: string;
    public email_verified: string;
    public role: string[];

    constructor() {
        this.given_name = '';
        this.name = '';
        this.preferred_username = '';
        this.sub = '';
        this.nombre = '';
        this.apellido = '';
        this.address = '';
        this.email = '';
        this.email_verified = '';
        this.role = null;
    }
}


