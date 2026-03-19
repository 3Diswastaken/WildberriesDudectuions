using System.Diagnostics;
using System.Text;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public class Client(AuthService authService, WbRequestService wbService, AllConfigs configs)
    {
        public async Task TrySwitchAsync(Pickpoint pickpoint)
        {
            string _tokenForSwitch = authService.Load();

            var result = await wbService.Switch(configs.switchConfig, _tokenForSwitch, pickpoint);
            pickpoint.Token = result.Access.Token;
            Debug.WriteLine($"Запрос switch на {pickpoint.Name} выполнен.");

        }

        public async Task<List<Pickpoint>> TryGetPickpointsListAsync()
        {
            string _token = authService.Load();

            var result = await wbService.Pickpoints(configs.pickpointsListConfig, _token);
            Debug.WriteLine($"Запрос pickpoints выполенен");

            return result.Pickpoints;
        }

        public async Task<List<DefectGood>> TryGetDefectGoodsAsync(Pickpoint pickpoint)
        {
            List<DefectGood> defectGoods = new();
            try
            {
                var result = await wbService.GetGoods<DefectGoodsRoot>(configs.defectGoodsConfig, pickpoint);
                Debug.WriteLine($"Запрос defectGoods на {pickpoint.Name} выполнен.");
                foreach (var item in result.Goods)
                {
                    if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                    {
                        item.Pickpoint = pickpoint.Name;
                        defectGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по бракованным товарам на пвз {pickpoint.Name}.");
            }

            return defectGoods;
        }


        public async Task<List<ReplaceGood>> TryGetReplaceGoodsAsync(Pickpoint pickpoint)
        {
            List<ReplaceGood> replaceGoods = new();
            try
            {
                var result = await wbService.GetGoods<ReplaceGoodsRoot>(configs.replaceGoodsConfig, pickpoint);
                Debug.WriteLine($"Запрос replaceGoods на {pickpoint.Name} выполнен.");
                foreach (var item in result.Goods)
                {
                    if (item.TicketId == 0)
                    {
                        item.Pickpoint = pickpoint.Name;
                        replaceGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по товарам на замену на пвз {pickpoint.Name}.");
            }
            return replaceGoods;
        }
        public async Task<List<HandingGood>> TryGetHandingGoodsAsync(Pickpoint pickpoint)
        {
            List<HandingGood> handingGoods = new();
            try
            {
                var result = await wbService.GetGoods<HandingGoodsRoot>(configs.handingGoodsConfig, pickpoint);
                Debug.WriteLine($"Запрос handingGoods на {pickpoint.Name} выполнен.");
                foreach (var item in result.data.Goods)
                {
                    if (item.Type == "expired" && item.TicketId == 0)
                    {
                        item.Pickpoint = pickpoint.Name;
                        handingGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по зависшим ШК на пвз {pickpoint.Name}.");
            }
            return handingGoods;
        }

        /*public async Task<List<Ticket>> TryGetTicketsAsync(Pickpoint pickpoint)
        {
            List<Ticket> tickets = new();
            try
            {
                var result = await wbService.GetGoods<WbTicketsResponse>(configs.ticketsConfig, pickpoint);
                Debug.WriteLine($"Запрос tickets на {pickpoint.Name} выполнен.");
                foreach (var item in result.Data)
                {
                    if (item.State != 3 && item.TicketId != 0 && item.OnHoldDate != null)
                    {
                        item.Pickpoint = pickpoint.Name;
                        tickets.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных о тикетах на пвз {pickpoint.Name}.");
            }
            return tickets;
        }*/


        public async Task<WbGoodSearchEntry> TryFindByShkAsync(string shk, Pickpoint pickpoint)
        {
            List<WbGoodSearchEntry> Good = new();
            byte[] textBytes = Encoding.UTF8.GetBytes(shk);
            string shk64base = Convert.ToBase64String(textBytes);
            try
            {
                var result = await wbService.FindByShk(configs.findByShkConfig, pickpoint, shk);
                Debug.WriteLine($"Запрос поиска шк на {pickpoint.Name} выполнен.");
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        Good.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при поиске по ШК на пвз {pickpoint.Name}.");
            }
            return Good.FirstOrDefault();
        }
    }
}