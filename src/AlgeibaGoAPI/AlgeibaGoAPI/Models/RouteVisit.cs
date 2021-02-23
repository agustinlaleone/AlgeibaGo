using System;

namespace AlgeibaGoAPI.Models
{
    public partial class RouteVisit
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime VisitDate { get; set; }

        public Routes Route { get; set; }

        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string Device { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string UserAgent { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string IPData { get; set; }

        public  RouteVisit()
        {
            this.Browser = null;
            this.BrowserVersion = null;
            this.Device = null;
            this.OS = null;
            this.OSVersion = null;
            this.UserAgent = null;
            this.City = null;
            this.Region = null;
            this.Country = null;
            this.IPData = null;
        }
    }
    public partial class IPInfo
    {
        public string city { get; set; }
        public string regionName { get; set; }
        public string country { get; set; }
        public IPInfo()
        {
            this.city = null;
            this.regionName = null;
            this.country = null;
        }
    }
}
