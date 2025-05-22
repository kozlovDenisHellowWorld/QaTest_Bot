using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Web_test_bot.DbTools;
using Web_test_bot.DbTools.DbObjeckts;
//вотт тут
namespace Web_test_bot.BotTools.ScriptTools
{
    public class Scripts
    {
        [StartMenu("StartMenu_001")]
        public async Task OnStartMenu_StartMenu_001(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            if (myChat.CurentMessageIdForMenu != null)
            {
                //так посколько делаем хардкод, то тут убедм удалять все лишнее при старте когда вызываем стартоовое меню
                //и первое с чего начнем меню которое изменялось

                try
                {
                    await client.DeleteMessageAsync(
                        myChat.TeleChatId,
                        (int)myChat.CurentMessageIdForMenu,
                        cancellationToken
                    );
                }
                catch { }

                var dbMSG = myChat.ChatMessages?.FirstOrDefault(m =>
                    m.TeleMessageId == (int?)myChat.CurentMessageIdForMenu
                );

                dbMSG.IsDelite = true;

                var isues = myChat.WorkItems.Where(i =>
                    i.CodeType == "Предложение" && i.isDone == false && i.IsDelite == false
                    || i.CodeType == "Вопрос" && i.isDone == false && i.IsDelite == false
                    || i.CodeType == "Ошибка" && i.isDone == false && i.IsDelite == false
                );
                foreach (var isue in isues)
                {
                    isue.isDone = true;
                    isue.IsDelite = true;
                    await db.SaveChangesAsync();
                }

                //||i.CodeType=="Вопрос"||i.CodeType=="Ошибка"

                await db.SaveChangesAsync();

                myChat.CurentMessageIdForMenu = null;
                await db.SaveChangesAsync();
            }

            //await myChat.ClearChat(client, cancellationToken);
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs);

            var telegramMessage = await TeleTools.SendMenuTextOrPhoto(
                client,
                myChat,
                cancellationToken,
                ButtonMenu,
                db,
                true,
                false
            );

            // var mreere = myChat.WorkItems.ToList();
        }

        [EndMenu("StartMenu_001")]
        public async Task OnEndMenu_StartMenu_001(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        ) { }

        //💡----------------------------------------------------------------------
        //💡----------------------------------------------------------------------
        //💡----------------------------------------------------------------------
        //💡----------------------------------------------------------------------


        #region 💡
        //💡🆕---------------------------------------------------------------------

        [StartMenu("ProcessAdvice_start")]
        public async Task OnStartMenu_ProcessAdvice_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            try
            {
                var mess = await client.SendPhotoAsync(
                    chatId: myChat.TeleChatId,
                    caption: myChat.CurentMenu.Content,
                    photo: myChat.CurentMenu.PhotoFormMenu.FirstOrDefault().FileId,
                    replyMarkup: ButtonMenu,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage2 = MyMessage.CreateMyMessageFroMessage(mess);
                myMessage2.IsIncomingMessage = false;
                myMessage2.NeedToDelite = true;
                myChat.AddMessage(myMessage2);

                await db.SaveChangesAsync();
            }
            catch
            {
                var message = await client.SendTextMessageAsync(
                    chatId: myChat.TeleChatId,
                    text: myChat.CurentMenu.Content,
                    replyMarkup: ButtonMenu,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage = MyMessage.CreateMyMessageFroMessage(message);
                myMessage.IsIncomingMessage = false;
                myMessage.NeedToDelite = true;
                myChat.AddMessage(myMessage);
                myChat.CurentMessageIdForMenu = myMessage.TeleMessageId;
                await db.SaveChangesAsync();
            }
        }

        [EndMenu("ProcessAdvice_start")]
        public async Task OnEndMenu_ProcessAdvice_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var content = myChat
                .ChatMessages?.LastOrDefault(m => m.IsIncomingMessage == true)
                ?.MessageContent;
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            if (Ident != 0) // если есть старый
            {
                var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
                curententEntyWork.Items.FirstOrDefault(i => i.Key == "UserMessage").Value = content;
                await db.SaveChangesAsync();
                return;
            }
            //---новый
            if (content == null)
                return;
            var sourse = new MyWorkDetales()
            {
                CreatedAt = DateTime.Now,
                Key = "UserMessage",
                Value = content,
            };

            var sourseStatus = new MyWorkDetales()
            {
                CreatedAt = DateTime.Now,
                Key = "status",
                Value = "new",
            };

            MyWorkEnty userAdvice = new MyWorkEnty()
            {
                CodeType = "Предложение",
                EntyName = "Заявка на предложение",
                CreatedAt = DateTime.Now,
                isDone = false,
            };

            userAdvice.Items.Add(sourse);
            userAdvice.Items.Add(sourseStatus);
            myChat.WorkItems.Add(userAdvice);

            await db.SaveChangesAsync();
            myChat.ProcessCode = userAdvice.EntyCode + userAdvice.Id;

            await db.SaveChangesAsync();
        }

        //💡🆕---------------------------------------------------------------------

        [StartMenu("ProcessAdvice_step_chek")]
        public async Task OnStartMenu_ProcessAdvice_step_chek(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);

            var WorkItem = curententEntyWork?.Items.FirstOrDefault(i => i.Key == "UserMessage");
            var textContent = WorkItem?.Value;
            if (WorkItem == null)
            {
                textContent = "что то пошло не так";
            }
            textContent = myChat.CurentMenu.Content.Replace("{text}", textContent);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        //💡📝---------------------------------------------------------------------

        [StartMenu("ProcessAdvice_step_send_ok")]
        public async Task OnStartMenu_ProcessAdvice_step_send_ok(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
            curententEntyWork.isDone = true;
            myChat.ProcessCode = "";
            db.SaveChanges();

            var nexPr = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;

            var textContent = myChat.CurentMenu.Content;

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            await myChat.SetNextMenu(nexPr, client, cancellationToken, db);
        }

        //💡📷---------------------------------------------------------------------

