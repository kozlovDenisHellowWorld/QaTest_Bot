using System;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyWorkDetales : IDbMyElement
    {
        public int Id { get; set; }

        public string EntyName { get; set; } = "Мое свойство";

        public string EntyCode { get; set; } = "MWD";

        public DateTime? CreatedAt { get; set; }
        public bool? IsDelite { get; set; } = false;

        public string? Key { get; set; }
        public string? Value { get; set; }

        public int? ParenId { get; set; }

        public virtual MyWorkEnty? ParentEnt { get; set; }

        /// <summary>
        /// если в каком либо запросы прислали фото
        /// </summary>
        public virtual List<MyPhoto> TelegramPfotoes { set; get; } = new List<MyPhoto>();
    }
}
