using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
            RequestConfig replaceGoodsConfig = ConfigLoader.LoadReplaceGoods();
            UserConfig userConfig = ConfigLoader.LoadUserConfig(); // хранит токен и список пвз с их id

            WbRequestService service = new(httpClient);

            List<GoodsInterface> allGoods = new();

            foreach (var pickpoint in userConfig.PickpointId)
            {
                try
                {
                    string jsonData = await service.GetDataForPickpoint(userConfig, replaceGoodsConfig, pickpoint.Key);
                    var resultReplaceGoods = ResultsConverter.ConvertReplaceGoods(jsonData);
                    foreach (var item in resultReplaceGoods.ReplaceGoodsList)
                    {
                        if (item.TicketId == 0)
                        {
                            item.Pickpoint = pickpoint.Key;
                            allGoods.Add(item);
                        }
                    }

                    jsonData = await service.GetDataForPickpoint(userConfig, defectGoodsConfig, pickpoint.Key);
                    var resultDefectGoods = ResultsConverter.ConvertDefectGoods(jsonData);
                    foreach (var item in resultDefectGoods.DefectGoodsList)
                    {
                        if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                        {
                            item.Pickpoint = pickpoint.Key;
                            allGoods.Add(item);
                        }
                    }

                    jsonData = await service.GetDataForPickpoint(userConfig, ticketsConfig, pickpoint.Key);
                    var reslutTickets = ResultsConverter.ConvertTickets(jsonData);
                    foreach (var item in reslutTickets.Tickets)
                    {
                        if (item.OnHoldDate != null)
                        {
                            item.Pickpoint = pickpoint.Key;
                            allGoods.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при запросе {ex.Message}");
                }
            }

            var sortedGoods = allGoods.OrderBy(g => g.OnHoldDate);
            foreach (var item in sortedGoods)
            {
                Console.WriteLine($"{item.OnHoldDate?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)} {item.Reason} {item.Pickpoint}");
            }



            Console.ReadLine();
        }
    }
}
