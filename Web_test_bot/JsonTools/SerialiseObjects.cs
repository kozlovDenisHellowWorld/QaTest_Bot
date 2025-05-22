using System;
using System.Security.Principal;
using System.Threading.Tasks;
using RestSharp;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.SerialiseObjects
{
    public class BotConfig
    {
        public Bot Bot { get; set; }
        public List<MyDefoltUser> DefoltUsers { get; set; }
        public BotInst BotInst { get; set; }
    }

    public class Bot
    {
        public string Token { get; set; }
        public string BotId { get; set; }
        public string UpdatetDB { get; set; }
    }

    public class BotInst
    {
        public List<MyUserType> UserTypes { get; set; }
        public List<MyMenuType> MenuTypes { get; set; }
        public List<MyInputType> InputTypes { get; set; }
        public List<MyProcess> Processes { get; set; }
    }

    public class UserType
    {
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public string TypeDiscr { get; set; }
    }
}
