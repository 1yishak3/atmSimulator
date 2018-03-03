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
        // GET: /<controller>/
        //this is going to be the home page
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
        public IActionResult Home()
        {
            return View();
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

        public IActionResult History()
        {
            return View();
        }

    }
}
