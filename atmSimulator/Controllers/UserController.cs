using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using atmSimulator.Models;
using System.Diagnostics;


namespace atmSimulator.Controllers
{
    public class UserController : Controller
    {
        Dictionary<String, String> user;
        // GET: /<controller>/
        //this is going to be the home page
        public IActionResult Mlogin(string username, int pin)
        {
            
            user = UserModel.Login(username, pin);
          
            if (UserModel.LoggedIn)
            {
                return RedirectToAction("Home", "User");
            }
            else
            {
                return RedirectToAction("Error", "User");
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
        public IActionResult Home()
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }

            return View();
        }

        //This is what shows when the withdraw button is clicked
        public IActionResult Withdraw()
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }
            return View();
        }

        //this is what pops up when Deposit button is clicked 
        public IActionResult Deposit()
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }
            return View();
        }

        public IActionResult Transfer()
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }
            return View();
        }

        public IActionResult History()
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
                ViewData["transactions"] = UserModel.getTransactions();
            }
            return View();
        }
        public IActionResult WithdrawAmt(double amt)
        {
            Dictionary<string,string> status= UserModel.Withdraw(amt);
            if (status["status"] == "0")
            {
                return RedirectToAction("Success", "Withdrawal successful!");
            }else if (status["status"] == "1")
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction("Error", "Something went wrong. Sorry!");
        }
        //always redirects to home for ow
        public IActionResult Success(string type)
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }
            ViewData["message"] = "Unknown success hath befallen your path. :)";
            if (!type.Equals(null))
            {
                ViewData["message"] = type;
            }
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Error(string type)
        {
            if (UserModel.LoggedIn)
            {
                ViewData["UserId"] = UserModel.UserId;
                ViewData["Name"] = UserModel.Name;
                ViewData["CurrentBalance"] = UserModel.CurrentBalance;
                ViewData["LoggedIn"] = UserModel.LoggedIn;
            }
           
            ViewData["message"] = "Unknown error hath befallen your path. :(";
            if (!type.Equals(null))
            {
                ViewData["message"] = type;
            }
            return View();
        }

        public IActionResult DepositAmt(double amt)
        {
            Dictionary<string, string> status = UserModel.Deposit(amt);
            if (status["status"] == "0")
            {
                return RedirectToAction("Success", "Deposit successful!");
            }else if (status["status"] == "1")
            {
                return RedirectToAction("Error", "Unable to deposit. Make sure your amount is greater than zero.");
            }
            return RedirectToAction("Error","Something went wrong. Sorry!");
        }

        public IActionResult TransferAmt(string uid, double amt)
        {
            Dictionary<string, string> status = UserModel.Transfer(uid, amt);
            if (status["status"] == "0")
            {
                return RedirectToAction("Success", "Transfer Successful!");
            }
            else if (status["status"] == "1")
            {
                return RedirectToAction("Error", "Unable to transfer. Make sure the user ID is correct, you have enough balance and the amount is positive.");
            }
            return RedirectToAction("Error", "Something went wrong. Sorry!");
        }

       
        public IActionResult Logout()
        {
            UserModel.Logout();
            return RedirectToAction("Index");
        }
        public void ErrorHandle()
        {

        }

    }
}
