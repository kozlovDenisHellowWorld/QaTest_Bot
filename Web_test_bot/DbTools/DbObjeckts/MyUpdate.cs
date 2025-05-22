using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyUpdate : IDbMyElement
    {
        [Key]
        public int Id { get; set; }

        public string EntyName
        {
            get { return "Обновление"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "Up"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        /// <summary>
        /// ID обновления из телеграмма.
        /// </summary>
        public int? TeleUpdateId { get; set; }

        /// <summary>
        /// Дата обновления, когда пришел .
        /// </summary>
        public DateTime? TeleDate { get; set; }

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string? MessageText { get; set; }

        /// <summary>
        /// Телеграм чат ID, в котором было получено обновление.
        /// </summary>
        public long? ChatId { get; set; }

        /// <summary>
        /// Название чата, в котором было получено обновление.
        /// </summary>
        public string? ChatTitle { get; set; }

        /// <summary>
        /// Тип чата (private, group, supergroup, channel).
        /// </summary>
        public virtual ChatType? ChatType { get; set; }

        /// <summary>
        /// Является ли сообщение командой.
        /// </summary>
        public bool? IsCommand { get; set; } = false;

        /// <summary>
        /// Является ли сообщение командой.
        /// </summary>
        public bool? IsTextMessage { get; set; } = false;

        /// <summary>
        /// Команда, если это сообщение является командой.
        /// </summary>
        public string? Command { get; set; }

        public virtual UpdateType? Type { set; get; }

        public bool IsInccoming { set; get; }

        /// <summary>
        ///Чат к которуму пренадлежит данный апдейт или чат из кототрого пришло обнавлние
        /// </summary>
        public virtual MyChat? ParentChat { set; get; }

        /// <summary>
        /// Создает мой update для того чтобы полижить в базу. Обрабатывает только пока !CallBack и message
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public static MyUpdate CreateNewUpdate(Update update)
        {
            MyUpdate result = new MyUpdate()
            {
                TeleUpdateId = update.Id, // ID обновления из Telegram
                CreatedAt = DateTime.Now, // Дата обновления

                MessageText =
                    update.Message?.Text
                    ?? update.EditedMessage?.Text
                    ?? update.ChannelPost?.Text
                    ?? update.EditedChannelPost?.Text
                    ?? update.BusinessMessage?.Text, // Текст сообщения

                ChatId =
                    update.Message?.Chat.Id
                    ?? update.CallbackQuery?.Message?.Chat.Id
                    ?? update.EditedMessage?.Chat.Id
                    ?? update.ChannelPost?.Chat.Id
                    ?? update.EditedChannelPost?.Chat.Id
                    ?? update.BusinessMessage?.Chat.Id, // ID чата

                ChatTitle =
                    update.Message?.Chat.Title
                    ?? update.CallbackQuery?.Message?.Chat.Title
                    ?? update.EditedMessage?.Chat.Title
                    ?? update.ChannelPost?.Chat.Title
                    ?? update.EditedChannelPost?.Chat.Title
                    ?? update.BusinessMessage?.Chat.Title, // Название чата

                ChatType =
                    update.Message?.Chat.Type
                    ?? update.CallbackQuery?.Message?.Chat.Type
                    ?? update.EditedMessage?.Chat.Type
                    ?? update.ChannelPost?.Chat.Type
                    ?? update.EditedChannelPost?.Chat.Type
                    ?? update.BusinessMessage?.Chat.Type, // Тип чата

                TeleDate =
                    update.Message?.Date
                    ?? update.CallbackQuery?.Message?.Date
                    ?? update.EditedMessage?.Date
                    ?? update.ChannelPost?.Date
                    ?? update.EditedChannelPost?.Date
                    ?? update.BusinessMessage?.Date,

                Type = update.Type,
            };
            result.IsInccoming = true;

            if (update.Type == UpdateType.CallbackQuery)
            {
                result.IsCommand = true;
                result.Command = update.CallbackQuery?.Data;
            }
            else //if (update.Type == UpdateType.Message)
            {
                if (update.Message?.Text?.StartsWith("/") == true)
                {
                    result.IsCommand = true;
                    result.Command = update.Message.Text.Substring(1);
                }
                else
                {
                    //этого никогда не должно быть
                    result.IsCommand = false;
                    result.MessageText = update?.Message?.Text;
                }
            }

            return result;
        }

        /// <summary>
        /// Создает мой update для того чтобы полижить в базу. Обрабатывает только пока message
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public static MyUpdate CreateNewMyUpdateFromUpdate(Message message)
        {
            MyUpdate result = new MyUpdate()
            {
                TeleUpdateId = message.MessageId, // ID обновления из Telegram
                CreatedAt = DateTime.Now, // Дата обновления
                MessageText = message.Text, // Текст сообщения
                ChatId = message.Chat.Id, // ID чата
                ChatTitle = message.Chat.Title, // Название чата
                ChatType = message.Chat.Type, // Тип чата

                TeleDate = message.Date,
                Type = UpdateType.Message,
            };

            result.IsInccoming = true;

            if (message.Text?.StartsWith("/") == true)
            {
                result.IsCommand = true;
                result.Command = message.Text;
            }
            else
            {
                result.IsCommand = false;
                result.MessageText = message.Text;
            }

            return result;
        }
    }
}
