using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyInput : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Контент меню"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "MC"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        public string? InputName { get; set; }
        public string? InputDescription { get; set; }
        public string? InputContent { get; set; }

        public string? InputType { get; set; }

        public string? NextMenuCode { get; set; }

        public int? MenuId { get; set; }

        [ForeignKey("MenuId")]
        public virtual MyMenuContent? Menu { get; set; }

        public virtual MyMenuContent? NextMenu { get; set; }
    }
}
