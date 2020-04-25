using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email Already Exist");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("userinSession", newUser.UserId);
                    return Redirect("/dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userIn.LoginEmail);
                if(userInDb == null){
                    ModelState.AddModelError("LoginEmail","Invalid Email Address");
                    return View("Index");
                }
                else
                {
                    var hash = new PasswordHasher<LoginUser>();
                    var result = hash.VerifyHashedPassword(userIn, userInDb.Password, userIn.LoginPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LoginPassword","Invalid Password");
                        return View("Index");
                    }
                    else
                    {
                        //userId stored in userinSession
                        HttpContext.Session.SetInt32("userinSession", userInDb.UserId);
                        return Redirect("/dashboard");
                    }
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("dashboard")]
        public IActionResult Dashbaord()
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            User userInDB = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            List<Reservation> allReservations = _context.Reservations.Include(r=>r.Creator)
                                                                     .Include(r=>r.Guests)
                                                                     .ThenInclude(a=>a.Wedding)
                                                                     .ToList();
            if(userInDB == null)
            {
                return RedirectToAction("Logout");
            }
            else
            {
                ViewBag.User = userInDB;
                return View("Dashboard",allReservations);
            }
        }

        [HttpGet("reservation")]
        public IActionResult Reservation()
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            ViewBag.getUser = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            return View("Reservation");
        }

        [HttpPost("reservation")]
        public IActionResult Reserve(Reservation newReservation)
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            newReservation.UserId = (int)ID; 
            if(ModelState.IsValid)
            {
                _context.Reservations.Add(newReservation);
                _context.SaveChanges();
                return Redirect("/dashboard");
            }
            else
            {
                ViewBag.getUser = _context.Users.FirstOrDefault(u=>u.UserId==ID);
                return View("Reservation");
            }

        }

        [HttpGet("/{resID}")]
        public IActionResult WeddingInfo(int resID)
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            ViewBag.getUser = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            ViewBag.getOne = _context.Reservations.FirstOrDefault(r => r.ReservationId == resID);
                                                  
                                                  
            List<Association> Guests = _context.Associations.Where(a => a.Wedding.ReservationId == resID)
                                                            .Include(r => r.Guest) 
                                                            .ToList();
            return View("WeddingInfo", Guests);
        }
        

        [HttpGet("/attend/{resID}/{userID}")]
        public IActionResult Attend(int resID, int userID)
        {
            Association attend = new Association();
            attend.UserId = userID;
            attend.ReservationId = resID;
            _context.Associations.Add(attend);
            _context.SaveChanges();
            return Redirect("/dashboard");
        }
        [HttpGet("/leave/{resID}/{userID}")]
        public IActionResult Leave(int resID, int userID)
        {
            Association leave = _context.Associations.FirstOrDefault(a=> a.UserId == userID && a.ReservationId == resID);
            _context.Associations.Remove(leave);
            _context.SaveChanges();
            return Redirect("/dashboard");
        }

        [HttpGet("/cancel/{resID}")]
        public IActionResult Cancel(int resID)
        {
            Reservation cancel = _context.Reservations.FirstOrDefault(r=>r.ReservationId == resID);
            _context.Reservations.Remove(cancel);
            _context.SaveChanges();
            return Redirect("/dashboard");
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userinSession");
            HttpContext.Session.Clear();
            return Redirect("/");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
