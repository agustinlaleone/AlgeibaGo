using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace AlgeibaGoAPI.Models
{
    public partial class UsersByRoutes
    {
        public UsersByRoutes()
        {
            RouteVisit = new HashSet<RouteVisit>();
        }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Route")]
        public string Route { get; set; }

        [JsonProperty(PropertyName = "RedirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty(PropertyName = "PageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public bool? Status { get; set; }

        [JsonProperty(PropertyName = "Exists")]
        public bool? Exists { get; set; }

        [JsonProperty(PropertyName = "isFavorite")]
        public bool? isFavorite { get; set; }

        [JsonProperty(PropertyName = "VisitCount")]
        public long VisitCount
        {
            get
            {
                return RouteVisit != null ? RouteVisit.Count : 0;
            }
        }
        [JsonProperty(PropertyName = "Users")]
        public List<User> Users { get; set; }

        [IgnoreDataMember]
        public ICollection<RelRouteUser> RelRouteUser { get; set; }

        [IgnoreDataMember]
        public ICollection<RouteVisit> RouteVisit { get; set; }
    }
}
