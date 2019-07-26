using Microsoft.EntityFrameworkCore;
using Auctions.Models;


namespace Auctions.Models
{
    public class AuctionsContext : DbContext
    {
        public AuctionsContext(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=auctionsdb.db");
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Bid> Bids { get; set; }

    }
}