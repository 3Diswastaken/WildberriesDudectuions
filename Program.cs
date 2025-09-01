using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildberriesDudectuions.Models;

namespace WildberriesDudectuions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            // конфиг на каждый тип удержаний со своими url и headers
            RequestConfig defectGoodsConfig = ConfigLoader.LoadDefectGoods();
            RequestConfig ticketsConfig = ConfigLoader.LoadTickets(); // использует post и payload
            RequestConfig handingGoodsConfig = ConfigLoader.LoadHandingGoods();
            RequestConfig replaceGoodsConfig = ConfigLoader.LoadReplaceGoods();
            UserConfig userConfig = ConfigLoader.LoadUserConfig(); // хранит токен и список пвз с их id

            WbRequestService service = new(httpClient);

            try
            {
                string jsonData = await service.GetDataForPickpoint(userConfig, ticketsConfig, "p105");
                Console.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе {ex.Message}");
            }
        }
    }
}
