using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyMenuContent : IDbMyElement
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

        /// <summary>
        /// Нименование меню или контента (не уникальное)
        /// </summary>
        public string? MenuName { set; get; }

        /// <summary>
        /// Код меню (уникальное в рамках процесса)
        /// </summary>
        public string? MenuCode { set; get; }

        /// <summary>
        /// Тип меню - код (уникальное индификатор типа)
        /// </summary>
        public string? MenuTypeCode { set; get; }

        public int? TypeId { set; get; }

        /// <summary>
        /// Тип меню - объект (уникальное индификатор типа)
        /// </summary>
        public virtual MyMenuType? Type { set; get; }

        /// <summary>
        ///  Удалять или не удалять
        /// </summary>
        public bool? NeedToDelete { set; get; }

        /// <summary>
        ///  текстт кототрый будем вствалять в меню
        /// </summary>
        public string? Content { set; get; }

        public int? ProcessID { set; get; }

        /// <summary>
        /// Родительская меню
        /// </summary>
        public virtual MyProcess? Process { set; get; }

        public virtual List<MyInput> Inputs { set; get; } = new List<MyInput>();

        public virtual List<MyInput> CallingInputs { set; get; } = new List<MyInput>();

        /// <summary>
        /// Чаты которые сейчас в этом меню
        /// </summary>
        public virtual List<MyChat>? ChatsInThisMenu { set; get; } = new List<MyChat>();

        /// <summary>
        /// Фото для конкретного меню
        /// </summary>
        [JsonIgnore]
        public virtual List<MyPhoto>? PhotoFormMenu { set; get; } = new List<MyPhoto>();

        /// <summary>
        /// Метод для получения текста который необходимо отправить
        /// </summary>
        /// <returns>Конеттент, который будет оттправлен пользователю</returns>
        public string GetContent()
        {
            string resuls = "";
            //вотт тут будем фоорматировать текст
            resuls = Content;
            return resuls;
        }
    }
}
