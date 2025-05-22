using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyMenuType : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Меню"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "MT"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        public string? TypeName { set; get; }

        public string? TypeCode { set; get; }

        public string? TypeDescription { set; get; }

        public virtual List<MyMenuContent>? Menus { set; get; } = new List<MyMenuContent>();
    }
}
