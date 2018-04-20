using System.ComponentModel.DataAnnotations;

namespace BorBaNetCore.Models.AccountViewModels
{
    public class LoginViewModel
    {
       // [Required]
       // [EmailAddress]
        [Display(Name = "Guest")]
        public string Email { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Guest")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
