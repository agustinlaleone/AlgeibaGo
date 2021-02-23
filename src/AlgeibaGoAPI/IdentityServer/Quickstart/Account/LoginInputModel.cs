using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class LoginInputModel
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Contraseña { get; set; }
        public string Email { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public string Error { get; set; }
        public bool? LoginFailure { get; set; } = null;
    }
}