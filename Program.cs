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

            RequestConfig defectGoodsConfig = ConfigLoader.LoadDefectGoods();
            RequestConfig ticketsConfig = ConfigLoader.LoadTickets();
            RequestConfig handingGoodsConfig = ConfigLoader.LoadHandingGoods();
            RequestConfig replaceGoodsConfig = ConfigLoader.LoadReplaceGoods();
            UserConfig userConfig = ConfigLoader.LoadUserConfig();

            WbRequestService service = new(httpClient);

            try
            {
                string jsonData = await service.GetDataForPickpoint(userConfig, defectGoodsConfig, "p105");
                Console.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе {ex.Message}");
            }
        }
    }
}
