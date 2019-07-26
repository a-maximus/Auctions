using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Auctions.Models;
using Newtonsoft.Json;



namespace Auctions.Controllers
{
    public class HomeController : Controller
    {
        private static AuctionsContext context;

        private PasswordHasher<User> RegisterHasher = new PasswordHasher<User>();

        private PasswordHasher<LoginUser> LoginHasher = new PasswordHasher<LoginUser>();
        public HomeController(AuctionsContext dbc)
        {
            context = dbc;
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User u)
        {
            if (ModelState.IsValid)
            {
                User checkName = context.Users.SingleOrDefault(us => us.Username == u.Username);

                if (checkName == null)
                {
                    PasswordHasher<User> RegisterHasher = new PasswordHasher<User>();
                    u.Password = RegisterHasher.HashPassword(u, u.Password);

                    u.Wallet = 1000.00;
                    u.CreatedAt = DateTime.Now;
                    u.UpdatedAt = DateTime.Now;
                    context.Users.Add(u);
                    context.SaveChanges();
                    Console.WriteLine($"User: {u.Username}");

                    // return RedirectToAction("Home");
                }
                HttpContext.Session.SetInt32("UserId", u.UserId);
                return Redirect("/home");
            }

            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser l)
        {
            if (ModelState.IsValid)
            {
                User logging_in_user = context.Users.FirstOrDefault(u => u.Username == l.LoginUsername);
                if (logging_in_user != null)
                {
                    var result = LoginHasher.VerifyHashedPassword(l, logging_in_user.Password, l.LoginPassword);
                    if (result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "Invalid Password");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", logging_in_user.UserId);

                        return Redirect("/home");
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginUsername", "Invalid Username");
                    ModelState.AddModelError("LoginUsername", "Username does noto exist");
                }
            }
            return View("Index");
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            ViewBag.SessionUser = HttpContext.Session.GetInt32("LoggedIn");

            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/");
            }

            
            List<Product> AllProducts = context.Products
                .Include(p => p.Seller)
                .Include(p => p.Bids)
                .ThenInclude(b => b.User)
                .OrderBy(p => p.EndDate)
                .ToList();

            foreach (Product prod in AllProducts)
            {
                if (prod.EndDate < DateTime.Now)
                {
                    prod.Seller.Wallet += prod.HighestBid;
                    Bid bid = context.Bids
                    .Include(c => c.User)
                    .SingleOrDefault(b => b.ProductId == prod.ProductId && b.Amount == prod.HighestBid);

                    if (bid != null)
                    {
                        bid.User.Wallet -= prod.HighestBid;
                    }
                        context.Products.Remove(prod);
                        context.SaveChanges();
                }
            }
                ViewBag.Products = AllProducts;
                ViewBag.UserId = UserId;
                ViewBag.User = context.Users.FirstOrDefault(u => u.UserId == (int)UserId);

            return View(AllProducts);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return Redirect("/");
        }

        [HttpGet("auction/new")]
        public IActionResult NewAuction()
        {
            return View();
        }

        [HttpPost("auction/new")]
        public IActionResult CreateNew(Product newP)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/");
            }

            if (ModelState.IsValid)
            {
                if (newP.StartBid > 0)
                {
                    if (newP.EndDate > DateTime.Now)
                    {

                        newP.SellerId = (int)UserId;
                        newP.HighestBid = newP.StartBid;
                        newP.CreatedAt = DateTime.Now;
                        newP.UpdatedAt = DateTime.Now;
                        context.Products.Add(newP);
                        context.SaveChanges();

                        return Redirect("/home");
                    }else
                    {
                        ModelState.AddModelError("EndDate", "End date must be in the future");
                    }
                }else
                {
                    ModelState.AddModelError("StartBid", "Starting Bid must be greater than 0");
                }
            }

            
                Product product = new Product();
                return View("NewAuction", newP);
            
        }

        [HttpGet("product/{id}")]
        public IActionResult ShowProduct(int id)
        {
            ViewBag.SessionUser = HttpContext.Session.GetInt32("LoggedIn");

            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/");
            }

            Product product = context.Products
                .Include(x => x.Seller)
                .Include(y => y.Bids)
                .ThenInclude(z => z.User)
                .FirstOrDefault(shi => shi.ProductId == id);

            List<Bid> highest = context.Bids
                .Include(b => b.User)
                .Include(b => b.Product)
                .OrderByDescending(b => b.Amount)
                .Where(c => c.ProductId == id)
                .ToList();
                Console.WriteLine($"Product End Date:{product.EndDate}");
                ViewBag.EndDate = product.EndDate;

            if (highest.Count > 0)
            {
                ViewBag.HighestBid = highest[0];
            }
            else
            {
                ViewBag.HighestBid = null;
            }

            Console.WriteLine("******************************************************************");
            Console.WriteLine($"ProductId: {id}");

            return View("ShowProduct", product);
        }
        

        [HttpPost("bid/{id}")]
        public IActionResult NewBid(int id, double UserBid)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.SessionUser = HttpContext.Session.GetInt32("LoggedIn");

            if (UserId == null)
            {
                return Redirect("/");
            }

            User sessionUser = context.Users.FirstOrDefault(u => u.UserId == UserId);

            Product prod = context.Products.Include(c => c.Seller).SingleOrDefault(p => p.ProductId == id);

            if (UserBid > 0)
            {

                if (UserBid <= sessionUser.Wallet && UserBid > prod.HighestBid)
                {
                    Bid newBid = new Bid();

                    newBid.ProductId = id;
                    newBid.UserId = sessionUser.UserId;
                    newBid.Amount = UserBid;
                    newBid.CreatedAt = DateTime.Now;
                    newBid.UpdatedAt = DateTime.Now;
                    context.Bids.Add(newBid);
                    context.SaveChanges();

                    Bid highestBid = context.Bids
                    .OrderByDescending(b => b.Amount).Where(c => c.ProductId == id).First();

                    prod.HighestBid = highestBid.Amount;
                    context.SaveChanges();

                    return Redirect($"/product/{id}");
                }
                else
                {
                    ViewBag.BidError = "You cannot bid more than you have in your wallet or less than the current highest bid";
                }
            }
            else
            {
                ViewBag.BidError = "Bids must be a positive number";
            }
            Product product = context.Products
                .Include(x => x.Seller)
                .Include(y => y.Bids)
                .ThenInclude(z => z.User)
                .FirstOrDefault(shi => shi.ProductId == id);

            ViewBag.EndDate = product.EndDate;

            return View("ShowProduct", prod);
        }

        [HttpGet("delete/{ProductId}")]
        public IActionResult Delete(int ProductId)
        {
            Product p = context.Products.FirstOrDefault(pr => pr.ProductId == ProductId);
            context.Products.Remove(p);
            context.SaveChanges();
            return Redirect("/home");
        }

    }
}
