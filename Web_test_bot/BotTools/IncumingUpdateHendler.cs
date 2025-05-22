using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Telegram.Bot;
using Telegram.Bot.Types;
using Web_test_bot.DbTools;
using Web_test_bot.DbTools.DbObjeckts;
using Web_test_bot.Migrations;

namespace Web_test_bot.BotTools
{
    public class IncumingUpdateHendler
    {
        private MyUpdate? _myNewUpdate { get; }
        private MyChat? _myCurentChatDB { set; get; }
        private MyChat? _myCurentChat { get; }
        private MyUser? _myCurenntUser { get; }

        private MyMessage? _myCurentMessage { get; }
        MyDbContext db = new MyDbContext();

        public IncumingUpdateHendler(
            MyUpdate myNewUpdate,
            MyChat myCurentChat,
            MyUser myCurenntUser,
            MyMessage myIncomingMessage
        )
        {
            _myNewUpdate = myNewUpdate;
            _myCurentChat = myCurentChat;
            _myCurenntUser = myCurenntUser;
            _myCurentMessage = myIncomingMessage;

            var chats = db.MyChats.ToList();
            var _myCurentChatDB = db.MyChats
            // .Include(cht => cht.ChatUsers)
            //.Include(cht => cht.ChatUpdates)
            .FirstOrDefault(mch => mch.TeleChatId == _myCurentChat.TeleChatId);
        }

        public async Task StartUpdateHendling()
        {
            _myCurentChatDB = db.MyChats.FirstOrDefault(mch =>
                mch.TeleChatId == _myCurentChat.TeleChatId
            );

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    _myCurentChatDB?.AddUpddate(_myNewUpdate);

                    _myCurentChatDB?.AddMessage(_myCurentMessage);
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Data);
                    transaction.Rollback();
                }
            }
        }

        public async Task ProcessHendler(
            TelegramBotClient client,
            CancellationToken cancellationToken
        )
        {
            Console.WriteLine(
                $"🤖:  ProcessHendler- start------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
            );
            if (_myCurentChatDB?.TeleChatType == Telegram.Bot.Types.Enums.ChatType.Private)
            {
                var lastThisChatMessage = _myCurentChatDB.ChatMessages?.LastOrDefault();
                if (lastThisChatMessage == null) { }
                if (lastThisChatMessage?.isComand == true)
                {
                    if (lastThisChatMessage.MessageContent.Contains("/start"))
                    {
                        Console.WriteLine(
                            $"🤖:  ProcessHendler- find nextMeny start------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
                        );
                        //  var ffff = db.MyProcesses.ToList();
                        var nextMeny = _myCurentChatDB
                            .ChatUsers?.FirstOrDefault()
                            ?.UserType?.Process?.Content.FirstOrDefault(m =>
                                m.MenuCode.ToLower().Contains("startmenu")
                            );
                        Console.WriteLine(
                            $"🤖:  ProcessHendler- find nextMeny finish------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
                        );

                        Console.WriteLine(
                            $"🤖:  ProcessHendler- find nextMeny set next------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
                        );

                        await _myCurentChatDB.SetNextMenu(nextMeny, client, cancellationToken, db);
                        Console.WriteLine(
                            $"🤖:  ProcessHendler- find nextMeny set finish------------------{DateTime.Now.ToString("HH:MM HH:mm:ss:ff")}"
                        );

                        //  await _myCurentChatDB.ExecuteMessage();
                    }
                    else if (
                        lastThisChatMessage.MessageContent != ""
                        && lastThisChatMessage.MessageContent.Contains("/") == false
                    )
                    {
                        var nextmenuId = getIdBDFromCallBack_kostil(
                            lastThisChatMessage.MessageContent,
                            "MC"
                        );
                        var testTru= db. MyChats.FirstOrDefault();
                        var nextMeny = _myCurentChatDB
                            .ChatUsers?.FirstOrDefault()
                            ?.UserType?.Process?.Content.FirstOrDefault(m => m.Id == nextmenuId);
                        if (nextMeny == null)
                        {
                            Console.WriteLine(
                                "Ошибка выполнения не нашел по кнопке информацию о меню - ?jxGD"
                            );
                            return;
                        }
                        await _myCurentChatDB.SetNextMenu(nextMeny, client, cancellationToken, db);
                    }
                }
                else if (lastThisChatMessage?.isComand == null)
                {
                    var nextMeny = _myCurentChatDB
                        .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "AwaitingText")
                        .NextMenu;

                    if (nextMeny == null)
                    {
                        nextMeny = _myCurentChatDB
                            .CurentMenu.Inputs.FirstOrDefault(i => i.InputType == "Aktion")
                            .NextMenu;
                    }
                    await _myCurentChatDB.SetNextMenu(nextMeny, client, cancellationToken, db);
                }
            }
        }

        private int getIdBDFromCallBack_kostil(string caallBack, string entCode)
        {
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
