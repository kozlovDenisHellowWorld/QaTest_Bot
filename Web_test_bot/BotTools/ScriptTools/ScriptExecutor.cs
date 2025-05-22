using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.BotTools.ScriptTools
{
    public class ScriptExecutor
    {
        private readonly Scripts _scripts;

        private static readonly ConcurrentDictionary<string, MethodInfo> _startCache = new();
        private static readonly ConcurrentDictionary<string, MethodInfo> _endCache = new();

        public ScriptExecutor(Scripts scripts)
        {
            _scripts = scripts;

            InitializeCache();
        }

        private void InitializeCache()
        {
            foreach (var method in typeof(Scripts).GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes<StartMenuAttribute>())
                {
                    _startCache.TryAdd(attr.MenuCode, method);
                }

                foreach (var attr in method.GetCustomAttributes<EndMenuAttribute>())
                {
                    _endCache.TryAdd(attr.MenuCode, method);
                }
            }
        }

        public async Task ExecuteStartMenu(string menuCode, MyChat myyDbChat,Telegram.Bot.TelegramBotClient client, CancellationToken cancellationToken,  DbContext db)
        {
            await ExecuteScript(menuCode, _startCache, myyDbChat, "Start",client,cancellationToken, db);
        }

        public async Task ExecuteEndMenu(string menuCode, MyChat myyDbChat,Telegram.Bot.TelegramBotClient client, CancellationToken cancellationToken, DbContext db)
        {
            await ExecuteScript(menuCode, _endCache, myyDbChat, "End",client,cancellationToken, db);
        }

        private async Task ExecuteScript(
            string menuCode,
            ConcurrentDictionary<string, MethodInfo> cache,
            MyChat myDBChat,
            string scriptType,
            Telegram.Bot.TelegramBotClient client,
            CancellationToken cancellationToken,
            DbContext db
            //
        )
        {
            if (cache.TryGetValue(menuCode, out var method))
            {
                try
                {
                    var parameters = method.GetParameters();
                   
                //    MyChat myChat,  Telegram.Bot.TelegramBotClient client, CancellationToken cancellationToken, DbContext db
                    var args =
                        parameters.Length > 0
                            ? new object[] { myDBChat, client, cancellationToken, db }
                            : null;

                    var result = method.Invoke(_scripts, args);

                    if (result is Task task)
                    {
                        await task;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        ex.InnerException + $"Ошибка выполнения {scriptType} скрипта для {menuCode}"
                    );
                }
            }
            else
            {
                Console.WriteLine($"{scriptType} скрипт для меню {menuCode} не найден");
            }
        }
    }
}
