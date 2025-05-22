using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyProcess : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Процесс"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "Pr"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;
        public string? ProcessName { set; get; }
        public bool? IsActive { set; get; }
        public string? UserAccessCode { set; get; }

        public virtual MyUserType? UserAccess { set; get; }

        /// <summary>
        /// все менюшки данного процессса
        /// </summary>
        public virtual List<MyMenuContent> Content { set; get; } = new List<MyMenuContent>();

      

    }
}
