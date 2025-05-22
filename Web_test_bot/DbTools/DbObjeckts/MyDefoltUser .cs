using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyDefoltUser : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Пользоваттель по дефолту"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "DU"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        public string? TelegramUserName { set; get; }
        public long? TelegramId { set; get; }

        public string? UserTypeСode { set; get; }

        public virtual MyUserType? UserType { set; get; }
    }
}
