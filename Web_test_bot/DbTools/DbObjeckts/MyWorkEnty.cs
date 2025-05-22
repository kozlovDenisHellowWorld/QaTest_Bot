using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyWorkEnty : IDbMyElement
    {
        public int Id { get; set; }

        public string EntyName { get; set; } = "Рабочий объект";

        public string EntyCode { get; set; } = "MWE";

        public DateTime? CreatedAt { get; set; }
        public bool? IsDelite { get; set; } = false;

        public string? CodeType { set; get; }

        public bool? isDone { set; get; }
        public virtual List<MyWorkDetales> Items { get; set; } = new List<MyWorkDetales>();

        public virtual int? ParentChatId { get; set; }
        public virtual MyChat? ParentChat { get; set; }
    }
}
