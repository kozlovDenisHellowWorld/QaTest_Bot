using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Web_test_bot.BotTools
{
    public class MainСonfiguration
    {
        public string BotToken { set; get; } = null;
        public string BotId { set; get; } = null;

        public string FigmaToken { set; get; } = "";

        public IConfigurationRoot? СonfigurationRoot { set; get; }

        public MainСonfiguration(IConfigurationRoot? configuration)
        {
            if (configuration is not null)
            {
                СonfigurationRoot = configuration;
                BotToken = configuration["Bot:Token"];
                FigmaToken = configuration["FigmaApps:Token"];
            }
        }

        public void SetId(long BotId)
        {
            СonfigurationRoot["Bot:BotId"] = BotId.ToString();
        }

        private void SaveConfiguration()
        {
            // Получаем путь к файлу конфигурации
            var filePath = "botInst.json"; // Убедитесь, что путь к файлу правильный

            // Загружаем текущий JSON
            var json = File.ReadAllText(filePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            // Обновляем значение
            jsonObj.Bot.Id = BotId;

            // Сохраняем обратно в файл
            File.WriteAllText(
                filePath,
                Newtonsoft.Json.JsonConvert.SerializeObject(
                    jsonObj,
                    Newtonsoft.Json.Formatting.Indented
                )
            );
        }
    }
}