        [StartMenu("ProcessAdvice_step_addPhoto")]
        public async Task Process_Advice_step_addPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("ProcessAdvice_step_addPhoto")]
        public async Task OnEndMenu_Advice_step_addPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
            try
            {
                curententEntyWork.Items.Add(
                    new MyWorkDetales()
                    {
                        CreatedAt = DateTime.Now,
                        Key = "photo",
                        Value = myChat.ChatMessages.LastOrDefault().photoInfo,
                        TelegramPfotoes = myChat.ChatMessages.LastOrDefault().photo,
                    }
                );
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // SQLite Error 1: 'table MyPhoto has no column named MyWorkDetalesId'.
            }
        }

        //💡❓❌---------------------------------------------------------------------

        [StartMenu("ProcessAdvice_step_askToStopProcess")]
        public async Task OnStartMenu_ProcessAdvice_step_askToStopProcess(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;

            db.SaveChanges();
        }

        [EndMenu("ProcessAdvice_step_askToStopProcess")]
        public async Task OnEndMenu_ProcessAdvice_step_askToStopProcess(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        ) { }

        //💡❓❌---------------------------------------------------------------------

        private int getIdBDFromCallBack_kostil(string caallBack, string entCode)
        {
            if (caallBack == null)
            {
                return 0;
            }
            if (caallBack == "")
            {
                return 0;
            }
            if (!caallBack.Contains(entCode))
            {
                return 0;
            }
            string menu = caallBack.Split('|').FirstOrDefault(i => i.Contains(entCode));
            menu = menu.Replace(entCode, "");
            menu = menu.Replace(":", "");
            if (menu != null)
            {
                // Попытка конвертировать строку в int
                if (int.TryParse(menu, out int id))
                {
                    return id; // Возвращаем результат, если конвертация успешна
                }
            }

            return 0;
        }

        //💡🗑 ---------------------------------------------------------------------

        [StartMenu("ProcessAdvice_End_forgetAll")]
        public async Task OnStartMenu_ProcessAdvice_End_forgetAll(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            myChat.ProcessCode = "";

            var allIsuesClosed = myChat
                .WorkItems.Where(i => i.CodeType == "Предложение" && i.isDone == false)
                .ToList();

            foreach (var item in allIsuesClosed)
            {
                item.isDone = true;
                item.IsDelite = true;
                await db.SaveChangesAsync();
            }
            var nexPr = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;

            db.SaveChanges();
            await myChat.SetNextMenu(nexPr, client, cancellationToken, db);
        }
        #endregion

        //---❓----------------------------------------------------------------------------------------------
        //---❓----------------------------------------------------------------------------------------------
        //---❓----------------------------------------------------------------------------------------------
        //---❓----------------------------------------------------------------------------------------------
        //---❓----------------------------------------------------------------------------------------------
        //---❓----------------------------------------------------------------------------------------------

        #region ❓

        //❓🆕---------------------------------------------------------------------
        [StartMenu("ProcessAskQwes_start")]
        public async Task OnStartMenu_ProcessAskQwes_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню
            try
            {
                var mess = await client.SendPhotoAsync(
                    chatId: myChat.TeleChatId,
                    caption: myChat.CurentMenu.Content,
                    photo: myChat.CurentMenu.PhotoFormMenu.FirstOrDefault().FileId,
                    replyMarkup: ButtonMenu,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage2 = MyMessage.CreateMyMessageFroMessage(mess);
                myMessage2.IsIncomingMessage = false;
                myMessage2.NeedToDelite = true;
                myChat.AddMessage(myMessage2);

                await db.SaveChangesAsync();
            }
            catch
            {
                var message = await client.SendTextMessageAsync(
                    chatId: myChat.TeleChatId,
                    text: myChat.CurentMenu.Content,
                    replyMarkup: ButtonMenu,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage = MyMessage.CreateMyMessageFroMessage(message);
                myMessage.IsIncomingMessage = false;
                myMessage.NeedToDelite = true;
                myChat.AddMessage(myMessage);
                myChat.CurentMessageIdForMenu = myMessage.TeleMessageId;
                await db.SaveChangesAsync();
            }
        }

        [EndMenu("ProcessAskQwes_start")]
        public async Task OnEndMenu_ProcessAskQwes_startt(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var content = myChat
                .ChatMessages?.LastOrDefault(m => m.IsIncomingMessage == true)
                ?.MessageContent;
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            if (Ident != 0) // если есть старый
            {
                var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
                curententEntyWork
                    .Items.FirstOrDefault(i => i.Key == "UserMessage" && i.IsDelite == false)
                    .IsDelite = true;

                await db.SaveChangesAsync();
                var sourse2 = new MyWorkDetales()
                {
                    CreatedAt = DateTime.Now,
                    Key = "UserMessage",
                    Value = content,
                };
                curententEntyWork.Items.Add(sourse2);
                await db.SaveChangesAsync();

                return;
            }
            //---новый
            if (content == null)
                return;
            var sourse = new MyWorkDetales()
            {
                CreatedAt = DateTime.Now,
                Key = "UserMessage",
                Value = content,
            };
            var sourseStatus = new MyWorkDetales()
            {
                CreatedAt = DateTime.Now,
                Key = "status",
                Value = "new",
            };
            MyWorkEnty userAdvice = new MyWorkEnty()
            {
                CodeType = "Вопрос",
                CreatedAt = DateTime.Now,
                isDone = false,
            };

            userAdvice.Items.Add(sourse);
            userAdvice.Items.Add(sourseStatus);

            myChat.WorkItems.Add(userAdvice);

            await db.SaveChangesAsync();
            myChat.ProcessCode = userAdvice.EntyCode + userAdvice.Id;

            await db.SaveChangesAsync();
        }

        [StartMenu("ProcessAskQwes_GetEmail")]
        public async Task OnStartMenu_ProcessAskQwes_GetEmail(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("ProcessAskQwes_GetEmail")]
        public async Task OnEndMenu_ProcessAskQwes_GetEmail(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var content = myChat
                .ChatMessages?.LastOrDefault(m => m.IsIncomingMessage == true)
                ?.MessageContent;
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var isue = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);

            var oldEmail = isue.Items.FirstOrDefault(e => e.Key == "email" && e.IsDelite == false);
            if (oldEmail != null)
            {
                oldEmail.IsDelite = true;
            }
            isue.Items.Add(
                new MyWorkDetales()
                {
                    CreatedAt = DateTime.Now,
                    EntyName = "email",
                    Key = "email",
                    Value = content,
                }
            );

            await db.SaveChangesAsync();
        }

        //❓📝---------------------------------------------------------------------
        [StartMenu("ProcessAskQwes_step_chek")]
        public async Task OnStartMenu_ProcessAskQwes_step_chekk(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);

            var WorkItem = curententEntyWork?.Items.FirstOrDefault(i =>
                i.Key == "UserMessage" && i.IsDelite == false
            );
            var email = curententEntyWork?.Items.FirstOrDefault(i =>
                i.Key == "email" && i.IsDelite == false
            );

            var photo = curententEntyWork
                ?.Items.Where(i => i.Key == "photo" && i.IsDelite == false)
                .ToList();
            var textContent = WorkItem?.Value;
            if (WorkItem == null)
            {
                textContent = "что то пошло не так";
            }
            textContent = myChat.CurentMenu?.Content?.Replace("{text}", WorkItem?.Value);
            textContent = textContent?.Replace("{email}", email?.Value ?? "-");
            textContent = textContent?.Replace("{nPHoto}", photo.Count().ToString() ?? "-");
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu?.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent ?? "Что то пошло не так",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        //❓📷---------------------------------------------------------------------
        [StartMenu("ProcessAskQwes_step_addPhoto")]
        public async Task Process_ProcessAskQwes_step_addPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("ProcessAskQwes_step_addPhoto")]
        public async Task OnEndMenu_ProcessAskQwes_step_addPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);

            curententEntyWork.Items.Add(
                new MyWorkDetales()
                {
                    CreatedAt = DateTime.Now,
                    Key = "photo",
                    Value = myChat.ChatMessages.LastOrDefault().photoInfo,
                    TelegramPfotoes = myChat.ChatMessages.LastOrDefault().photo,
                }
            );
            await db.SaveChangesAsync();
        }

        //❓📷---------------------------------------------------------------------
        [StartMenu("ProcessAskQwes_step_askToStopProcess")]
        public async Task OnStartMenu_ProcessAskQwes_step_askToStopProcess(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;

            db.SaveChanges();
        }

        //💡❓❌---------------------------------------------------------------------
        [StartMenu("ProcessAskQwes_End_forgetAll")]
        public async Task OnStartMenu_ProcessAskQwes_End_forgetAll(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            myChat.ProcessCode = "";
            var allIsuesClosed = myChat
                .WorkItems.Where(i => i.CodeType == "Вопрос" && i.isDone == false)
                .ToList();

            foreach (var item in allIsuesClosed)
            {
                item.isDone = true;
                item.IsDelite = true;
                await db.SaveChangesAsync();
            }
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню
            var nexPr = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;
            await myChat.SetNextMenu(nexPr, client, cancellationToken, db);
        }

        [StartMenu("ProcessAskQwes_step_send_ok")]
        public async Task OnStartMenu_ProcessAskQwes_step_send_ok(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
            curententEntyWork.isDone = true;
            myChat.ProcessCode = "";
            db.SaveChanges();

            var nexPr = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;

            var textContent = myChat.CurentMenu.Content;

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            await myChat.SetNextMenu(nexPr, client, cancellationToken, db);
        }
        #endregion

        //---⚠️----------------------------------------------------------------------------------------------
        //---⚠️----------------------------------------------------------------------------------------------
        //---⚠️----------------------------------------------------------------------------------------------
        //---⚠️----------------------------------------------------------------------------------------------
        //---⚠️----------------------------------------------------------------------------------------------
        //---⚠️----------------------------------------------------------------------------------------------

        #region ⚠️

        //⚠️🆕---------------------------------------------------------------------
        [StartMenu("ProcessExption_start")]
        public async Task OnStartMenu_ProcessExption_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            try
            {
                var mess = await client.SendPhotoAsync(
                    chatId: myChat.TeleChatId,
                    caption: myChat.CurentMenu.Content,
                    photo: myChat.CurentMenu.PhotoFormMenu.FirstOrDefault().FileId,
                    replyMarkup: ButtonMenu,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage2 = MyMessage.CreateMyMessageFroMessage(mess);
                myMessage2.IsIncomingMessage = false;
                myMessage2.NeedToDelite = true;
                myChat.AddMessage(myMessage2);

                await db.SaveChangesAsync();
            }
            catch
            {
                var message = await client.SendTextMessageAsync(
                    chatId: myChat.TeleChatId,
                    text: myChat.CurentMenu.Content,
                    replyMarkup: ButtonMenu,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage = MyMessage.CreateMyMessageFroMessage(message);
                myMessage.IsIncomingMessage = false;
                myMessage.NeedToDelite = true;
                myChat.AddMessage(myMessage);
                myChat.CurentMessageIdForMenu = myMessage.TeleMessageId;
                await db.SaveChangesAsync();
            }
        }

        [EndMenu("ProcessExption_start")]
        public async Task OnEndMenu_ProcessExption_startt(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var content = myChat
                .ChatMessages?.LastOrDefault(m => m.IsIncomingMessage == true)
                ?.MessageContent;
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            if (Ident != 0) // если есть старый
            {
                var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
                curententEntyWork.Items.FirstOrDefault(i => i.Key == "UserMessage").Value = content;
                await db.SaveChangesAsync();
                return;
            }
            //---новый
            if (content == null)
                return;
            var sourse = new MyWorkDetales()
            {
                CreatedAt = DateTime.Now,
                Key = "UserMessage",
                Value = content,
            };
            var sourseStatus = new MyWorkDetales()
            {
                CreatedAt = DateTime.Now,
                Key = "status",
                Value = "new",
            };
            MyWorkEnty userAdvice = new MyWorkEnty()
            {
                CodeType = "Ошибка",
                CreatedAt = DateTime.Now,
                isDone = false,
            };

            userAdvice.Items.Add(sourse);
            userAdvice.Items.Add(sourseStatus);
            myChat.WorkItems.Add(userAdvice);

            await db.SaveChangesAsync();
            myChat.ProcessCode = userAdvice.EntyCode + userAdvice.Id;

            await db.SaveChangesAsync();
        }

        //⚠️📝---------------------------------------------------------------------
        [StartMenu("ProcessExption_step_chek")]
        public async Task OnStartMenu_ProcessExption_step_chekk(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);

            var WorkItem = curententEntyWork?.Items.FirstOrDefault(i => i.Key == "UserMessage");
            var textContent = WorkItem?.Value;
            if (WorkItem == null)
            {
                textContent = "что то пошло не так";
            }
            textContent = myChat.CurentMenu.Content.Replace("{text}", textContent);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        //⚠️📷---------------------------------------------------------------------
        [StartMenu("ProcessExption_step_addPhoto")]
        public async Task Process_ProcessExption_step_addPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("ProcessExption_step_addPhoto")]
        public async Task OnEndMenu_ProcessExption_step_addPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");
            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);

            curententEntyWork.Items.Add(
                new MyWorkDetales()
                {
                    CreatedAt = DateTime.Now,
                    Key = "photo",
                    Value = myChat.ChatMessages.LastOrDefault().photoInfo,
                    TelegramPfotoes = myChat.ChatMessages.LastOrDefault().photo,
                }
            );
            await db.SaveChangesAsync();
        }

        //⚠️📷---------------------------------------------------------------------
        [StartMenu("ProcessExption_step_askToStopProcess")]
        public async Task OnStartMenu_ProcessExption_step_askToStopProcess(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;

            db.SaveChanges();
        }

        //💡⚠️❌---------------------------------------------------------------------
        [StartMenu("ProcessExption_End_forgetAll")]
        public async Task OnStartMenu_ProcessExption_End_forgetAll(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            myChat.ProcessCode = "";
            var allIsuesClosed = myChat
                .WorkItems.Where(i => i.CodeType == "Ошибка" && i.isDone == false)
                .ToList();

            foreach (var item in allIsuesClosed)
            {
                item.isDone = true;
                item.IsDelite = true;
                await db.SaveChangesAsync();
            }
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню
            var nexPr = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;
            await myChat.SetNextMenu(nexPr, client, cancellationToken, db);
        }

        [StartMenu("ProcessExption_step_send_ok")]
        public async Task OnStartMenu_ProcessExption_step_send_ok(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var Ident = getIdBDFromCallBack_kostil(myChat.ProcessCode, "MWE");

            var curententEntyWork = myChat.WorkItems.FirstOrDefault(i => i.Id == Ident);
            curententEntyWork.isDone = true;
            myChat.ProcessCode = "";
            db.SaveChanges();

            var nexPr = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;

            var textContent = myChat.CurentMenu.Content;

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            await myChat.SetNextMenu(nexPr, client, cancellationToken, db);
        }
        #endregion




        //---ListUserIsues----------------------------------------------------------------------------------------------
        //---ListUserIsues----------------------------------------------------------------------------------------------
        //---ListUserIsues----------------------------------------------------------------------------------------------
        //---ListUserIsues----------------------------------------------------------------------------------------------


        #region ListUserIsues


        [StartMenu("ListUserIsues")]
        [Obsolete]
        public async Task OnStartMenu_ListUserIsues(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var qw = myChat
                .WorkItems.Where(i =>
                    i.CodeType == "Вопрос" && i.isDone == true && i.IsDelite == false
                )
                .ToList();

            var ex = myChat
                .WorkItems.Where(i =>
                    i.CodeType == "Ошибка" && i.isDone == true && i.IsDelite == false
                )
                .ToList();

            var idea = myChat
                .WorkItems.Where(i =>
                    i.CodeType == "Предложение" && i.isDone == true && i.IsDelite == false
                )
                .ToList();

            var allIsues = qw;
            allIsues.AddRange(ex);
            allIsues.AddRange(idea);

            using (null)
            {
                var sampleBtn = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                    i.InputType == "DynamicSample"
                );

                int nRows = 10;
                int iPagination = 0;

                List<InlineKeyboardButton> paginationButtons = new List<InlineKeyboardButton>();
                iPagination = (int)getIdBDFromCallBack_kostil(
                    myChat.ChatMessages.LastOrDefault().MessageContent,
                    "nf"
                );

                if (iPagination == 0)
                {
                    iPagination = (int)getIdBDFromCallBack_kostil(
                        myChat.ChatMessages.LastOrDefault().MessageContent,
                        "nb"
                    );
                }

                if (iPagination - nRows >= 0)
                {
                    paginationButtons.Add(
                        new InlineKeyboardButton()
                        {
                            Text = "⬅️",
                            CallbackData =
                                $"{myChat.CurentMenu.EntyCode}{myChat.CurentMenu.Id}|"
                                + $"|nb{iPagination - nRows}|",
                        }
                    );
                }
                if (iPagination + nRows < allIsues.Count)
                {
                    paginationButtons.Add(
                        new InlineKeyboardButton()
                        {
                            Text = "➡️",
                            CallbackData =
                                $"{myChat.CurentMenu.EntyCode}{myChat.CurentMenu.Id}|"
                                + $"|nf{iPagination + nRows}|",
                        }
                    );
                }

                //  if (allIsues.Count <= iPagination + 10)
                //      allIsues.RemoveRange(iPagination, 10);
                //  if (allIsues.Count <= 10)
                //      allIsues.RemoveRange(0, allIsues.Count - 10);

                foreach (var isue in allIsues)
                {
                    int iv = allIsues.IndexOf(isue);

                    if (!((iPagination) <= allIsues.IndexOf(isue)))
                    {
                        continue;
                    }
                    if (!(allIsues.IndexOf(isue) < (iPagination + nRows)))
                    {
                        continue;
                    }

                    var callback =
                        $"{sampleBtn.NextMenu.EntyCode}{sampleBtn.NextMenu.Id}|{isue.ParentChat.EntyCode}{isue.ParentChat.Id}|{isue.EntyCode}{isue.Id}";

                    var btnContent =
                        $"{isue.Id}) {isue.CodeType} {isue.ParentChat.ChatUsers.FirstOrDefault().TeleUserName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleFirstName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleLasttName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleId.ToString()} {isue.CreatedAt.Value.Date.ToString("dd.MM.yy")}";

                    _ = ButtonMenu.AddNewRow(
                        new InlineKeyboardButton() { Text = btnContent, CallbackData = callback }
                    );
                }
                ButtonMenu.AddNewRow(paginationButtons.ToArray());
            }

            var semple = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );
            /*
                        foreach (var item in qw)
                        {
                            ButtonMenu.AddNewRow(
                                new InlineKeyboardButton()
                                {
                                    Text = $"{item.CodeType} {item.CreatedAt.Value.Date.ToString("dd.MM.yy")}",
                                    CallbackData =
                                        $"{semple.NextMenu.EntyCode}{semple.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                                }
                            );
                        }
                        foreach (var item in ex)
                        {
                            ButtonMenu.AddNewRow(
                                new InlineKeyboardButton()
                                {
                                    Text = $"{item.CodeType} {item.CreatedAt.Value.Date.ToString("dd.MM.yy")}",
                                    CallbackData =
                                        $"{semple.NextMenu.EntyCode}{semple.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                                }
                            );
                        }
                        foreach (var item in idea)
                        {
                            ButtonMenu.AddNewRow(
                                new InlineKeyboardButton()
                                {
                                    Text = $"{item.CodeType} {item.CreatedAt.Value.Date.ToString("dd.MM.yy")}",
                                    CallbackData =
                                        $"{semple.NextMenu.EntyCode}{semple.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                                }
                            );
                        }
            */
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }
        #endregion



        [StartMenu("UseIsuesInfo")]
        [Obsolete]
        public async Task OnStartMenu_UseIsuesInfo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var curentIsueId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "MWE"
            );
            var curentIsue = myChat.WorkItems.FirstOrDefault(i => i.Id == curentIsueId);

            var text = myChat.CurentMenu.Content;
            text = text.Replace("{id}", curentIsue.Id.ToString());
            text = text.Replace("{date}", curentIsue.CreatedAt.Value.Date.ToString("dd.MM.yy"));
            text = text.Replace("{type}", curentIsue.CodeType);
            text = text.Replace(
                "{text}",
                curentIsue.Items.FirstOrDefault(i => i.Key == "UserMessage").Value
            );
            try
            {
                var albumMessages = await TeleTools.SendPhotoForIsue(
                    myChat,
                    client,
                    cancellationToken,
                    db,
                    curentIsue
                );
                myChat.AddMessages(albumMessages);
                await db.SaveChangesAsync();
            }
            catch (System.Exception ex) { }

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        #region AwaytingUser
        [StartMenu("AwaytingUser")]
        [Obsolete]
        public async Task OnStartMenu_AwaytingUser(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }
        #endregion
        //-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------
        #region StartMenu_002
        [StartMenu("StartMenu_002")]
        [Obsolete]
        public async Task OnStartMenu_StartMenu_002(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                await using var transaction = await db.Database.BeginTransactionAsync(
                    cancellationToken
                );

                var userinfo =
                    $"Зарегистрированные пользователи (regUser): {db.myUserTypes.FirstOrDefault(i => i.TypeCode == "regUser")?.UserList.Count ?? 0}\n"
                    + $"Администраторы (admin): {db.myUserTypes.FirstOrDefault(i => i.TypeCode == "admin")?.UserList.Count ?? 0}\n"
                    + $"Чаты в режиме ожидания: {db.MyChats.Count(i => i.CurentMenu.MenuCode == "AwaytingUser")}";

                var message2 = await client.SendTextMessageAsync(
                    chatId: myChat.TeleChatId,
                    text: userinfo,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessage2 = MyMessage.CreateMyMessageFroMessage(message2);
                myMessage2.IsIncomingMessage = false;
                myMessage2.NeedToDelite = true;
                myChat.AddMessage(myMessage2);
                myChat.CurentMessageIdForMenu = null;

                await db.SaveChangesAsync(cancellationToken);
                //воот тут
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await using var errorTransaction = await db.Database.BeginTransactionAsync(
                    cancellationToken
                );

                const string errorInfo =
                    "Не смог посчитать пользователей и состояние пользователей";

                var errorMessage = await client.SendTextMessageAsync(
                    chatId: myChat.TeleChatId,
                    text: errorInfo,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myErrorMessage = MyMessage.CreateMyMessageFroMessage(errorMessage);
                myErrorMessage.IsIncomingMessage = false;
                myErrorMessage.NeedToDelite = true;
                myChat.AddMessage(myErrorMessage);
                myChat.CurentMessageIdForMenu = null;

                await db.SaveChangesAsync(cancellationToken);
                await errorTransaction.CommitAsync(cancellationToken);
            }

            // Финальное сообщение с меню
            await using var finalTransaction = await db.Database.BeginTransactionAsync(
                cancellationToken
            );

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs);

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;

            await db.SaveChangesAsync(cancellationToken);
            await finalTransaction.CommitAsync(cancellationToken);
        }

        [StartMenu("AllUserTypsMenu")]
        [Obsolete]
        public async Task OnStartMenu_AllUserTypsMenu(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("AllIsuesMenu")]
        [Obsolete]
        public async Task OnStartMenu_AllIsuesMenu(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("Administrator_list")]
        [Obsolete]
        public async Task OnStartMenu_Administrator_list(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var semple = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );

            int nStartUsers = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "Us"
            );

            var allUsers = db
                .MyUsers.Where(u => u.IsDelite == false && u.UserType.TypeCode == "admin")
                .ToList();

            if (nStartUsers != 0) { }

            int i = 1;
            foreach (var item in allUsers)
            {
                var textcontent =
                    i + ") " + item.TeleUserName
                    ?? item.TeleFirstName
                    ?? item.TeleLasttName
                    ?? item.TeleId.ToString();
                ButtonMenu.AddNewRow(
                    new InlineKeyboardButton()
                    {
                        Text = textcontent,
                        CallbackData =
                            $"{semple.NextMenu.EntyCode}{semple.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                    }
                );
                i++;
            }

            ButtonMenu = TeleTools.GetPaginateButtons(myChat, ButtonMenu, 10);

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );
            myChat.ProcessCode = "";
            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("Admin_Info")]
        [Obsolete]
        public async Task OnStartMenu_Admin_Infot(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var curentIsueId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "Us"
            );
            if (curentIsueId == 0)
            {
                curentIsueId = (int)getIdBDFromCallBack_kostil(myChat.ProcessCode, "Us");
            }

            var item = db.MyUsers.FirstOrDefault(u => u.Id == curentIsueId);
            myChat.ProcessCode = $"{item.EntyCode}{item.Id}|";
            var textcontent = myChat.CurentMenu.Content;
            textcontent = textcontent.Replace("{id}", item.Id.ToString());
            textcontent = textcontent.Replace("{telename}", userLink(item));
            textcontent = textcontent.Replace("{teleid}", item.TeleId.ToString());
            textcontent = textcontent.Replace("{usertype}", item.UserType.TypeName);
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var btnsempleUserTypList = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );
            ButtonMenu.AddNewRow(
                new InlineKeyboardButton()
                {
                    Text = btnsempleUserTypList.InputContent,
                    CallbackData =
                        $"{btnsempleUserTypList.NextMenu.EntyCode}{btnsempleUserTypList.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                }
            );
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("Admin_Info_changeTypeToUserMenu")]
        [Obsolete]
        public async Task OnStartAdmin_Info_changeTypeToUserMenu(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var curentUserTypeId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "UT"
            );

            InlineKeyboardMarkup ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var curentUserId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "Us"
            );

            var curentUser = db.MyUsers.FirstOrDefault(i => i.Id == curentUserId);

            if (curentUserTypeId != 0)
            {
                curentUser.UserType = db.myUserTypes.FirstOrDefault(i => i.Id == curentUserTypeId);
            }
            var btnsempleUserTypList = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );
            foreach (var userTypeBd in db.myUserTypes.ToList())
            {
                if (curentUser.UserType.TypeCode == userTypeBd.TypeCode)
                    continue;
                ButtonMenu.AddNewRow(
                    new InlineKeyboardButton()
                    {
                        Text = userTypeBd.TypeName,
                        CallbackData =
                            $"{btnsempleUserTypList.NextMenu.EntyCode}{btnsempleUserTypList.NextMenu.Id}|{userTypeBd.EntyCode}{userTypeBd.Id}|{curentUser.EntyCode}{curentUser.Id}|",
                    }
                );
            }
            // ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs);
            var textcontent = myChat.CurentMenu.Content;
            textcontent = textcontent.Replace("{telename}", userLink(curentUser));
            textcontent = textcontent.Replace("{usertype}", curentUser.UserType.TypeName);
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("User_list")]
        [Obsolete]
        public async Task OnStartUser_list(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            InlineKeyboardMarkup ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var allUsers = db
                .MyUsers.Where(u => u.IsDelite == false && u.UserType.TypeCode != "admin")
                .ToList();
            var semple = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );

            int i = 1;
            foreach (var item in allUsers)
            {
                var btntextcontent =
                    i
                    + ") "
                    + (
                        item.TeleUserName
                        ?? item.TeleFirstName
                        ?? item.TeleLasttName
                        ?? item.TeleId.ToString()
                    );
                ButtonMenu.AddNewRow(
                    new InlineKeyboardButton()
                    {
                        Text = btntextcontent,
                        CallbackData =
                            $"{semple.NextMenu.EntyCode}{semple.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                    }
                );
                i++;
            }

            var textcontent = myChat.CurentMenu.Content;

            ButtonMenu = TeleTools.GetPaginateButtons(myChat, ButtonMenu, 10);

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myChat.ProcessCode = "";
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("User_Info")]
        public async Task OnStartMenu_User_Info(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var curentIsueId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "Us"
            );
            if (curentIsueId == 0)
            {
                curentIsueId = (int)getIdBDFromCallBack_kostil(myChat.ProcessCode, "Us");
            }

            var item = db.MyUsers.FirstOrDefault(u => u.Id == curentIsueId);
            myChat.ProcessCode = $"{item.EntyCode}{item.Id}|";
            var textcontent = myChat.CurentMenu.Content;
            textcontent = textcontent.Replace("{id}", item.Id.ToString());
            textcontent = textcontent.Replace("{telename}", userLink(item));
            textcontent = textcontent.Replace("{teleid}", item.TeleId.ToString());
            textcontent = textcontent.Replace("{usertype}", item.UserType.TypeName);
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var btnsempleUserTypList = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );
            ButtonMenu.AddNewRow(
                new InlineKeyboardButton()
                {
                    Text = btnsempleUserTypList.InputContent,
                    CallbackData =
                        $"{btnsempleUserTypList.NextMenu.EntyCode}{btnsempleUserTypList.NextMenu.Id}|{item.EntyCode}{item.Id}|",
                }
            );
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("User_Info_changeTypeToUserMenu")]
        [Obsolete]
        public async Task OnStart_User_Info_changeTypeToUserMenu(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var curentUserTypeId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "UT"
            );

            InlineKeyboardMarkup ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var curentUserId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "Us"
            );

            var curentUser = db.MyUsers.FirstOrDefault(i => i.Id == curentUserId);

            if (curentUserTypeId != 0)
            {
                curentUser.UserType = db.myUserTypes.FirstOrDefault(i => i.Id == curentUserTypeId);
            }
            var btnsempleUserTypList = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );
            foreach (var userTypeBd in db.myUserTypes.ToList())
            {
                if (curentUser.UserType.TypeCode == userTypeBd.TypeCode)
                    continue;
                ButtonMenu.AddNewRow(
                    new InlineKeyboardButton()
                    {
                        Text = userTypeBd.TypeName,
                        CallbackData =
                            $"{btnsempleUserTypList.NextMenu.EntyCode}{btnsempleUserTypList.NextMenu.Id}|{userTypeBd.EntyCode}{userTypeBd.Id}|{curentUser.EntyCode}{curentUser.Id}|",
                    }
                );
            }
            // ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs);
            var textcontent = myChat.CurentMenu.Content;
            textcontent = textcontent.Replace("{telename}", userLink(curentUser));
            textcontent = textcontent.Replace("{usertype}", curentUser.UserType.TypeName);
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        #endregion

        #region Заявки в разделе администраторов
        [StartMenu("User_isue_menu")]
        [Obsolete]
        public async Task OnStart_User_isue_menu(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var textcontent = myChat.CurentMenu.Content;
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("User_Newisue_menu_list")]
        [Obsolete]
        public async Task OnStart_User_Newisue_menu_list(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню
            var ButtonMenu2 = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var ButtonMenu3 = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs);

            var newIsues = db
                .MyChats.Where(i => i.WorkItems.Count > 0)
                .SelectMany(chat =>
                    chat.WorkItems.Where(item =>
                        item.IsDelite == false && item.CodeType == "Предложение"
                    )
                )
                .ToList();

            newIsues.AddRange(
                db.MyChats.Where(i => i.WorkItems.Count > 0)
                    .SelectMany(chat =>
                        chat.WorkItems.Where(item =>
                            item.IsDelite == false && item.CodeType == "Вопрос"
                        )
                    )
                    .ToList()
            );

            newIsues.AddRange(
                db.MyChats.Where(i => i.WorkItems.Count > 0)
                    .SelectMany(chat =>
                        chat.WorkItems.Where(item =>
                            item.IsDelite == false && item.CodeType == "Ошибка"
                        )
                    )
                    .ToList()
            );

            var sampleBtn = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );
            // Сортируем элементы по CreateAt, ближайшие к текущему времени первыми
            var sortedItems = newIsues
                .OrderBy(item => Math.Abs((item.CreatedAt - DateTime.Now).Value.Ticks))
                .ToList();

            var sortedItemsinBTn = sortedItems
                .Where(workItem =>
                    workItem.Items.Any(item =>
                        item.Key == "status" && item.Value == "new" && item.IsDelite == false
                    )
                )
                .ToList();

            sortedItemsinBTn = sortedItemsinBTn.Where(btnf => btnf.IsDelite == false).ToList();
            sortedItemsinBTn = sortedItemsinBTn.Where(btnf => btnf.isDone == true).ToList();

            int nRows = 10;
            int iPagination = 0;
            List<InlineKeyboardButton> paginationButtons = new List<InlineKeyboardButton>();

            var paginItemList = new List<MyWorkEnty>();
            if (sortedItemsinBTn.Count() > nRows)
            {
                iPagination = (int)getIdBDFromCallBack_kostil(
                    myChat.ChatMessages.LastOrDefault().MessageContent,
                    "nf"
                );

                if (iPagination == 0)
                {
                    iPagination = (int)getIdBDFromCallBack_kostil(
                        myChat.ChatMessages.LastOrDefault().MessageContent,
                        "nb"
                    );
                }

                if (iPagination - nRows >= 0)
                {
                    paginationButtons.Add(
                        new InlineKeyboardButton()
                        {
                            Text = "⬅️",
                            CallbackData =
                                $"{myChat.CurentMenu.EntyCode}{myChat.CurentMenu.Id}|"
                                + $"|nb{iPagination - nRows}|",
                        }
                    );
                }

                if (iPagination + nRows < sortedItemsinBTn.Count)
                {
                    paginationButtons.Add(
                        new InlineKeyboardButton()
                        {
                            Text = "➡️",
                            CallbackData =
                                $"{myChat.CurentMenu.EntyCode}{myChat.CurentMenu.Id}|"
                                + $"|nf{iPagination + nRows}|",
                        }
                    );
                }

                foreach (var isue in sortedItemsinBTn)
                {
                    int iv = sortedItemsinBTn.IndexOf(isue);

                    if (!((iPagination) <= sortedItemsinBTn.IndexOf(isue)))
                    {
                        continue;
                    }
                    if (!(sortedItemsinBTn.IndexOf(isue) < (iPagination + nRows)))
                    {
                        continue;
                    }

                    var callback =
                        $"{sampleBtn.NextMenu.EntyCode}{sampleBtn.NextMenu.Id}|{isue.ParentChat.EntyCode}{isue.ParentChat.Id}|{isue.EntyCode}{isue.Id}";

                    var btnContent =
                        $"{isue.Id}) {isue.CodeType} {isue.ParentChat.ChatUsers.FirstOrDefault().TeleUserName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleFirstName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleLasttName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleId.ToString()} {isue.CreatedAt.Value.Date.ToString("dd.MM.yy")}";

                    _ = ButtonMenu2.AddNewRow(
                        new InlineKeyboardButton() { Text = btnContent, CallbackData = callback }
                    );
                }

                ButtonMenu2.AddNewRow(paginationButtons.ToArray());
            }
            else
            {
                foreach (var isue in sortedItemsinBTn)
                {
                    if (isue.IsDelite == true || isue.isDone == false)
                        continue;
                    var callback =
                        $"{sampleBtn.NextMenu.EntyCode}{sampleBtn.NextMenu.Id}|{isue.ParentChat.EntyCode}{isue.ParentChat.Id}|{isue.EntyCode}{isue.Id}";

                    var btnContent =
                        $"{isue.Id}) {isue.CodeType} {isue.ParentChat.ChatUsers.FirstOrDefault().TeleUserName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleFirstName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleLasttName
                ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleId.ToString()} {isue.CreatedAt.Value.Date.ToString("dd.MM.yy")}";

                    _ = ButtonMenu2.AddNewRow(
                        new InlineKeyboardButton() { Text = btnContent, CallbackData = callback }
                    );
                }
            }

            foreach (var isue in sortedItemsinBTn)
            {
                var callback =
                    $"{sampleBtn.NextMenu.EntyCode}{sampleBtn.NextMenu.Id}|{isue.ParentChat.EntyCode}{isue.ParentChat.Id}|{isue.EntyCode}{isue.Id}";

                var btnContent =
                    $"{isue.Id}) {isue.CodeType} {isue.ParentChat.ChatUsers.FirstOrDefault().TeleUserName
               ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleFirstName
               ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleLasttName
               ?? isue.ParentChat.ChatUsers.FirstOrDefault().TeleId.ToString()} {isue.CreatedAt.Value.Date.ToString("dd.MM.yy")}";

                _ = ButtonMenu3.AddNewRow(
                    new InlineKeyboardButton() { Text = btnContent, CallbackData = callback }
                );
            }
            var tryPagin = TeleTools.GetPaginateButtons(myChat, ButtonMenu3, 10);

            var textcontent = myChat.CurentMenu.Content;
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: tryPagin,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("User_Newisue_menu_Info")]
        [Obsolete]
        public async Task OnStart_User_Newisue_menu_Info(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            int isueId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "MWE"
            );
            int chatId = (int)getIdBDFromCallBack_kostil(
                myChat.ChatMessages.LastOrDefault().MessageContent,
                "Ch"
            );
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var semple = myChat.CurentMenu.Inputs.FirstOrDefault(i =>
                i.InputType == "DynamicSample"
            );

            ButtonMenu.AddNewRow(
                new InlineKeyboardButton()
                {
                    CallbackData = myChat.ChatMessages.LastOrDefault().MessageContent + "|gw",
                    Text = semple.InputContent,
                }
            );

            var curentIsue = db
                .MyChats.FirstOrDefault(c => c.Id == chatId)
                .WorkItems.FirstOrDefault(i => i.Id == isueId);
            if (myChat.ChatMessages.LastOrDefault().MessageContent.Contains("|gw"))
            {
                var statusNew = curentIsue.Items.FirstOrDefault(i =>
                    i.Key == "status" && i.Value == "new" && i.IsDelite == false
                );
                if (statusNew != null)
                {
                    statusNew.IsDelite = true;
                    var sourseStatus = new MyWorkDetales()
                    {
                        CreatedAt = DateTime.Now,
                        Key = "status",
                        Value = "in progress",
                    };
                    curentIsue.Items.Add(sourseStatus);
                    await db.SaveChangesAsync();
                }
            }
            var textContent = myChat.CurentMenu.Content;
            var userNameF = userLink(curentIsue.ParentChat.ChatUsers.FirstOrDefault());
            if (curentIsue.ParentChat.ChatUsers.FirstOrDefault().TeleUserName != null)
            {
                userNameF += $"\n@{curentIsue.ParentChat.ChatUsers.FirstOrDefault().TeleUserName}";
            }

            var email = curentIsue
                .Items?.LastOrDefault(i => i.Key == "email" && i.IsDelite == false)
                ?.Value;
            if (email != null)
            {
                userNameF += $"\n<b>Email:</b> {email}";
            }

            textContent = textContent.Replace("{userName}", userNameF);

            textContent = textContent.Replace("{isueId}", curentIsue.Id.ToString());
            textContent = textContent.Replace("{isueType}", curentIsue.CodeType);
            textContent = textContent.Replace(
                "{texIsue}",
                curentIsue
                    .Items.LastOrDefault(i => i.Key == "UserMessage" && i.IsDelite == false)
                    .Value
                    ?? curentIsue
                        .Items.FirstOrDefault(i => i.Key == "UserMessage" && i.IsDelite == false)
                        .Value
            );
            textContent = textContent.Replace(
                "{nPhoto}",
                curentIsue.Items.Where(i => i.Key == "photo").ToList().Count().ToString()
            );

            // Фоточки оотправляем
            try
            {
                var albumMessages = await TeleTools.SendPhotoForIsue(
                    myChat,
                    client,
                    cancellationToken,
                    db,
                    curentIsue
                );
                myChat.AddMessages(albumMessages);
                await db.SaveChangesAsync();
            }
            catch (System.Exception ex) { }

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textContent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [StartMenu("User_InWorkisue_menu_list")]
        [Obsolete]
        public async Task OnStart_User_InWorkisue_menu_list(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var textcontent = myChat.CurentMenu.Content;
            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: textcontent,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }
        #endregion


        [StartMenu("ChanngeStartMenuPhoto")]
        [Obsolete]
        public async Task OnStart_ChanngeStartMenuPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "StartMenu_001";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content + $"\n Сейчас фото: {menu.PhotoFormMenu?.Count}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("ChanngeStartMenuPhoto")]
        public async Task OnEndMenu_ChanngeStartMenuPhoto(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                var menyCode = "StartMenu_001";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var photesList = myChat.ChatMessages?.LastOrDefault()?.photo;
                if (photesList.IsNullOrEmpty())
                {
                    return;
                }
                menu.PhotoFormMenu?.AddRange(photesList);
                await db.SaveChangesAsync();
            }
            catch { }
        }

        [StartMenu("ChanngeStartMenuPhoto_delite")]
        [Obsolete]
        public async Task OnStart_ChanngeStartMenuPhoto_delite(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                await myChat.ClearChat(client, cancellationToken);
                var menyCode = "StartMenu_001";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);
                menu.PhotoFormMenu?.RemoveRange(0, menu.PhotoFormMenu.Count);
                await db.SaveChangesAsync();
            }
            catch { }

            var nextMenu = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;
            myChat.SetNextMenu(nextMenu, client, cancellationToken, db);
        }

        #region загрузить фотот предложение


        [StartMenu("Channge_Advice_Photo")]
        [Obsolete]
        public async Task OnStart_Channge_Advice_Photo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "ProcessAdvice_start";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content + $"\n Сейчас фото: {menu.PhotoFormMenu?.Count}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("Channge_Advice_Photo")]
        public async Task OnEndMenu_Channge_Advice_Photo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                var menyCode = "ProcessAdvice_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var photesList = myChat.ChatMessages?.LastOrDefault()?.photo;
                if (photesList.IsNullOrEmpty())
                {
                    return;
                }
                menu.PhotoFormMenu?.AddRange(photesList);
                await db.SaveChangesAsync();
            }
            catch { }
        }

        [StartMenu("Channge_Advice_Photo_delite")]
        [Obsolete]
        public async Task OnStart_Channge_Advice_Photo_delite(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                await myChat.ClearChat(client, cancellationToken);
                var menyCode = "ProcessAdvice_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);
                menu.PhotoFormMenu?.RemoveRange(0, menu.PhotoFormMenu.Count);
                await db.SaveChangesAsync();
            }
            catch { }

            var nextMenu = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;
            myChat.SetNextMenu(nextMenu, client, cancellationToken, db);
        }
        #endregion

        #region загрузить фотот вопрос


        [StartMenu("Channge_Qwestion_Photo")]
        [Obsolete]
        public async Task OnStart_Channge_Qwestion_Photo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "ProcessAskQwes_start";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content + $"\n Сейчас фото: {menu.PhotoFormMenu?.Count}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("Channge_Qwestion_Photo")]
        public async Task OnEndMenu_Channge_Qwestion_Photo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                var menyCode = "ProcessAskQwes_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var photesList = myChat.ChatMessages?.LastOrDefault()?.photo;
                if (photesList.IsNullOrEmpty())
                {
                    return;
                }
                menu.PhotoFormMenu?.AddRange(photesList);
                await db.SaveChangesAsync();
            }
            catch { }
        }

        [StartMenu("Channge_Qwestion_Photo_delite")]
        [Obsolete]
        public async Task OnStart_Channge_Qwestion_Photo_delite(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                await myChat.ClearChat(client, cancellationToken);
                var menyCode = "ProcessAskQwes_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);
                menu.PhotoFormMenu?.RemoveRange(0, menu.PhotoFormMenu.Count);
                await db.SaveChangesAsync();
            }
            catch { }

            var nextMenu = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;
            myChat.SetNextMenu(nextMenu, client, cancellationToken, db);
        }
        #endregion




        #region загрузить фотот ошибки


        [StartMenu("Channge_Exeption_Photo")]
        [Obsolete]
        public async Task OnStart_Channge_Exeption_Photo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "ProcessExption_start";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: myChat.CurentMenu.Content + $"\n Сейчас фото: {menu.PhotoFormMenu?.Count}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("Channge_Exeption_Photo")]
        public async Task OnEndMenu_Channge_Exeption_Photo(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                var menyCode = "ProcessExption_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var photesList = myChat.ChatMessages?.LastOrDefault()?.photo;
                if (photesList.IsNullOrEmpty())
                {
                    return;
                }
                menu.PhotoFormMenu?.AddRange(photesList);
                await db.SaveChangesAsync();
            }
            catch { }
        }

        [StartMenu("Channge_Exeption_Photo_delite")]
        [Obsolete]
        public async Task OnStart_Channge_Exeption_Photo_delite(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                await myChat.ClearChat(client, cancellationToken);
                var menyCode = "ProcessExption_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);
                menu.PhotoFormMenu?.RemoveRange(0, menu.PhotoFormMenu.Count);
                await db.SaveChangesAsync();
            }
            catch { }

            var nextMenu = myChat
                .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                .NextMenu;
            myChat.SetNextMenu(nextMenu, client, cancellationToken, db);
        }

        #endregion

        #region исправляем текст
        [StartMenu("Channge_Exeption_TextMenu_ProcessAdvice_start")]
        [Obsolete]
        public async Task OnStart_Channge_Exeption_TextMenu_ProcessAdvice_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "ProcessAdvice_start";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            string messageText = myChat.CurentMenu.Content.Replace("{text}", menu.Content);

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: messageText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("Channge_Exeption_TextMenu_ProcessAdvice_start")]
        public async Task OnEndMenu_Channge_Exeption_TextMenu_ProcessAdvice_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                if (myChat.ChatMessages?.LastOrDefault()?.isComand == true)
                {
                    return;
                }

                var menyCode = "ProcessAdvice_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var newTeext = myChat.ChatMessages?.LastOrDefault()?.MessageContent;

                menu.Content = newTeext;
                await db.SaveChangesAsync();
            }
            catch { }
        }

        [StartMenu("Channge_Exeption_TextMenu_ProcessAskQwes_start")]
        [Obsolete]
        public async Task OnStart_Channge_Exeption_TextMenu_ProcessAskQwes_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "ProcessAskQwes_start";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            string messageText = myChat.CurentMenu.Content.Replace("{text}", menu.Content);

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: messageText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("Channge_Exeption_TextMenu_ProcessAskQwes_start")]
        public async Task OnEndMenu_Channge_Exeption_TextMenu_ProcessAskQwes_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                if (myChat.ChatMessages?.LastOrDefault()?.isComand == true)
                {
                    return;
                }

                var menyCode = "ProcessAskQwes_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var newTeext = myChat.ChatMessages?.LastOrDefault()?.MessageContent;

                menu.Content = newTeext;
                await db.SaveChangesAsync();
            }
            catch { }
        }

        [StartMenu("Channge_Exeption_TextMenu_ProcessExption_start")]
        [Obsolete]
        public async Task OnStart_Channge_Exeption_TextMenu_ProcessExption_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            await myChat.ClearChat(client, cancellationToken);

            var menyCode = "ProcessExption_start";
            var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

            var ButtonMenu = TeleTools.GetOneCallomMenu(myChat.CurentMenu.Inputs); // Получаем кнопки для меню

            string messageText = myChat.CurentMenu.Content.Replace("{text}", menu.Content);

            var message = await client.SendTextMessageAsync(
                chatId: myChat.TeleChatId,
                text: messageText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: ButtonMenu,
                cancellationToken: cancellationToken,
                disableNotification: false
            );

            var myMessage = MyMessage.CreateMyMessageFroMessage(message);
            myMessage.IsIncomingMessage = false;
            myMessage.NeedToDelite = true;
            myChat.AddMessage(myMessage);
            myChat.CurentMessageIdForMenu = null;
            db.SaveChanges();
        }

        [EndMenu("Channge_Exeption_TextMenu_ProcessExption_start")]
        public async Task OnEndMenu_Channge_Exeption_TextMenu_ProcessExption_start(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            try
            {
                if (myChat.ChatMessages?.LastOrDefault()?.isComand == true)
                {
                    return;
                }

                var menyCode = "ProcessExption_start";
                var menu = db.MyMenus.FirstOrDefault(i => i.MenuCode == menyCode);

                var newTeext = myChat.ChatMessages?.LastOrDefault()?.MessageContent;

                menu.Content = newTeext;
                await db.SaveChangesAsync();
            }
            catch { }
        }
        #endregion


        private string userLink(MyUser user)
        {
            string name =
                user.TeleUserName
                ?? user.TeleFirstName
                ?? user.TeleLasttName
                ?? user.TeleId.ToString();

            return $"<a href=\"tg://user?id={user.TeleId}\">{name}</a>";
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StartMenuAttribute : Attribute
    {
        public string MenuCode { get; }

        public StartMenuAttribute(string menuCode)
        {
            MenuCode = menuCode;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EndMenuAttribute : Attribute
    {
        public string MenuCode { get; }

        public EndMenuAttribute(string menuCode)
        {
            MenuCode = menuCode;
        }
    }
}
