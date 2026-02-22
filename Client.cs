using System.Diagnostics;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public class Client(HttpClient httpClient, AuthService authService, WbRequestService wbService, AllConfigs configs)
    {
        private string _switchToken = authService.Load();
        public async Task TrySwitchAsync(string pickpointName)
        {
            string switchJson = await wbService.Switch(configs.userConfig, configs.switchConfig, _switchToken, pickpointName);
            var switchResponse = ResultsConverter.ConvertSwitchResponse(switchJson);
            Debug.WriteLine($"Запрос switch на {pickpointName} выполнен.");
            string requestToken = switchResponse.Access.Token;
            configs.userConfig.Token = requestToken;
        }

        public async Task<List<DefectGood>> TryGetDefectGoodsAsync(string pickpointName)
        {
            List<DefectGood> defectGoods = new();
            try
            {
                string resultJson = await wbService.GetDefectGoods(configs.userConfig, configs.defectGoodsConfig, pickpointName);
                var resultDefectGoods = ResultsConverter.ConvertDefectGoods(resultJson);
                Debug.WriteLine($"Запрос defectGoods на {pickpointName} выполнен.");
                foreach (var item in resultDefectGoods.Goods)
                {
                    if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                    {
                        item.Pickpoint = pickpointName;
                        defectGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по бракованным товарам на пвз {pickpointName}.");
            }

            return defectGoods;
        }

        public async Task<List<ReplaceGood>> TryGetReplaceGoodsAsync(string pickpointName)
        {
            List<ReplaceGood> replaceGoods = new();
            try
            {
                string resultJson = await wbService.GetReplaceGoods(configs.userConfig, configs.replaceGoodsConfig, pickpointName);
                var resultReplaceGoods = ResultsConverter.ConvertReplaceGoods(resultJson);
                Debug.WriteLine($"Запрос replaceGoods на {pickpointName} выполнен.");
                foreach (var item in resultReplaceGoods.Goods)
                {
                    if (item.TicketId == 0)
                    {
                        item.Pickpoint = pickpointName;
                        replaceGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по товарам на замену на пвз {pickpointName}.");
            }
            return replaceGoods;
        }
        public async Task<List<HandingGood>> TryGetHandingGoodsAsync(string pickpointName)
        {
            List<HandingGood> handingGoods = new();
            try
            {
                string resultJson = await wbService.GetHandingGoods(configs.userConfig, configs.handingGoodsConfig, pickpointName);
                var resultHandingGoods = ResultsConverter.ConvertHandingGoods(resultJson);
                Debug.WriteLine($"Запрос handingGoods на {pickpointName} выполнен.");
                foreach (var item in resultHandingGoods.data.Goods)
                {
                    if (item.Type == "expired" && item.TicketId == 0)
                    {
                        item.Pickpoint = pickpointName;
                        handingGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по зависшим ШК на пвз {pickpointName}.");
            }
            return handingGoods;
        }

        public async Task<List<Ticket>> TryGetTicketsAsync(string pickpointName)
        {
            List<Ticket> tickets = new();
            try
            {
                string resultJson = await wbService.GetTickets(configs.userConfig, configs.ticketsConfig, pickpointName);
                var resultTickets = ResultsConverter.ConvertTickets(resultJson);
                Debug.WriteLine($"Запрос tickets на {pickpointName} выполнен.");
                foreach (var item in resultTickets.Tickets)
                {
                    if (item.State != 3 && item.TicketId != 0 && item.OnHoldDate != null)
                    {
                        item.Pickpoint = pickpointName;
                        tickets.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных о тикетах на пвз {pickpointName}.");
            }
            return tickets;
        }
    }
}