using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public static Dictionary<String, String> Transactions { get; set; }
        public static bool LoggedIn { get; set; }//every function below should check if user is logged in first
        //if user is logged in, continue with fetching data, if not, then don't fetch. Just return a dict with status 1 and error not logged in
        //my controllers will handle this and tell it to the view.

        public UserModel(string username, int pin){
            //use the Login method here. It should do all the assigning and instantiating
        }

        
        private static void Login()
        {
            //do login here
            //I will return an object, which will be the username userId current blance and stuff like that
            //you can deserialize this and assign to the static variables above

            //then I will fetch that data on controller and do the rest of the work there

            //when it has successfully logged in, make sure you set loggedIn to true so that
            //the rest of the functions will work
           
        }
        private static void FetchUpdated()
        {
            //you should call this function after every update like withdrawing transfering ot depositing
           
        }
        public Dictionary<String, String> withdraw(double amt)
        {
            //if not logged in, return a dict containing status 1 and error not logged in automatically
            //make sure you check for common sense things like amount greater than 0 and stuff
            
            //call fetchUpdated() here to reflect changes made to the data base
            return new Dictionary<string, string>();
        }
        public  Dictionary<String, String> deposit(double amt)
        {
            return new Dictionary<string, string>();
        }
        public  Dictionary<String, String> transfer(String toId)
        {
            return new Dictionary<string, string>();
        }
        public  Dictionary<String, String> getTransactions()
        {
            return new Dictionary<string, string>();
        }
        
    }
}
