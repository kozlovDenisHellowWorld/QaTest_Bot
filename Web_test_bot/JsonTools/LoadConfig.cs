using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using Web_test_bot.BotTools;
using Web_test_bot.DbTools;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.SerialiseObjects
{
    public class LoadConfig
    {
        public static string botInitPath = "botInst.json";

        public static BotConfig LoadBotConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<BotConfig>(json);
        }

        private static void UpdateProperties<T>(T? DBtarget, T? LoadSource)
        {
            if (DBtarget == null || LoadSource == null)
                return;
            var properties = typeof(T).GetProperties();
            var unUpProps = new HashSet<string>(
                typeof(IDbMyElement).GetProperties().Select(p => p.Name)
            );

            foreach (var property in properties)
            {
                // Пропускаем свойства, которые не нужно обновлять
                if (unUpProps.Contains(property.Name))
                    continue;
                else if (property.Name.Contains("code"))
                    continue;
                if (property.CanWrite)
                {
                    var value = property.GetValue(LoadSource);
                    property.SetValue(DBtarget, value);
                }
            }
        }

        private static List<T>? UpdateProperties<T>(
            DbSet<T>? DBEntyUpdating,
            List<T>? ListLoadSource
        )
            where T : class
        {
            if (DBEntyUpdating == null || ListLoadSource == null)
                return null;

            foreach (var LoadSource in ListLoadSource)
            {
                var existeObj = DBEntyUpdating.ToList().FirstOrDefault(LoadSource);
                if (existeObj == null)
                {
                    DBEntyUpdating.Add(LoadSource);
                }
                var properties = typeof(T).GetProperties();

                var unUpProps = new HashSet<string>(
                    typeof(IDbMyElement).GetProperties().Select(p => p.Name)
                );

                foreach (var property in properties)
                {
                    if (!unUpProps.Contains(property.Name))
                    {
                        var newValue_1 = property.GetValue(existeObj);
                        property.SetValue(existeObj, newValue_1);
                    }

                    //  var newValue = property.GetValue(loadSource);
                    // property.SetValue(existsObj, newValue);
                }
            }

            return DBEntyUpdating.ToList();
        }

        public static async void LoadDefoltsDB2(
            BotConfig? configs,
            bool needToUpdate,
            bool allupdate
        )
        {
            TeleTools.ConssolWriting("⚙️ Начало работы с данными из JSON\n\n\n\n");

            if (!needToUpdate)
            {
                return;
            }

            using (var context = new MyDbContext())
            {
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyChats");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyUpdates");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyUsers");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM myUserTypes");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyDefoltUsers");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyIntupTypes");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyMenuTypes");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyProcesses");





                // // Удаляем данные из всех таблиц
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyDefoltUsers");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyUpdates");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyUsers");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyChats");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyInputTypes");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyMenuTypes");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyProcesses");
                // await context.Database.ExecuteSqlRawAsync("DELETE FROM MyMenuContent");
            }
            TeleTools.ConssolWriting("⚙️ Начало загрузки типов пользователей UserTypes\n");

            using (var context = new MyDbContext()) //myUserTypes
            {
                var dbEnty = context.myUserTypes;
                var newEntys = configs.BotInst.UserTypes;

                TeleTools.ConssolWriting($"⚙️ Найдено типов: {newEntys.Count()}");

                if (allupdate)
                {
                    if (dbEnty.ToList().Count() > 0)
                    {
                        foreach (var newEnty in newEntys)
                        {
                            var existeEntyDb = dbEnty.FirstOrDefault(i =>
                                i.TypeCode == newEnty.TypeCode
                            );
                            if (existeEntyDb != null)
                            {
                                existeEntyDb.TypeName = newEnty.TypeName;
                                existeEntyDb.TypeDiscr = newEnty.TypeDiscr;
                                existeEntyDb.IsDefoult = newEnty.IsDefoult;

                                await context.SaveChangesAsync();
                                TeleTools.ConssolWriting(
                                    $"🔁 Внесены изменения в тип: {newEnty.TypeCode}"
                                );
                            }
                            else
                            {
                                dbEnty.Add(newEnty);
                                await context.SaveChangesAsync();
                                TeleTools.ConssolWriting(
                                    $"✅ Сохранен новый тип: {newEnty.TypeCode}"
                                );
                            }
                        }

                        await context.SaveChangesAsync();
                    }
                }
                else if (dbEnty.ToList().Count() == 0)
                {
                    dbEnty.AddRange(newEntys);
                    await context.SaveChangesAsync();
                    TeleTools.ConssolWriting(
                        $"✅ В бд не было типов пользователей. Все новые ттипы пользоваттелей были сохранены"
                    );
                }
            }
            TeleTools.ConssolWriting($"\n");

            using (var context = new MyDbContext()) //MenuTypes
            {
                TeleTools.ConssolWriting("⚙️ Начало загрузки типов меню MenuTypes\n");

                var dbEnty = context.MyMenuTypes;
                var newEntys = configs.BotInst.MenuTypes;
                TeleTools.ConssolWriting($"⚙️ Найдено типов: {newEntys.Count()}");

                if (allupdate)
                {
                    if (dbEnty.ToList().Count() > 0)
                    {
                        foreach (var newEnty in newEntys)
                        {
                            var existeEntyDb = dbEnty.FirstOrDefault(i =>
                                i.TypeCode == newEnty.TypeCode
                            );
                            if (existeEntyDb != null)
                            {
                                existeEntyDb.TypeName = newEnty.TypeName;
                                existeEntyDb.TypeName = newEnty.TypeName;
                                existeEntyDb.TypeDescription = newEnty.TypeDescription;
                                await context.SaveChangesAsync();
                                TeleTools.ConssolWriting(
                                    $"🔁 Внесены изменения в тип: {newEnty.TypeCode}"
                                );
                            }
                            else
                            {
                                dbEnty.Add(newEnty);
                                await context.SaveChangesAsync();
                                TeleTools.ConssolWriting(
                                    $"✅ Сохранен новый тип: {newEnty.TypeCode}"
                                );
                            }
                        }

                        await context.SaveChangesAsync();
                    }
                    else if (dbEnty.ToList().Count() == 0)
                    {
                        dbEnty.AddRange(newEntys);
                        await context.SaveChangesAsync();
                        TeleTools.ConssolWriting(
                            $"✅ В бд не было типов меню. Все новые типы меню были сохранены"
                        );
                    }
                    // var chekc = dbEnty.ToList();
                }
            }
            TeleTools.ConssolWriting($"\n");

            using (var context = new MyDbContext()) //MyIntupTypes
            {
                TeleTools.ConssolWriting("⚙️ Начало загрузки типов инпутов IntupTypes\n");

                var dbEnty = context.MyIntupTypes;
                var newEntys = configs.BotInst.InputTypes;
                TeleTools.ConssolWriting($"⚙️ Найдено типов: {newEntys.Count()}");

                if (allupdate)
                {
                    foreach (var newEnty in newEntys)
                    {
                        var existeEntyDb = dbEnty.FirstOrDefault(i =>
                            i.TypeCode == newEnty.TypeCode
                        );
                        if (existeEntyDb != null)
                        {
                            existeEntyDb.TypeName = newEnty.TypeName;
                            existeEntyDb.TypeName = newEnty.TypeName;
                            existeEntyDb.TypeDescription = newEnty.TypeDescription;
                            await context.SaveChangesAsync();
                            TeleTools.ConssolWriting(
                                $"🔁 Внесены изменения в тип: {newEnty.TypeCode}"
                            );
                        }
                        else
                        {
                            dbEnty.Add(newEnty);
                            await context.SaveChangesAsync();
                            TeleTools.ConssolWriting($"✅ Сохранен новый тип: {newEnty.TypeCode}");
                        }
                    }

                    await context.SaveChangesAsync();
                }
                else if (dbEnty.ToList().Count() == 0)
                {
                    dbEnty.AddRange(newEntys);
                    await context.SaveChangesAsync();
                    TeleTools.ConssolWriting(
                        $"✅ В бд не было типов инпутов. Все новые типы инпутов были сохранены"
                    );
                }
                //   var chekc = dbEnty.ToList();
            }
            TeleTools.ConssolWriting($"\n");
            using (var context = new MyDbContext()) //myDefoltUser
            {
                TeleTools.ConssolWriting(
                    "⚙️ Начало загрузки пользователей по умолчанию (администраторов)\n"
                );

                var dbEnty = context.MyDefoltUsers;
                var newEntys = configs.DefoltUsers;
                TeleTools.ConssolWriting($"⚙️ Найдено типов: {newEntys.Count()}");

                if (allupdate)
                {
                    foreach (var newEnty in newEntys)
                    {
                        MyDefoltUser? existeEntyDb = dbEnty.FirstOrDefault(i =>
                            i.TelegramUserName == newEnty.TelegramUserName
                        );

                        if (existeEntyDb == null)
                        {
                            existeEntyDb = dbEnty.FirstOrDefault(i =>
                                i.TelegramId == newEnty.TelegramId
                            );
                        }

                        if (existeEntyDb != null)
                        {
                            // await context.SaveChangesAsync();
                        }
                        else
                        {
                            dbEnty.Add(newEnty);
                            TeleTools.ConssolWriting(
                                $"✅ Добавали новго пользователя: {newEnty.TelegramUserName ?? newEnty.TelegramId.ToString()}"
                            );
                        }
                        await context.SaveChangesAsync();
                    }

                    //await context.SaveChangesAsync();
                }
                else if (dbEnty.ToList().Count() == 0)
                {
                    dbEnty.AddRange(newEntys);
                    await context.SaveChangesAsync();
                    TeleTools.ConssolWriting(
                        $"✅ Все пользователи являьться новыми. Добавлены в БД"
                    );
                }
                // var chekc = dbEnty.ToList();
            }

            try
            {
                using (var context = new MyDbContext()) //MyProcesses
                {
                    TeleTools.ConssolWriting("⚙️ Начало загрузки процессов\n");
                    var dbEnty = context.MyProcesses;
                    var newEntyProcess = configs.BotInst.Processes;
                    TeleTools.ConssolWriting($"⚙️ Всего процессов: {newEntyProcess.Count}\n");

                    //  if (dbEnty.ToList().Count() > 0)
                    //  {
                    //      dbEnty.RemoveRange(dbEnty.ToList());
                    //      context.SaveChanges();
                    //  }

                    foreach (var newProcesJs in newEntyProcess)
                    {
                        TeleTools.ConssolWriting(
                            $"⚙️ Начало загрузки/обнавоения процесса с пользовательским доступ: {newProcesJs.UserAccessCode}"
                        );

                        MyProcess? ProcessEntyBase = null;

                        ProcessEntyBase = context.MyProcesses.FirstOrDefault(i =>
                            i.UserAccessCode == newProcesJs.UserAccessCode
                        );

                        if (ProcessEntyBase == null)
                        {
                            TeleTools.ConssolWriting(
                                $"⚙️ Процесс являеться новым (не найден в бд): {newProcesJs.UserAccessCode}"
                            );

                            ProcessEntyBase = new MyProcess()
                            {
                                CreatedAt = DateTime.Now,
                                IsActive = true,
                                ProcessName = newProcesJs.ProcessName,
                                IsDelite = false,
                                UserAccessCode = newProcesJs.UserAccessCode,
                            };
                            context.MyProcesses.Add(ProcessEntyBase);
                            await context.SaveChangesAsync();

                            TeleTools.ConssolWriting(
                                $"✅ Процесс был сохранен: {newProcesJs.UserAccessCode}, но процесс пока пустой!"
                            );
                        }

                        TeleTools.ConssolWriting(
                            $"⚙️ Начало загрузки/обнавоения меню (кол-во: {newProcesJs.Content.Count}) в процесса с пользовательским доступ: {newProcesJs.UserAccessCode}"
                        );
                        foreach (var menuJS in newProcesJs.Content)
                        {
                            TeleTools.ConssolWriting($"⚙️ Меню из json - {menuJS.MenuCode}");
                            MyMenuContent? menuBase = null;

                            if (!ProcessEntyBase.Content.IsNullOrEmpty())
                            {
                                menuBase = ProcessEntyBase.Content?.FirstOrDefault(m =>
                                    m.MenuCode == menuJS.MenuCode
                                );

                                if (menuBase != null)
                                {
                                    menuBase.Content = menuJS.Content;

                                    //  menuBase.MenuCode = menuJS.MenuCode;
                                    menuBase.NeedToDelete = menuJS.NeedToDelete;
                                    menuBase.MenuName = menuJS.MenuName;
                                    menuBase.MenuTypeCode = menuJS.MenuTypeCode;
                                    menuBase.Type = context.MyMenuTypes.FirstOrDefault(i =>
                                        i.TypeCode == menuJS.MenuTypeCode
                                    );

                                    TeleTools.ConssolWriting(
                                        $"🔁 в бд обнавлено - {menuBase.MenuCode}"
                                    );
                                }
                            }

                            if (menuBase == null)
                            {
                                menuBase = new MyMenuContent()
                                {
                                    CreatedAt = DateTime.Now,
                                    IsDelite = false,
                                    Content = menuJS.Content,

                                    MenuCode = menuJS.MenuCode,
                                    NeedToDelete = menuJS.NeedToDelete,
                                    MenuName = menuJS.MenuName,
                                    MenuTypeCode = menuJS.MenuTypeCode,
                                };

                                menuBase.Type = context.MyMenuTypes.FirstOrDefault(i =>
                                    i.TypeCode == menuJS.MenuTypeCode
                                );

                                context.MyMenus.Add(menuBase);

                                await context.SaveChangesAsync();

                                ProcessEntyBase.Content.Add(menuBase);

                                TeleTools.ConssolWriting(
                                    $"✅ в бд добаввлено новое меню - {menuBase.MenuCode}"
                                );
                            }

                            await context.SaveChangesAsync();
                        }

                        foreach (var menuJS in newProcesJs.Content)
                        {
                            var baseProcess = context
                                .MyProcesses.SelectMany(i => i.Content)
                                .FirstOrDefault(m => m.MenuCode == menuJS.MenuCode);

                            if (baseProcess == null)
                            {
                                TeleTools.ConssolWriting(
                                    $"❌ при попытки доодбавить КНОПКИ (все должно было быть уже добалено на этапе сохранения меню) в бд не было найдено такое мню - {menuJS.MenuCode}"
                                );
                            }

                            if (!baseProcess.Inputs.IsNullOrEmpty())
                            {
                                TeleTools.ConssolWriting(
                                    $"⚙️ В бд найдено инпутов: {baseProcess.Inputs.Count}. Они будут заменены на инпуты которые были в файле"
                                );

                                // foreach (var input in baseProcess.Inputs)
                                // {
                                //     baseProcess.Inputs.Remove(input);
                                // }
                                baseProcess.Inputs.RemoveRange(0, baseProcess.Inputs.Count);

                                await context.SaveChangesAsync();
                                TeleTools.ConssolWriting($"✅ инпуты были удалены");
                            }

                            List<System.Exception> exption = new List<Exception>();

                            foreach (var inputJS in menuJS.Inputs)
                            {
                                MyInput inputBase = null;

                                if (inputBase == null)
                                {
                                    inputBase = new MyInput()
                                    {
                                        CreatedAt = DateTime.Now,
                                        InputContent = inputJS.InputContent,
                                        InputDescription = inputJS.InputDescription,
                                        IsDelite = false,
                                        NextMenuCode = inputJS.NextMenuCode,
                                        InputName = inputJS.InputName,
                                        InputType = inputJS.InputType,
                                    };
                                }

                                try
                                {
                                    ProcessEntyBase
                                        .Content.FirstOrDefault(i => i.MenuCode == menuJS.MenuCode)
                                        .Inputs.Add(inputBase);

                                    await context.SaveChangesAsync();
                                    TeleTools.ConssolWriting(
                                        $"✅ Добавил - {inputJS.InputName}, в меню {menuJS.MenuCode}, NextMenuCode {inputJS.NextMenuCode}"
                                    );
                                }
                                catch (System.Exception ex)
                                {
                                    TeleTools.ConssolWriting(
                                        $"❌ не получилось загрузить  - {inputJS.InputName} в меню {menuJS.MenuCode}"
                                    );
                                    exption.Add(ex);
                                }

                                foreach (var item in exption)
                                {
                                    TeleTools.ConssolWriting(
                                        item.Message
                                            + "________________________________________\n\n\n"
                                    );
                                }
                            }
                        }
                    }

                    var proc = context.MyProcesses.ToList();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
        }

        public static void LoadDefoltsDB(BotConfig? configs)
        {
            using (var db = new MyDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        using (null)
                        {
                            //-------------------------------------------------------------InputTypes
                            var _newItemsList = configs?.BotInst?.InputTypes;
                            var _dbObjecctList = db.MyIntupTypes;

                            foreach (var item in _newItemsList)
                            {
                                var existInputTupe = db.MyIntupTypes.FirstOrDefault(i =>
                                    i.TypeCode == item.TypeCode
                                );
                                if (existInputTupe == null)
                                {
                                    _dbObjecctList.Add(item);
                                }
                                else
                                {
                                    UpdateProperties(existInputTupe, item);
                                }
                                db.SaveChanges();
                            }
                        }

                        using (null)
                        {
                            //-------------------------------------------------------------MyMenuTypes
                            var _newItemsList = configs?.BotInst?.MenuTypes;
                            var _dbObjecctList = db.MyMenuTypes;
                            foreach (var item in _newItemsList)
                            {
                                var existInputTupe = _dbObjecctList.FirstOrDefault(i =>
                                    i.TypeCode == item.TypeCode
                                );
                                if (existInputTupe == null)
                                {
                                    _dbObjecctList.Add(item);
                                }
                                else
                                {
                                    UpdateProperties(existInputTupe, item);
                                }
                                db.SaveChanges();
                            }
                        }

                        using (null)
                        {
                            //-------------------------------------------------------------UserTypes
                            var _newItemsList = configs?.BotInst?.UserTypes;
                            var _dbObjecctList = db.myUserTypes;
                            foreach (var item in _newItemsList)
                            {
                                var existInputTupe = _dbObjecctList.FirstOrDefault(i =>
                                    i.TypeCode == item.TypeCode
                                );
                                if (existInputTupe == null)
                                {
                                    _dbObjecctList.Add(item);
                                }
                                else
                                {
                                    UpdateProperties(existInputTupe, item);
                                }
                                db.SaveChanges();
                            }
                        }

                        using (null)
                        {
                            //-------------------------------------------------------------DefoltUsers
                            var _newItemsList = configs?.DefoltUsers;
                            var _dbObjecctList = db.MyDefoltUsers;
                            foreach (var item in _newItemsList)
                            {
                                var existInputTupe = _dbObjecctList.FirstOrDefault(i =>
                                    i.TelegramUserName == item.TelegramUserName
                                );
                                if (existInputTupe == null)
                                {
                                    existInputTupe = _dbObjecctList.FirstOrDefault(i =>
                                        i.TelegramId == item.TelegramId
                                    );
                                }

                                if (existInputTupe == null)
                                {
                                    _dbObjecctList.Add(item);
                                }
                                else
                                {
                                    UpdateProperties(existInputTupe, item);
                                }
                                db.SaveChanges();
                            }
                        }

                        using (null)
                        {
                            // //-------------------------------------------------------------Processes
                            // var _newItemsList = configs?.BotInst?.Processes;
                            // var _dbObjecctList = db.MyProcesses.ToList();
                            // foreach (var item in _newItemsList)
                            // {
                            //     var existInputTupe = _dbObjecctList.LastOrDefault(i =>
                            //         i.UserAccessCode == item.UserAccessCode
                            //     );
                            //     if (existInputTupe == null)
                            //     {
                            //         foreach (var menu in item.Content)
                            //         {
                            //             menu.Type = db.MyMenuTypes.FirstOrDefault(p =>
                            //                 p.TypeCode == menu.MenuTypeCode
                            //             );
                            //         }

                            //         db.MyProcesses.Add(item);
                            //     }
                            //     else
                            //     {
                            //         UpdateProperties(existInputTupe, item);
                            //     }
                            //     db.SaveChanges();
                            // }
                        }

                        using (null)
                        {
                            foreach (var proces in configs?.BotInst?.Processes)
                            {
                                var dbUpdateProcess = db.MyProcesses?.FirstOrDefault(p =>
                                    p.UserAccessCode == proces.UserAccessCode
                                );
                                if (dbUpdateProcess == null)
                                {
                                    db.MyProcesses.Add(proces);
                                    continue;
                                }

                                foreach (var menu in proces.Content)
                                {
                                    var dbUpdateMenu = dbUpdateProcess?.Content.FirstOrDefault(m =>
                                        m.MenuCode == menu.MenuCode
                                    );
                                    if (dbUpdateMenu == null)
                                        continue;

                                    if (dbUpdateMenu.Content != menu.Content)
                                        dbUpdateMenu.Content = menu.Content;
                                    dbUpdateMenu.Inputs.Clear();
                                    db.SaveChanges();
                                    dbUpdateMenu.Inputs = menu.Inputs;
                                }
                            }
                        }
                        db.SaveChanges();
                        var DDDDDProc = db.MyProcesses.ToList();

                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}
