using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Diagnostics;
//use http client here
//use the deserializer here

namespace atmSimulator.Models
{
    public class UserModel
    {
        //read the comments below very well dude
        //to do things faster.

        public static String UserId { get; set; }
        public static String Name { get; set; }
        public static Double CurrentBalance { get; set; }
        public static Dictionary<String, Dictionary<string, string>> Transactions { get; set; }
        public static bool LoggedIn { get; set; }
        public static String url = "https://rahyozxcgs.localtunnel.me";
        //every function below should check if user is logged in first
        //if user is logged in, continue with fetching data, if not, then don't fetch. Just return a dict with status 1 and error not logged in
        //my controllers will handle this and tell it to the view.

       

        
        public static Dictionary<string,string> Login(string username, int pin1)
        {
            //do login here
            //I will return an object, which will be the username userId current blance and stuff like that
            //you can deserialize this and assign to the static variables above

            //then I will fetch that data on controller and do the rest of the work there

            //when it has successfully logged in, make sure you set loggedIn to true so that
            //the rest of the functions will work
            string pin = (Convert.ToString(pin1));
            var result = "";
            try
            {
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["username"] = username;
                    values["pin"] = pin;
                    var response = client.UploadValues(url+"/login", values);

                    var responseString = Encoding.Default.GetString(response);

                    result = responseString;
                }
                Debug.WriteLine(result);
            }
            catch(WebException e)
            {
                Debug.WriteLine(e);
            }
            

            Dictionary<string, string> mainDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            while (mainDict == null) ;
            Debug.WriteLine(mainDict);
            if (mainDict["status"].Equals("0"))
            {
                LoggedIn = true;
                UserId = mainDict["uid"];
                Name = mainDict["username"];
                CurrentBalance = Double.Parse(mainDict["currentBalance"]);

            }

            else { LoggedIn = false; }
            

            if (LoggedIn)
            {
                Dictionary<String, String> u = new Dictionary<String, String>();
 
                u.Add("status", "0");
                u.Add("error", "");
                return u;
            }
            else
            {
                Dictionary<String, String> u = new Dictionary<String, String>();
              
                u.Add("status", "1");
                u.Add("error", "failed");
                return u;
            }
        }
        public static void FetchUpdated()
        {
            //you should call this function after every update like withdrawing transfering ot depositing
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //GET Request
            using (var client = new WebClient())
            {
                var responseString = client.DownloadString(url+"/data/" + UserId + "/all");
                dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
            }

            if (dict["status"].Equals("0"))
            {
                UserId = dict["uid"];
                Name = dict["username"];
                CurrentBalance = Double.Parse(dict["currentBalance"]);
                Debug.WriteLine(CurrentBalance);
            }

        }
        public static Dictionary<String, String> post(double amount, string action)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["userId"] = "" + UserId;
                values["amount"] = "" + amount;
                values["action"] = action;

                var response = client.UploadValues(url+"/action", values);

                var responseString = Encoding.Default.GetString(response);
                dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

            }

            return dict;
        }
        public static Dictionary<String, String> Withdraw(double amt)
        {
            //if not logged in, return a dict containing status 1 and error not logged in automatically
            //make sure you check for common sense things like amount greater than 0 and stuff

            //call fetchUpdated() here to reflect changes made to the data base
            Dictionary<string, string> fail = new Dictionary<string, string>()
                { {"status", "1"}, {"error", "not logged in"} };

            if (!LoggedIn) { return fail; }

            if (amt > CurrentBalance) { fail["error"] = "non-sufficient funds"; return fail; }
            else if (amt <= (double)0) { fail["error"] = "check amount being withdrawn"; return fail; }

            Dictionary<string, string> result = post(amt, "withdraw");

            if (result["status"].Equals("0"))
            {
                FetchUpdated();
            }

            return result;
            
        }
        public static Dictionary<String, String> Deposit(double amount)
        {
            Dictionary<string, string> fail = new Dictionary<string, string>()
                { {"status", "1"}, {"error", "not logged in"} };

            if (!LoggedIn) { return fail; }

            if (amount <= (double)0) { fail["error"] = "check amount being deposited"; return fail; }

            CurrentBalance += amount;
            Dictionary<string, string> response = post(amount, "deposit");

            if (response["status"].Equals("0") )
            {
                FetchUpdated();
            }

            return response;
        }
        public static Dictionary<String, String> Transfer(String toId, double amount)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["userId1"] = "" + UserId;
                values["userId2"] = toId;
                values["amount"] = "" + amount;
                values["action"] = "transfer";
                var response = client.UploadValues(url+"/action", values);

                var responseString = Encoding.Default.GetString(response);

                result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
            }

            if (result["status"].Equals("0"))
            {
                FetchUpdated();
            }

            return result;
        }
        public static  Dictionary<String, Dictionary<string, string>> getTransactions()
        {
            Dictionary<string, Dictionary<string, string>> response = new Dictionary<string, Dictionary<string, string>>();
            //GET Request
            using (var client = new WebClient())
            {
                var responseString = client.DownloadString(url+"/data/" + UserId + "/transactions");
                Debug.WriteLine(responseString);
                response = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(responseString);
            }
            Transactions = response;
            return response;
        }
        public static void Logout()
        {
            LoggedIn = false;
        }
        
    }
}
