using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username cannot be blank!")]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long!")]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name cannot be blank!")]
        [MinLength(2, ErrorMessage = "First Name must be at least 2 characters long!")]
        [RegularExpression(@"^[^0-9\s]+$*$", ErrorMessage = "No numbers and no spaces in First Name!")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name cannot be blank!")]
        [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters long!")]
        [RegularExpression(@"^[^0-9\s]+$*$", ErrorMessage = "No numbers and no spaces in Last Name!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password cannot be blank!")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long!")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Wallet { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        [Compare("Password", ErrorMessage = "The passwords entered do not match.")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }

        public List<Bid> Bids { get; set; }
    }
    
}