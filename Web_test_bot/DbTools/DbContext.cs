using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Web_test_bot.DbTools.DbObjeckts;

namespace Web_test_bot.DbTools
{
    public class MyDbContext : DbContext
    {
        public DbSet<MyChat> MyChats { get; set; }
        public DbSet<MyUpdate> MyUpdates { get; set; }
        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<MyUserType> myUserTypes { get; set; }
        public DbSet<MyDefoltUser> MyDefoltUsers { get; set; }
        public DbSet<MyInputType> MyIntupTypes { get; set; }
        public DbSet<MyMenuType> MyMenuTypes { get; set; }
        public DbSet<MyProcess> MyProcesses { get; set; }
        public DbSet<MyMenuContent> MyMenus { get; set; }

        public DbSet<MyWorkEnty> MyEnteties { get; set; }

        //  public DbSet<MyInput> MyIntup { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies() // Включите ленивую загрузку
                .UseSqlite("Data Source=MyDatabase2.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи между MyUser и MyUserType
            modelBuilder
                .Entity<MyUser>()
                .HasOne(u => u.UserType) // Указывает, что MyUser имеет один MyUserType
                .WithMany(ut => ut.UserList) // Указывает, что MyUserType имеет много MyUser
                .HasForeignKey(u => u.usertypeId); // Указывает внешний ключ

            modelBuilder
                .Entity<MyUser>()
                .HasMany(u => u.UserGroupList) // Указывает, что MyUser имеет один MyUserType
                .WithMany(ch => ch.ChatUsers); // Указывает, что MyUserType имеет много MyUser
            modelBuilder
                .Entity<MyUserType>()
                .HasMany(ut => ut.DefoltUsers)
                .WithOne(du => du.UserType)
                .HasForeignKey(du => du.UserTypeСode) // Используем строку в качестве внешнего ключа
                .HasPrincipalKey(ut => ut.TypeCode); // Указываем, что TypeCode является основным ключом
            modelBuilder
                .Entity<MyChat>()
                .HasMany(ch => ch.ChatUpdates)
                .WithOne(u => u.ParentChat)
                .HasForeignKey(u => u.ChatId) // Убедитесь, что это ChatId
                .HasPrincipalKey(ch => ch.TeleChatId); // Убедитесь, что это TeleChatId
            modelBuilder
                .Entity<MyChat>()
                .HasMany(ch => ch.ChatMessages)
                .WithOne(m => m.ParentChat)
                .HasForeignKey(m => m.TeleChatId) // Убедитесь, что это ChatId
                .HasPrincipalKey(ch => ch.TeleChatId); // Убедитесь, что это TeleChatId

            modelBuilder
                .Entity<MyUserType>()
                .HasOne(ut => ut.Process)
                .WithOne(p => p.UserAccess)
                .HasForeignKey<MyProcess>(p => p.UserAccessCode) // Используем строку в качестве внешнего ключа
                .HasPrincipalKey<MyUserType>(ut => ut.TypeCode); // Указываем, что TypeCode является основным ключом

            modelBuilder
                .Entity<MyMenuContent>()
                .HasOne(ut => ut.Type)
                .WithMany(p => p.Menus)
                .HasForeignKey(i => i.TypeId);

            modelBuilder
                .Entity<MyMenuContent>()
                .HasOne(u => u.Process)
                .WithMany(ut => ut.Content)
                .HasForeignKey(u => u.ProcessID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<MyInput>()
                .HasOne(u => u.Menu)
                .WithMany(ut => ut.Inputs)
                .HasForeignKey(u => u.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
            ;

            modelBuilder
                .Entity<MyInput>()
                .HasOne(ut => ut.NextMenu)
                .WithMany(p => p.CallingInputs)
                .HasForeignKey(p => p.NextMenuCode) // Используем строку в качестве внешнего ключа
                .HasPrincipalKey(ut => ut.MenuCode)
                .OnDelete(DeleteBehavior.Cascade); // Указываем, что TypeCode является основным ключом

            modelBuilder
                .Entity<MyChat>()
                .HasOne(ut => ut.CurentMenu)
                .WithMany(p => p.ChatsInThisMenu)
                .HasForeignKey(p => p.CurentMenuId)
                .OnDelete(DeleteBehavior.Cascade); // Используем строку в качестве внешнего ключа
            // Указываем, что TypeCode является основным ключом

            modelBuilder
                .Entity<MyChat>()
                .HasOne(ut => ut.PriviousIncummingMessage)
                .WithOne(p => p.PriviousIncummingMessageChat)
                .HasForeignKey<MyChat>(p => p.PriviousIncummingMessageId);

            modelBuilder
                .Entity<MyWorkEnty>()
                .HasMany(ch => ch.Items)
                .WithOne(u => u.ParentEnt)
                .HasForeignKey(u => u.ParenId) // Убедитесь, что это ChatId
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<MyChat>()
                .HasMany(ch => ch.WorkItems)
                .WithOne(u => u.ParentChat)
                .HasForeignKey(u => u.ParentChatId) // Убедитесь, что это ChatId
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<MyWorkDetales>()
                .HasMany(ch => ch.TelegramPfotoes)
                .WithOne(u => u.ParentDetale)
                .HasForeignKey(u => u.MyWorkDetalesId) // Убедитесь, что это ChatId
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .Entity<MyMessage>()
                .HasMany(ch => ch.photo)
                .WithOne(u => u.ParentMessage)
                .HasForeignKey(u => u.MessageId) // Убедитесь, что это ChatId
                .OnDelete(DeleteBehavior.Cascade);

     
        }
    }
}
