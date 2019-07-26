using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public int SellerId { get; set; }

        public User Seller { get; set; }

        [Required(ErrorMessage = "Product name cannot be blank!")]
        [MinLength(3)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product description cannot be blank!")]
        [MinLength(10)]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Starting Bid cannot be blank!")]
        [DataType(DataType.Currency)]
        public double StartBid { get; set; }

        [DataType(DataType.Currency)]
        public double HighestBid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime UpdatedAt { get; set; }

        public List<Bid> Bids { get; set; }
    }
}