using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Auctions.Models
{
    public class LoginUser
    {
        [NotMapped]
        [Required(ErrorMessage = "Username cannot be blank!")]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long!")]
        [MaxLength(20)]
        public string LoginUsername { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password cannot be blank!")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string LoginPassword { get; set; }
    }
}