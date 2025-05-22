using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Web_test_bot.DbTools;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.BotTools
{
    public class MyAutorisation
    {
        private MyUpdate _myNewUpdate { get; }
        private MyChat _myCurentChat { get; }
        private MyUser _myCurenntUser { get; }

        private MyMessage _myCurentMessage { get; }
        MyDbContext db = new MyDbContext();

        public MyAutorisation(
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
        }

        /// <summary>
        ///тут необходимо проверить что за пользователь и  есть ли он в бд
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartAutorisation()
        {
            bool? IsNew = IsNewChat();
            if (IsNew == null)
            {
                return false;
            }
            if (IsNew == true && _myNewUpdate.Type != UpdateType.Message)
            {
                return false;
            }

            if (IsNew.Value == true)
            {
                if (_myCurentChat.TeleChatType == Telegram.Bot.Types.Enums.ChatType.Private)
                {
                    _myCurenntUser.UserType = GetUserTypeForNewUser();

                    _myCurentChat.ChatUsers.Add(_myCurenntUser);
                }
                else
                {
                    return false;
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.MyChats.Add(_myCurentChat);
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Data);
                        await transaction.RollbackAsync();
                    }
                    return true;
                }
            }
            else
            {
                if (_myCurentChat.IsBan == true)
                {
                    return false;
                }

                return true;
            }
        }

        private bool? IsNewChat()
        {
            bool result = true;

            var dbChat = db.MyChats.FirstOrDefault(ch => ch.TeleChatId == _myCurentChat.TeleChatId);
            if (dbChat != null)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Узнаем нтипо пользователя для нового пользователя
        /// </summary>
        /// <returns></returns>
        private MyUserType GetUserTypeForNewUser()
        {
            MyUserType result = db.myUserTypes.FirstOrDefault(ut => ut.IsDefoult == true);
            MyDefoltUser defoult = null;
            try
            {
                if (_myCurenntUser.TeleUserName != null)
                {
                    defoult =
                        db.MyDefoltUsers.Include(du => du.UserType)
                            ? // Здесь указываем навигационное свойство
                            .FirstOrDefault(du =>
                                du.TelegramUserName.Trim().ToLower()
                                == _myCurenntUser.TeleUserName.Trim().ToLower()
                            ) ?? null;
                }
            }
            catch (System.Exception ex)
            {
                defoult = null;
            }

            if (defoult == null)
            {
                defoult = db.MyDefoltUsers.FirstOrDefault(du =>
                    du.TelegramId == _myCurenntUser.TeleId
                );
            }

            if (defoult != null)
            {
                result = defoult?.UserType;
            }

            return result;
        }
    }
}
