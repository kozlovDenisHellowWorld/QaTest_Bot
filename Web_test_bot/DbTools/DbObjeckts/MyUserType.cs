using System;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyUserType : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Вид пользователя"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "UT"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        /// <summary>
        /// Наименование типа
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// Код типа
        /// </summary>
        public string? TypeCode { get; set; }

        /// <summary>
        /// Описание ипа
        /// </summary>
        public string? TypeDiscr { get; set; }

        public virtual List<MyUser> UserList { get; set; } = new List<MyUser>();

        public virtual List<MyDefoltUser> DefoltUsers { set; get; } = new List<MyDefoltUser>();

        /// <summary>
        /// Код типа
        /// </summary>
        public bool? IsDefoult { get; set; }

        public virtual MyProcess? Process { set; get; }
    }
}
