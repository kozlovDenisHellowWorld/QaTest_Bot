using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Web_test_bot.BotTools;
using Web_test_bot.BotTools.ScriptTools;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public class MyChat : IDbMyElement
    {
        [Key]
        public int Id { get; set; } // Идентификатор чата

        public string EntyName
        {
            get { return "Чат"; }
        } // Русское название элемента
        public string EntyCode
        {
            get { return "Ch"; }
        }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDelite { get; set; } = false;

        /// <summary>
        /// Имя пользователя чата.
        /// </summary>
        public string? TeleChatUserName { get; set; }

        /// <summary>
        /// Уникальный идентификатор чата в Telegram.
        /// </summary>
        public long? TeleChatId { get; set; }

        /// <summary>
        /// Название чата.
        /// </summary>
        public string? TeleChatTitle { get; set; }

        /// <summary>
        /// Тип чата (private, group, supergroup, channel).
        /// </summary>
        public virtual ChatType? TeleChatType { get; set; }

        /// <summary>
        /// Количество участников в чате.
        /// </summary>
        public int? TeleChatMembersCount { get; set; }

        /// <summary>
        /// Является ли чат группой.
        /// </summary>
        public bool? IsGroupChat { get; set; }

        /// <summary>
        /// Дата последнего сообщения в чате.
        /// </summary>
        public virtual DateTime? LastMessageDate { get; set; }

        /// <summary>
        /// Пользователи чата
        /// </summary>
        public virtual List<MyUser> ChatUsers { get; set; } = new List<MyUser>();

        /// <summary>
        /// Является ли чат группой.
        /// </summary>
        public bool? IsBan { get; set; } = false;

        /// <summary>
        /// Является ли чат группой.
        /// </summary>
        public int? LastUpdateId { get; set; }

        [JsonIgnore]
        public virtual List<MyUpdate>? ChatUpdates { set; get; } = new List<MyUpdate>();

        /// <summary>
        ///Все сообщения которые есть в чате и оотт бота и от пользователя
        /// </summary>
        public virtual List<MyMessage>? ChatMessages { set; get; } = new List<MyMessage>();

        //текущий процесс
        public virtual MyMenuContent? CurentMenu { set; get; }

        //текущий процесс
        public int? CurentMenuId { set; get; }

        /// <summary>
        /// поле для сохраненения иформации типа в каком объекте мы ковыряемся, какую заявку используем и так далее
        /// </summary>
        public string? ProcessCode { set; get; }

        /// <summary>
        /// Текст который мы отправим в меню
        /// </summary>
        public string? CurentTextModel { set; get; }

        /// <summary>
        /// Тут будем хранитть ID сообщения если у нас менються собшения
        /// </summary>
        public long? CurentMessageIdForMenu { set; get; }

        public int? PriviousIncummingMessageId { set; get; }

        /// <summary>
        ///тут храним прошлое сообщение кототрое пришло
        /// </summary>
        public virtual MyMessage? PriviousIncummingMessage { set; get; }

        public virtual List<MyWorkEnty> WorkItems { set; get; } = new List<MyWorkEnty>();

        //----------------------------------------------------------------------------------------

        public void AddUpddate(MyUpdate myUpdate)
        {
            LastUpdateId = myUpdate.TeleUpdateId;
            ChatUpdates.Add(myUpdate);
        }

        public void AddMessage(MyMessage newMessage)
        {
            LastMessageDate = newMessage.MessageDate;
            ChatMessages.Add(newMessage);
        }

        public void AddMessages(List<MyMessage> newMessages)
        {
            if (newMessages == null)
            {
                return;
            }
            foreach (var newMessage in newMessages)
            {
                LastMessageDate = newMessage.MessageDate;
                ChatMessages.Add(newMessage);
            }
        }

        public async Task OnEndMenu(
            TelegramBotClient client,
            CancellationToken cancellationToken,
            DbContext db
        )
        {
            if (CurentMenu == null)
            {
                return;
            }

            await ClearChat(client, cancellationToken);

            var executeEndMenu = new ScriptExecutor(new Scripts());
            await executeEndMenu.ExecuteEndMenu(
                CurentMenu.MenuCode,
                this,
                client,
                cancellationToken,
                db
            );

            CurentTextModel = "";
            Console.WriteLine(
                $"Bot {DateTime.Now} - действие ---OnEndMenu--- объекте:{EntyName} {Id}"
            );
        }

        public async Task OnStartMenu(
            TelegramBotClient client,
            CancellationToken cancellationToken,
            DbContext db
        )
        {
            if (CurentMenu == null)
            {
                return;
            }

            var executeEndMenu = new ScriptExecutor(new Scripts());
            await executeEndMenu.ExecuteStartMenu(
                CurentMenu.MenuCode,
                this,
                client,
                cancellationToken,
                db
            );

            CurentTextModel = CurentMenu.GetContent();
            Console.WriteLine(
                $"Bot {DateTime.Now} - действие ---OnStartMenu--- объекте:{EntyName} {Id}"
            );
        }

        public static MyChat CreateMyChatFromUpdate(Update update)
        {
            var chat =
                update.CallbackQuery?.Message?.Chat
                ?? update.Message?.Chat
                ?? update.EditedMessage?.Chat;

            MyChat result = new MyChat()
            {
                CreatedAt = DateTime.Now,
                IsDelite = false,
                IsGroupChat = chat?.IsForum,
                TeleChatId = chat?.Id,
                TeleChatType = chat?.Type,
                TeleChatTitle = chat?.Title,
                TeleChatUserName = chat?.Username,
            };

            return result; // Или выбросьте исключение, если это необходимо
        }

        public static MyChat CreateMyChatFromUpdate(Message message)
        {
            MyChat result = new MyChat()
            {
                CreatedAt = DateTime.Now,
                IsDelite = false,
                IsGroupChat = message.Chat?.IsForum,
                TeleChatId = message.Chat?.Id,
                TeleChatType = message.Chat?.Type,
                TeleChatTitle = message.Chat?.Title,
                TeleChatUserName = message.Chat?.Username,
            };

            return result; // Или выбросьте исключение, если это необходимо
        }

        public async Task SetNextMenu(
            MyMenuContent? nextProcess,
            TelegramBotClient telegramBotClient,
            CancellationToken cancellationToken,
            DbContext db
        )
        {
            var priviousProcess = CurentMenu;
            var priviousProcessCode = ProcessCode;
            await OnEndMenu(telegramBotClient, cancellationToken, db);
            //тут мы должны почистить все поля кооторые до этого исчпользовали
            //-------------------

            CurentMenu = nextProcess;
            await OnStartMenu(telegramBotClient, cancellationToken, db);
            db.SaveChanges();
        }

        public async Task<string> SendMessageRequest()
        {
            return "";
        }

        public async Task ExecuteMessage() { }

        public async Task<string> ClearChat(
            TelegramBotClient client,
            CancellationToken cancellationToken
        )
        {
            if (ChatMessages.Count == 0)
                return $"🤖 {DateTime.Now}: Не нашел сообщения коотрыые надоо удалять из чата -GqzB_";
            else
            {
                Console.WriteLine(
                    $"🤖 {DateTime.Now}: Начало очиски чата от отосланых сообщений ранее -YlWC6"
                );
                var remuveMessages = ChatMessages
                    .Where(i =>
                        i.NeedToDelite == true
                        && i.IsDelite == false
                        && i.IsIncomingMessage == false
                    )
                    .ToList();

                Console.WriteLine(
                    $"🤖 {DateTime.Now}: Нашел {remuveMessages.Count()} для удаления из чата  -YlWC6"
                );

                bool isExptionTelegraam = false;
                while (true)
                {
                    try
                    {
                        var deliteMessage = remuveMessages.LastOrDefault() ?? null;

                        if (deliteMessage == null)
                        {
                            if (remuveMessages.Count() == 1)
                            {
                                deliteMessage = remuveMessages.FirstOrDefault();
                            }
                            else if (remuveMessages.Count() == 0)
                                break;
                        }

                        if (!isExptionTelegraam)
                        {
                            await client.DeleteMessageAsync(
                                TeleChatId,
                                deliteMessage.TeleMessageId ?? 0011,
                                cancellationToken: cancellationToken
                            );
                        }
                        ChatMessages.FirstOrDefault(m => m.Id == deliteMessage.Id).IsDelite = true;
                        remuveMessages.Remove(deliteMessage);
                        isExptionTelegraam = false;
                    }
                    catch (System.Exception ex)
                    {
                        isExptionTelegraam = true;
                        Console.WriteLine(
                            $"Ошибка выполнения удаления собщения BDMessageID: -yRO!@"
                        );
                    }
                }
                Console.WriteLine(
                    $"🤖 {DateTime.Now}: Удалил все если было что удалять, отработало все как надо -2wzj+"
                );
            }
            return "ddd";
        }
    }
}
