using System.Diagnostics;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public class Client(AuthService authService, WbRequestService wbService, AllConfigs configs)
    {
        public async Task TrySwitchAsync(string pickpointName)
        {
            string _tokenForSwitch = authService.Load();

            var result = await wbService.Switch(configs.userConfig, configs.switchConfig, _tokenForSwitch, pickpointName);
            Debug.WriteLine($"Запрос switch на {pickpointName} выполнен.");
            string requestToken = result.Access.Token;
            configs.userConfig.Token = requestToken;
        }

        public async Task<List<DefectGood>> TryGetDefectGoodsAsync(string pickpointName)
        {
            List<DefectGood> defectGoods = new();
            try
            {
                var result = await wbService.GetGoods<DefectGoodsRoot>(configs.userConfig, configs.defectGoodsConfig, pickpointName);
                Debug.WriteLine($"Запрос defectGoods на {pickpointName} выполнен.");
                foreach (var item in result.Goods)
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
                var result = await wbService.GetGoods<ReplaceGoodsRoot>(configs.userConfig, configs.replaceGoodsConfig, pickpointName);
                Debug.WriteLine($"Запрос replaceGoods на {pickpointName} выполнен.");
                foreach (var item in result.Goods)
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
                var result = await wbService.GetGoods<HandingGoodsRoot>(configs.userConfig, configs.handingGoodsConfig, pickpointName);
                Debug.WriteLine($"Запрос handingGoods на {pickpointName} выполнен.");
                foreach (var item in result.data.Goods)
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
                var result = await wbService.GetGoods<TicketsRoot>(configs.userConfig, configs.ticketsConfig, pickpointName);
                Debug.WriteLine($"Запрос tickets на {pickpointName} выполнен.");
                foreach (var item in result.Tickets)
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