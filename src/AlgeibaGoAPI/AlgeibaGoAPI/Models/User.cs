using System.ComponentModel.DataAnnotations.Schema;

namespace AlgeibaGoAPI.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        [NotMapped]
        public string[] Roles { get; set; }
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
