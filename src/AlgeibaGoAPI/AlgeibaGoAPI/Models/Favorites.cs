using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AlgeibaGoAPI.Models
{
    public class Favorites
    {
        public Favorites()
        {
        }
        public int IdFavorites { get; set; }
        public bool isFavorite { get; set; }
        public int? IdRoute { get; set; }
        public string IdUser { get; set; }

        public Routes IdRouteNavigation { get; set; }
        public AspNetUsers IdUserNavigation { get; set; }
    }
}
