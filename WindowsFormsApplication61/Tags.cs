using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication61
{
    class App
    {
        public Dictionary<string, string[]> dict = new Dictionary<string, string[]>()
        {

        };
        private object key;
        private object value;
    }
     class Buttons
    {
        public Dictionary<string, string[]> dict = new Dictionary<string, string[]>()
        {

        };
        private object key;
        private object value;
    }

    class Tag
    {
        

        public string music_app = "";
        public string[][] name = { new [] {"qwe"} }; 
        public string[] time = { "сколько время", "скажи время", "текущее время", "время" };
        public string[] youtube = { "открой youtube", "включи youtube" };
        public string[] vk = { "открой вк", "открой переписки", "открой сообщения" };
        public string[] music = { "включи музыку" };
        public string[] search = { "найди" };
        public string[] mute = { "выключи звук" };
        public string[] unmute = { "включи звук" };
        public string[] upsound = { "громкость на максимум", "громкость на полную", "звук на максимум", "звук на полную", "сделай громко", "громкий режим" };
        public string[] fiftysound = { "громкость на половину", "звук на половину", "громкость на середину", "звук на середину", "сделай наполовину", "средний режим" };
        public string[] downsound = { "громкость на минимум", "звук на минимум", "сделай тихо", "тихий режим" };
        public string[] play = { "играй", "воспроизведи" };
        public string[] pause = { "пауза", "не играй" };
        public string[] next = { "вперед", "следующая", "некст" };
        public string[] previsou = { "назад", "предыдущая", "вернись" };
        public string[] refresh_site = { "обнови", "обнови страницу" };
        public string[] next_site = { "открой будущую вкладку" };
        public string[] back_site = { "открой прошлую вкладку" };
        public string[] exit = { "отключись" };
        public string[] close = { "закрой", "закрой приложение" };
        public string[] cmd = { "открой диспетчер задач","включи диспетчер задач"};
        public string[] otkat = { "откат", "откати"};
    }

   
}
