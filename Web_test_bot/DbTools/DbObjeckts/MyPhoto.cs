using System;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyPhoto : PhotoSize, IDbMyElement
    {
        [Key]
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Фото"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "PH"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        public int? MyWorkDetalesId { set; get; }
        public virtual MyWorkDetales? ParentDetale { set; get; }

        public int? MessageId { set; get; }
        public virtual MyMessage? ParentMessage { set; get; }

        public int? ParentMenuID { set; get; }
        public virtual MyMessage? ParentMenu { set; get; }
    }
}
