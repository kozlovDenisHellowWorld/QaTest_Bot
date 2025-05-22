using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Web_test_bot.DbTools;
using Web_test_bot.DbTools.DbObjeckts;
using Web_test_bot.SerialiseObjects;

namespace Web_test_bot.BotTools
{
    public class TeleBotClient
    {
        private MainСonfiguration _config { set; get; } = null;
        public TelegramBotClient TeleClient { get; }
        private static CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private bool _disposed;

        // ... остальной код конструктора ...

        #region Dispose Pattern


        public void Dispose()
        {
            if (_disposed)
                return;

            // Отменяем все операции
            _cancellationTokenSource.Cancel();

            // Отписываемся от событий
            TeleClient.OnError -= OnError;
            TeleClient.OnMessage -= OnMessage;
            TeleClient.OnUpdate -= OnUpdate;

            _cancellationTokenSource.Dispose();
            _disposed = true;
            endBot();
            GC.SuppressFinalize(this);
        }

        ~TeleBotClient() => Dispose();

        #endregion

        public TeleBotClient(IConfigurationRoot? botConfig)
        {
            TeleTools.DeleteLogFile();
            _config = new MainСonfiguration(botConfig);
            if (_config.BotToken == null)
                return;
            TeleClient = new TelegramBotClient(
                _config.BotToken,
                cancellationToken: _cancellationTokenSource.Token
            );

            try
            {
                TeleClient
                    .DeleteWebhookAsync(cancellationToken: _cancellationTokenSource.Token)
                    .Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении вебхука: {ex.Message}");
            }
            var config = LoadConfig.LoadBotConfig(LoadConfig.botInitPath);
            LoadConfig.LoadDefoltsDB2(config, true, true);

            TeleClient.OnError += OnError;
            TeleClient.OnMessage += OnMessage;
            TeleClient.OnUpdate += OnUpdate;

            Task.Run(() => CheckAndClearMessages());

            // Подписка на события завершения приложения
            ///ddddd
        }

