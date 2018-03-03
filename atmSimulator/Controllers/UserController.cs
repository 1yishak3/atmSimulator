using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using atmSimulator.Models;


namespace atmSimulator.Controllers
{
    public class UserController : Controller
    {
        UserModel user;
        bool loggedIn;
        // GET: /<controller>/
        //this is going to be the home page
        public void Mlogin(String username, int pin)
        {

            user = new UserModel(username, pin);

             if (UserModel.LoggedIn)
             {
                 Home(user);
             }
             else
             {
                 new ErrorViewModel();
             }
        }
        //public 
        public IActionResult Index()
        {
            return View();
        }

        //this is going to be the login page
        public IActionResult Login()
        {
            return View();
        }

        //Landing page (contains buttons to withdraw page deposit page and stuff)
        public IActionResult Home(UserModel user)
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }

            return View(user);
        }

        //This is what shows when the withdraw button is clicked
        public IActionResult Withdraw()
        {
            return View();
        }

        //this is what pops up when Deposit button is clicked 
        public IActionResult Deposit()
        {
            return View();
        }

        public IActionResult Transfer()
        {   
            return View();
        }

        public IActionResult Transactions()
        {
            return View();
        }

    }
}
