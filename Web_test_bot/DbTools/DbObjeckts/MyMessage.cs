using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyMessage : IDbMyElement
    {
        public int Id { get; set; }
        public string EntyName
        {
            get { return "Сообщение"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "Ms"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        public long TeleChatId { set; get; }

        /// <summary>
        /// Сообщение или CallBack который пришел из чата
        /// </summary>
        public string? MessageContent { set; get; }

        /// <summary>
        /// Если сообщениеявляеться командой
        /// </summary>
        public bool? isComand { set; get; }

        /// <summary>
        ///Если сообщение пришло
        /// </summary>
        public bool? IsIncomingMessage { set; get; }
        public DateTime? MessageDate { get; set; }

        public virtual MyChat? ParentChat { set; get; }

        /// <summary>
        ////Если сообщение было тправлено мной  то нужнно удлять его
        /// </summary>
        public virtual bool? NeedToDelite { set; get; }

        /// <summary>
        /// Айди сообщения
        /// </summary>
        public int? TeleMessageId { set; get; }

        /// <summary>
        /// Для фото
        /// </summary>
        public string? photoInfo { set; get; }

        /// <summary>
        /// Для фото
        /// </summary>
        public virtual List<MyPhoto>? photo { set; get; } = new List<MyPhoto>();

        /// <summary>
        ///тут храним прошлое чат, где это собщение являться пред идущем перед текущим
        /// </summary>
        public virtual MyChat? PriviousIncummingMessageChat { set; get; }

        public static MyMessage CreateMyMessageFromUpdate(Update update)
        {
            var message =
                update.Message
                ?? update.CallbackQuery?.Message
                ?? update.EditedMessage
                ?? update.ChannelPost;
            var result = new MyMessage() { IsIncomingMessage = true, MessageDate = message?.Date };

            if (update.Type == UpdateType.CallbackQuery)
            {
                result.isComand = true;
                result.MessageContent = update.CallbackQuery?.Data;
            }
            else if (update.Type == UpdateType.Message)
            {
                if (update.Message.Text.StartsWith("/"))
                {
                    result.isComand = true;
                }
                result.MessageContent = message?.Text;
            }
            result.TeleMessageId = message?.MessageId;

            return result;
        }

        public static MyMessage CreateMyMessageFroMessage(Message Message)
        {
            var message = Message;
            var result = new MyMessage() { IsIncomingMessage = true, MessageDate = message?.Date };

            if (message?.Text != null)
            {
                if (message.Text.StartsWith("/"))
                {
                    result.isComand = true;
                }
                result.MessageContent = message?.Text;
                result.TeleMessageId = message?.MessageId;
            }
            if (message?.Photo != null)
            {
                result.TeleMessageId = message?.MessageId;
                string text = "";
                foreach (var item in message?.Photo)
                {
                    text +=
                        $"FileId:{item.FileId}|FileSize:{item.FileSize}|FileUniqueId:{item.FileUniqueId}|Height:{item.Height}|Width:{item.Width}/";
                }
                if (!message.Photo.IsNullOrEmpty() && message?.Photo.Count() > 1)
                {
                    result.photo.Add(
                        new MyPhoto()
                        {
                            FileId = message?.Photo?.LastOrDefault()?.FileId,
                            FileSize = message?.Photo?.LastOrDefault()?.FileSize,
                            FileUniqueId = message?.Photo?.LastOrDefault()?.FileUniqueId,
                            Height = message?.Photo?.LastOrDefault()?.Height ?? 0,
                            Width = message?.Photo?.LastOrDefault().Width ?? 0,
                        }
                    );
                }

                result.photoInfo = text;
            }

            return result;
        }
    }
}
