using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public Product Product { get; set; }
        
        public User User { get; set; }
    }
}