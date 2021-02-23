using System;
using System.Collections.Generic;

namespace AlgeibaGoAPI.Models
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            RelRouteUser = new HashSet<RelRouteUser>();
            FavoriteUser = new HashSet<Favorites>();
        }

        public string Id { get; set; }
        public int AccessFailedCount { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string NormalizedEmail { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }

        public ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        public ICollection<RelRouteUser> RelRouteUser { get; set; }
        public ICollection<Favorites> FavoriteUser { get; set; }
    }
}
