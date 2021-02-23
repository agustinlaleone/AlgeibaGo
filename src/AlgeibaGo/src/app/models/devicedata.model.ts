export class DeviceData {

    browser: string;
    browserVersion: string;
    device: string;
    os: string;
    osVersion: string;
    userAgent: string;
    city: string;
    region: string;
    country: String;
    ipData: String;
    routeId: number;

    constructor() {
        this.browser = null;
        this.browserVersion = null;
        this.device = null;
        this.os = null;
        this.osVersion = null;
        this.userAgent = null;
        this.city = null;
        this.region = null;
        this.country = null;
        this.ipData = null;
        this.routeId = null;
    }
}