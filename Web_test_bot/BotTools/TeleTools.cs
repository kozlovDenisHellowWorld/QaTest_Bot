using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Web_test_bot.DbTools;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.BotTools
{
    public class TeleTools
    {
        public static InlineKeyboardMarkup GetOneCallomMenu(List<MyInput> buttons)
        {
            var result = new InlineKeyboardMarkup();

            foreach (var item in buttons.Where(i => i.InputType == "StaticInput"))
            {
                string callback = $"{item.NextMenu.EntyCode}{item.NextMenu.Id}";

                result.AddNewRow(
                    new InlineKeyboardButton() { Text = item.InputContent, CallbackData = callback }
                );
            }

            return result;
        }

        public static async Task<List<MyMessage>> SendPhotoForMenu(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db
        )
        {
            List<MyMessage> result = new List<MyMessage>();
            try
            {
                var photoSizeList = myChat.CurentMenu.PhotoFormMenu;
                // Проверяем, есть ли фотографии для отправки
                if (photoSizeList == null || !photoSizeList.Any())
                {
                    return null;
                }
                var mediaGroup = new List<IAlbumInputMedia>();
                foreach (var photo in photoSizeList)
                {
                    // Предполагается, что photo содержит путь к файлу или поток
                    mediaGroup.Add(new InputMediaPhoto(new InputFileId(photo.FileId)));
                }
                // Отправляем альбом
                var messages = await client.SendMediaGroupAsync(
                    chatId: myChat.TeleChatId, // ID чата, куда отправляем
                    media: mediaGroup, // Список медиа
                    cancellationToken: cancellationToken,
                    disableNotification: true // Токен отмены
                );

                foreach (var message in messages.ToList())
                {
                    var mymsg = MyMessage.CreateMyMessageFroMessage(message);
                    mymsg.IsIncomingMessage = false;
                    mymsg.NeedToDelite = true;
                    result.Add(mymsg);
                }

                return result;
            }
            catch
            {
                ConssolWriting(
                    $"⚠️ не получиилось отправить фото в чате TeleID:{myChat.TeleChatId} в меню {myChat.CurentMenu.MenuCode}"
                );
            }
            return null;
        }

        public static async Task<List<MyMessage>> SendPhotoForIsue(
            MyChat myChat,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            MyDbContext db,
            MyWorkEnty? isue
        )
        {
            List<MyMessage> result = new List<MyMessage>();
            try
            {
                var photoIsue = isue
                    .Items.Where(i => i.Key == "photo" && i.IsDelite == false)
                    .ToList();

                var photoSizeList = photoIsue.SelectMany(i => i.TelegramPfotoes);

                // Проверяем, есть ли фотографии для отправки
                if (photoSizeList == null || !photoSizeList.Any())
                {
                    return null;
                }
                var mediaGroup = new List<IAlbumInputMedia>();
                foreach (var photo in photoSizeList)
                {
                    // Предполагается, что photo содержит путь к файлу или поток
                    mediaGroup.Add(
                        new InputMediaPhoto(new InputFileId(photo.FileId))
                        {
                            Caption = "Фото к заявке",
                        }
                    );
                }
                // Отправляем альбом
                var messages = await client.SendMediaGroupAsync(
                    chatId: myChat.TeleChatId, // ID чата, куда отправляем
                    media: mediaGroup, // Список медиа
                    cancellationToken: cancellationToken // Токен отмены
                );

                foreach (var message in messages.ToList())
                {
                    var mymsg = MyMessage.CreateMyMessageFroMessage(message);
                    mymsg.IsIncomingMessage = false;
                    mymsg.NeedToDelite = true;
                    result.Add(mymsg);
                }

                return result;
            }
            catch
            {
                ConssolWriting(
                    $"⚠️ не получиилось отправить фото в чате TeleID:{myChat.TeleChatId} в меню {myChat.CurentMenu.MenuCode}"
                );
            }
            return null;
        }

        public static void ConssolWriting(string message)
        {
            Console.WriteLine($"🤖: ({DateTime.Now.ToString("HH:mm:ss ff")}):: {message}");
            LogToFile($"🤖;({DateTime.Now.ToString("HH:mm:ss ff")});{message}");
        }

        private static void LogToFile(string message)
        {
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.csv");
            try
            {
                // Открываем файл для записи (если файла нет, он будет создан)
                using (StreamWriter writer = new StreamWriter(logFilePath, true)) // true - добавление в конец файла
                {
                    writer.WriteLine(message); // Записываем сообщение в файл
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем возможные ошибки
                Console.WriteLine($"Ошибка при записи в файл: {ex.Message}");
                DeleteLogFile();
            }
        }

        public static void DeleteLogFile()
        {
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.csv");

            try
            {
                // Проверяем, существует ли файл
                if (System.IO.File.Exists(logFilePath))
                {
                    // Удаляем файл
                    System.IO.File.Delete(logFilePath);
                    Console.WriteLine("Файл log успешно удалён.");
                }
                else
                {
                    Console.WriteLine("Файл log не существует.");
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем возможные ошибки
                Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
            }
        }

        public static async Task<Message> SendMenuTextOrPhoto(
            ITelegramBotClient client,
            MyChat myChat,
            CancellationToken cancellationToken,
            IReplyMarkup? ButtonMenu,
            MyDbContext db,
            bool NeedToDelite,
            bool isThisMessageIsEditeMenu
        )
        {
            try
            {
                try
                {
                    if (myChat.CurentMenu.PhotoFormMenu.Any())
                    {
                        var messMessagePhoto = await client.SendPhotoAsync(
                            chatId: myChat.TeleChatId,
                            caption: myChat.CurentMenu.Content,
                            photo: myChat.CurentMenu.PhotoFormMenu.FirstOrDefault().FileId,
                            replyMarkup: ButtonMenu,
                            cancellationToken: cancellationToken,
                            disableNotification: false
                        );

                        var myMessagePhoto = MyMessage.CreateMyMessageFroMessage(messMessagePhoto);
                        myMessagePhoto.IsIncomingMessage = false;
                        myMessagePhoto.NeedToDelite = NeedToDelite;
                        myChat.AddMessage(myMessagePhoto);

                        await db.SaveChangesAsync();

                        return messMessagePhoto;
                    }
                    else
                    {
                        var messageText = await client.SendTextMessageAsync(
                            chatId: myChat.TeleChatId,
                            text: myChat.CurentMenu.Content,
                            replyMarkup: ButtonMenu,
                            cancellationToken: cancellationToken,
                            disableNotification: false
                        );

                        var myMessageText = MyMessage.CreateMyMessageFroMessage(messageText);

                        myMessageText.IsIncomingMessage = false;
                        myMessageText.NeedToDelite = NeedToDelite;

                        myChat.AddMessage(myMessageText);
                        myChat.CurentMessageIdForMenu = myMessageText.TeleMessageId;
                        await db.SaveChangesAsync();

                        return messageText;
                    }
                }
                catch
                {
                    var messageText = await client.SendTextMessageAsync(
                        chatId: myChat.TeleChatId,
                        text: myChat.CurentMenu.Content,
                        replyMarkup: ButtonMenu,
                        cancellationToken: cancellationToken,
                        disableNotification: false
                    );

                    var myMessageText = MyMessage.CreateMyMessageFroMessage(messageText);

                    myMessageText.IsIncomingMessage = false;
                    myMessageText.NeedToDelite = NeedToDelite;

                    myChat.AddMessage(myMessageText);
                    myChat.CurentMessageIdForMenu = myMessageText.TeleMessageId;
                    await db.SaveChangesAsync();

                    return messageText;
                }
            }
            catch (System.Exception ex)
            {
                var messageText = await client.SendTextMessageAsync(
                    chatId: myChat.TeleChatId,
                    text: "Что то пошло не так!\n Нажми /start",
                    cancellationToken: cancellationToken,
                    disableNotification: false
                );

                var myMessageText = MyMessage.CreateMyMessageFroMessage(messageText);

                myMessageText.IsIncomingMessage = false;
                myMessageText.NeedToDelite = NeedToDelite;

                myChat.AddMessage(myMessageText);
                myChat.CurentMessageIdForMenu = myMessageText.TeleMessageId;
                await db.SaveChangesAsync();

                return messageText;
            }
        }

        public static InlineKeyboardMarkup GetPaginateButtons(
            MyChat currentChat,
            InlineKeyboardMarkup allButtons,
            int nRows
        )
        {
            // 1. Разделяем кнопки на группы
            InlineKeyboardButton[] backButtonRow = null;
            InlineKeyboardButton[] paginationControls = null;
            List<InlineKeyboardButton[]> contentButtons = new List<InlineKeyboardButton[]>();

            foreach (var row in allButtons.InlineKeyboard)
            {
                bool isBackButton = row.Any(b =>
                    b.Text.Equals("назад", StringComparison.OrdinalIgnoreCase)
                );
                bool isPagination = row.Any(b =>
                    (b.CallbackData?.Contains("|nb") ?? false)
                    || (b.CallbackData?.Contains("|nf") ?? false)
                );

                if (isBackButton)
                {
                    backButtonRow = row.ToArray();
                }
                else if (isPagination)
                {
                    paginationControls = row.ToArray();
                }
                else
                {
                    contentButtons.Add(row.ToArray());
                }
            }

            // 2. Определяем текущую страницу
            int currentPage = 0;
            var lastMessage = currentChat.ChatMessages.LastOrDefault();
            currentPage = GetIdBDFromCallBack_kostil(lastMessage.MessageContent, "nf");
            if (currentPage == 0)
            {
                currentPage = GetIdBDFromCallBack_kostil(lastMessage.MessageContent, "nb");
            }
            // 3. Рассчитываем диапазон отображаемых кнопок
            int totalItems = contentButtons.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / nRows);
            currentPage = Math.Clamp(currentPage, 0, totalPages - 1);

            int start = currentPage * nRows;
            int end = Math.Min(start + nRows, totalItems);

            // 4. Формируем итоговую клавиатуру
            var resultButtons = new List<InlineKeyboardButton[]>();

            // Добавляем кнопку "Назад" в начале
            if (backButtonRow != null)
            {
                resultButtons.Add(backButtonRow);
            }

            // Добавляем контентные кнопки для текущей страницы
            for (int i = start; i < end; i++)
            {
                resultButtons.Add(contentButtons[i]);
            }

            // Добавляем элементы пагинации в конец
            if (paginationControls != null)
            {
                resultButtons.Add(paginationControls);
            }
            else if (totalItems > nRows)
            {
                //если totalItems > 10 но paginationControls==null  добавляем новые
                List<InlineKeyboardButton> paginationRow = new List<InlineKeyboardButton>();

                if (currentPage > 0)
                {
                    paginationRow.Add(
                        new InlineKeyboardButton("⬅️")
                        {
                            CallbackData =
                                $"{currentChat.CurentMenu.EntyCode}{currentChat.CurentMenu.Id}|nb{currentPage - 1}",
                        }
                    );
                }

                // Кнопка "Вперед"
                if (currentPage < totalPages - 1)
                {
                    paginationRow.Add(
                        new InlineKeyboardButton("➡️")
                        {
                            CallbackData =
                                $"{currentChat.CurentMenu.EntyCode}{currentChat.CurentMenu.Id}|nf{currentPage + 1}",
                        }
                    );
                }

                if (paginationRow.Any())
                    resultButtons.Add(paginationRow.ToArray());
            }

            return new InlineKeyboardMarkup(resultButtons);
        }

        public static int GetIdBDFromCallBack_kostil(string caallBack, string entCode)
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
    }
}
