using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServerAspNetIdentity.Models
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
        }

        public string Id { get; set; }
        public int AccessFailedCount { get; set; }
        public string ConcurrencyStamp { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        [StringLength(256)]
        public string NormalizedEmail { get; set; }
        [StringLength(256)]
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        [StringLength(256)]
        public string UserName { get; set; }
        [StringLength(50)]
        public string PersonName { get; set; }
        [StringLength(50)]
        public string PersonSurname { get; set; }

        [InverseProperty("User")]
        public ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        [InverseProperty("User")]
        public ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        [InverseProperty("User")]
        public ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        [InverseProperty("User")]
        public ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
    }
}
