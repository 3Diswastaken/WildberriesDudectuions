using System.Diagnostics;
using System.Text;
using WildberriesDeductions.Wildberries.Models;

namespace WildberriesDeductions.Wildberries
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

        public async Task<List<DeductionGood>> TryGetDefectGoodsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();
            try
            {
                var rawData = await wbService.GetGoods<DefectGoodsRoot>(configs.defectGoodsConfig, pickpoint);
                Debug.WriteLine($"Запрос defectGoods на {pickpoint.Name} выполнен.");
                if (rawData.Goods == null)
                    return result;
                foreach (var item in rawData.Goods)
                {
                    if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                    {
                        item.Pickpoint = pickpoint.Name;
                        result.Add(item.ToDomain());
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по бракованным товарам на пвз {pickpoint.Name}.");
            }

            return result;
        }


        public async Task<List<DeductionGood>> TryGetReplaceGoodsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();
            try
            {
                var rawData = await wbService.GetGoods<ReplaceGoodsRoot>(configs.replaceGoodsConfig, pickpoint);
                Debug.WriteLine($"Запрос result на {pickpoint.Name} выполнен.");
                if (rawData.Goods == null) return result;
                foreach (var item in rawData.Goods)
                {
                    if (item.TicketId == 0)
                    {
                        item.Pickpoint = pickpoint.Name;
                        result.Add(item.ToDomain());
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по товарам на замену на пвз {pickpoint.Name}.");
            }
            return result;
        }
        public async Task<List<DeductionGood>> TryGetHandingGoodsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();
            try
            {
                var rawData = await wbService.GetGoods<HandingGoodsRoot>(configs.handingGoodsConfig, pickpoint);
                Debug.WriteLine($"Запрос result на {pickpoint.Name} выполнен.");
                if (rawData.data.Goods == null) return result;
                foreach (var item in rawData.data.Goods)
                {
                    if (item.Type == "expired" && item.TicketId == 0)
                    {
                        item.Pickpoint = pickpoint.Name;
                        result.Add(item.ToDomain());
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по зависшим ШК на пвз {pickpoint.Name}.");
            }
            return result;
        }

        public async Task<List<DeductionGood>> TryGetTicketsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();
            try
            {
                var rawData = await wbService.GetGoods<WbTicketsResponse>(configs.ticketsConfig, pickpoint);
                Debug.WriteLine($"Запрос result на {pickpoint.Name} выполнен.");


                foreach (var item in rawData.Data)
                {
                    var mappedItem = item.ToDomain();

                    if (item.State != 3 && item.TicketId != 0 && mappedItem.OnHoldDate != null)
                    {
                        item.Pickpoint = pickpoint.Name;
                        result.Add(item.ToDomain());
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных о тикетах на пвз {pickpoint.Name}.");
            }
            return result;
        }


        public async Task<WbGoodSearchEntry> TryFindByShkAsync(string shk, Pickpoint pickpoint)
        {
            List<WbGoodSearchEntry> Good = new();
            byte[] textBytes = Encoding.UTF8.GetBytes(shk);
            string shk64base = Convert.ToBase64String(textBytes);
            try
            {
                var result = await wbService.FindByShk(configs.findByShkConfig, pickpoint, shk64base);
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