        private async Task CheckAndClearMessages()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromHours(4), _cancellationTokenSource.Token);
                }
                catch (TaskCanceledException) { }
                catch (System.Exception ex)
                {
                    Console.WriteLine("ex");
                }
            }
        }

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception); // just dump the exception to the console
        }

        // method that handle messages received by the bot:
        async Task OnMessage(Message msg, UpdateType type)
        {
            Console.WriteLine($"\n\n\n");
            Console.WriteLine(
                $"🤖: start updating------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var IncumingUpdate = DbTools.DbObjeckts.MyUpdate.CreateNewMyUpdateFromUpdate(msg);
            Console.WriteLine(
                $"🤖:  CreateNewMyUpdateFromUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            if (IncumingUpdate == null)
            {
                var message = await TeleClient.SendTextMessageAsync(
                    chatId: msg.Chat.Id,
                    text: "Что то пошло не так /start",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: _cancellationTokenSource.Token,
                    disableNotification: false
                );
                return;
            }

            var IncumingChat = DbTools.DbObjeckts.MyChat.CreateMyChatFromUpdate(msg);
            Console.WriteLine(
                $"🤖:  CreateMyChatFromUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var IncumingUser = DbTools.DbObjeckts.MyUser.CreteUserFromUpdate(msg);
            Console.WriteLine(
                $"🤖:  CreteUserFromUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var IncumingMessage = MyMessage.CreateMyMessageFroMessage(msg);
            Console.WriteLine(
                $"🤖:  CreateMyMessageFroMessage------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var avtorisateChat = new MyAutorisation(
                IncumingUpdate,
                IncumingChat,
                IncumingUser,
                IncumingMessage
            );
            Console.WriteLine(
                $"🤖:  ew MyAutorisation------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var isAvtorisate = await avtorisateChat.StartAutorisation();
            Console.WriteLine(
                $"🤖:  StartAutorisation------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            if (isAvtorisate != true)
            {
                var message = await TeleClient.SendTextMessageAsync(
                    chatId: IncumingChat.TeleChatId,
                    text: "Пожалуйста нажми на /start",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: _cancellationTokenSource.Token,
                    disableNotification: false
                );
                return;
            }
            Console.WriteLine(
                $"🤖:  isAvtorisate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var updatetHendler = new IncumingUpdateHendler(
                IncumingUpdate,
                IncumingChat,
                IncumingUser,
                IncumingMessage
            );
            Console.WriteLine(
                $"🤖:  new IncumingUpdateHendler end------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            updatetHendler.StartUpdateHendling();
            Console.WriteLine(
                $"🤖:  new StartUpdateHendling end------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            await updatetHendler.ProcessHendler(TeleClient, _cancellationTokenSource.Token);
            Console.WriteLine(
                $"🤖:  new ProcessHendler end------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );
        }

        // method that handle other types of updates received by the bot:
        async Task OnUpdate(Update update)
        {
            Console.WriteLine($"\n\n\n");

            Console.WriteLine(
                $"🤖: start updating------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );
            var curentUpdate = DbTools.DbObjeckts.MyUpdate.CreateNewUpdate(update);
            Console.WriteLine(
                $"🤖: CreateNewUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );
            if (curentUpdate == null && update.Type == UpdateType.CallbackQuery)
            {
                var message = await TeleClient.SendTextMessageAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    text: "Что то пошло не так /start",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: _cancellationTokenSource.Token,
                    disableNotification: false
                );
                return;
            }

            var curentChat = DbTools.DbObjeckts.MyChat.CreateMyChatFromUpdate(update);
            Console.WriteLine(
                $"🤖: CreateMyChatFromUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var curentUser = DbTools.DbObjeckts.MyUser.CreteUserFromUpdate(update);
            Console.WriteLine(
                $"🤖:  CreteUserFromUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var IncumingMessage = MyMessage.CreateMyMessageFromUpdate(update);
            Console.WriteLine(
                $"🤖:  CreateMyMessageFromUpdate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var avtorisateChat = new MyAutorisation(
                curentUpdate,
                curentChat,
                curentUser,
                IncumingMessage
            );
            Console.WriteLine(
                $"🤖:  new MyAutorisation------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var updatetHendler = new IncumingUpdateHendler(
                curentUpdate,
                curentChat,
                curentUser,
                IncumingMessage
            );

            Console.WriteLine(
                $"🤖:  new IncumingUpdateHendler------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            var isAvtorisate = await avtorisateChat.StartAutorisation();
            Console.WriteLine(
                $"🤖:  new isAvtorisate------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            if (isAvtorisate != true)
            {
                var message = await TeleClient.SendTextMessageAsync(
                    chatId: curentChat.TeleChatId,
                    text: "Пожалуйста нажми на /start",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: _cancellationTokenSource.Token,
                    disableNotification: false
                );
                return;
            }
            Console.WriteLine(
                $"🤖:  new isAvtorisate true------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );

            await updatetHendler.StartUpdateHendling();
            await updatetHendler.ProcessHendler(TeleClient, _cancellationTokenSource.Token);

            Console.WriteLine(
                $"🤖:  ProcessHendler------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );
            //-----
        }

        public async Task endBot()
        {
            Console.WriteLine("dddddsdsd");
            using (var db = new MyDbContext())
            {
                try
                {
                    foreach (var chat in db.MyChats)
                    {
                        TelegramBotClient endClient = new TelegramBotClient(_config.BotToken);
                        CancellationTokenSource cancellationTokenSourceEnd =
                            new CancellationTokenSource();

                        await chat.ClearChat(client: endClient, cancellationTokenSourceEnd.Token);

                        var messageText = await endClient.SendTextMessageAsync(
                            chatId: chat.TeleChatId,
                            text: "Что то пошло не так!\n Нажми /start",
                            cancellationToken: cancellationTokenSourceEnd.Token,
                            disableNotification: false
                        );
                        await Task.Delay(TimeSpan.FromMilliseconds(10));
                    }
                }
                catch (System.Exception ex)
                {
                    // TODO
                }
            }
        }
    }
}
