using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyInputType : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Сообщение"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "UT"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

      
        public string? TypeName { set; get; }

        public string? TypeCode { set; get; }

        public string? TypeDescription { set; get; }
    }
}
