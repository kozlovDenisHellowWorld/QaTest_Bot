using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyUser : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Пользователь"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "Us"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        /// <summary>
        ///  Телеграм ID
        /// </summary>
        public long? TeleId { set; get; }

        public int? usertypeId { set; get; }

        /// <summary>
        /// Тип пользователя
        /// </summary>
        public virtual MyUserType? UserType { set; get; }

        /// <summary>
        /// Список пользователей, групповых чатов
        /// </summary>
        public virtual List<MyChat> UserGroupList { set; get; } = new List<MyChat>();

        public string? TeleUserName { set; get; }
        public string? TeleFirstName { set; get; }
        public string? TeleLasttName { set; get; }
        public string? TeleLanguageCode { set; get; }
        public bool? TeleIsBot { set; get; }
        public bool? CanJoinGroups { set; get; }
        public bool? CanReadAllGroupMessages { set; get; }
        public bool? IsBan { set; get; } = false;

        public static MyUser CreteUserFromUpdate(Message message)
        {
            var teleUser = message?.From ?? null;

            return CreteMyUser(teleUser);
        }

        public static MyUser CreteUserFromUpdate(Update update)
        {
            var teleUser =
                update.Message?.From
                ?? update.CallbackQuery?.Message?.From
                ?? update.EditedMessage?.From
                ?? null;

            return CreteMyUser(teleUser);
        }

        private static MyUser CreteMyUser(User? teleUser)
        {
            MyUser resuls = new MyUser()
            {
                CreatedAt = DateTime.Now,
                IsDelite = false,
                TeleId = teleUser?.Id ?? null,
                TeleUserName = teleUser?.Username ?? null,
                TeleFirstName = teleUser?.FirstName ?? null,
                TeleLasttName = teleUser?.LastName ?? null,
                TeleLanguageCode = teleUser?.LanguageCode ?? null,
                TeleIsBot = teleUser?.IsBot ?? null,
                CanJoinGroups = teleUser?.CanJoinGroups ?? null,
                CanReadAllGroupMessages = teleUser?.CanReadAllGroupMessages ?? null,
            };
            return resuls;
        }
    }
}
