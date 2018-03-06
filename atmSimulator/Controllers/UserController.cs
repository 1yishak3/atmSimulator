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
                ViewData["myErrorMessage"] = "Invalid username an pin";
                return View("Login");
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



        public IActionResult Transactions()

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
            Dictionary<string, string> status = UserModel.Withraw(amt);

            if (amt > 500)
            {
                ViewData["myErrorMessage"] = "Cannot withraw more than 500";
                return View("Withraw");
            }
            else if (amt % 20 != 0)
            {
                ViewData["myErrorMessage"] = "Amount must be multiple of 20";
                return View("Withraw");
            }

            else if (status["status"] == "0")

            {
                ViewData["mySuccessMessage"] = "Withraw successful!";
                return View("Withraw");

            }
            else if (status["status"] == "1")

            {
                ViewData["myErrorMessage"] = "Unable to withraw. Make sure your amount is greater than zero.";
                return View("Withraw");

            }
            ViewData["myErrorMessage"] = "Someting went wrong. Sorry!";
            return View("Withraw");
        }

        //always redirects to home for ow


        public IActionResult DepositAmt(double amt)

        {
            Dictionary<string, string> status = UserModel.Deposit(amt);
            if (amt > 500)
            {
                ViewData["myErrorMessage"] = "Cannot deposit more than 500";
                return View("Deposit");
            }
            else if (amt % 20 != 0)
            {
                ViewData["myErrorMessage"] = "Amount must be multiple of 20";
                return View("Deposit");
            }

            else if (status["status"] == "0")

            {
                ViewData["mySuccessMessage"] = "Deposit successful!";
                return View("Deposit");

            }
            else if (status["status"] == "1")

            {
                ViewData["myErrorMessage"] = "Unable to deposit. Make sure your amount is greater than zero.";
                return View("Deposit");

            }
            ViewData["myErrorMessage"] = "Someting went wrong. Sorry!";
            return View("Deposit");
        }



        public IActionResult TransferAmt(string uid, double amt)

        {

            Dictionary<string, string> status = UserModel.Transfer(uid, amt);

            if (status["status"] == "0")

            {
                ViewData["mySuccessMessage"] = "Transfer successful!";
                return View("Transfer");

            }

            else if (status["status"] == "1")

            {
                ViewData["myErrorMessage"] = "Unable to transfer. Make sure the user ID is correct, you have enough balance and the amount is positive.";
                return View("Transfer");

            }

            ViewData["myErrorMessage"] = "Someting went wrong. Sorry!";
            return View("Transfer");

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