using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AlgeibaGoAPI.Models
{
    public partial class Routes
    {
        public Routes()
        {
            RouteVisit = new HashSet<RouteVisit>();
            FavoriteRoute = new HashSet<Favorites>();
        }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Route")]
        public string Route { get; set; }

        [JsonProperty(PropertyName = "RedirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public bool? Status { get; set; }

        [JsonProperty(PropertyName = "PageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty(PropertyName = "VisitCount")]
        public long VisitCount
        {
            get
            {
                return RouteVisit != null ? RouteVisit.Count : 0;
            }
        }

        [IgnoreDataMember]
        public ICollection<RelRouteUser> RelRouteUser { get; set; }

        [IgnoreDataMember]
        public ICollection<Favorites> FavoriteRoute { get; set; }

        [IgnoreDataMember]
        public ICollection<RouteVisit> RouteVisit { get; set; }
    }
}
