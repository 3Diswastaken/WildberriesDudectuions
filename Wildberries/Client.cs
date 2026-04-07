using System.Diagnostics;
using System.Text;
using WildberriesDeductions.Wildberries.Models;

namespace WildberriesDeductions.Wildberries
{
    public class Client(AuthService authService, WbRequestService wbService, AllConfigs configs)
    {
        public async Task SwitchAsync(Pickpoint pickpoint)
        {
            string _tokenForSwitch = authService.Load();

            var result = await wbService.Switch(configs.switchConfig, _tokenForSwitch, pickpoint);
            pickpoint.Token = result.Access.Token;
            Debug.WriteLine($"Запрос switch на {pickpoint.Name} выполнен.");
        }

        public async Task<List<Pickpoint>> GetPickpointsListAsync()
        {
            string _token = authService.Load();

            var result = await wbService.Pickpoints(configs.pickpointsListConfig, _token);
            Debug.WriteLine($"Запрос pickpoints выполенен");

            return result.Pickpoints;
        }

        public async Task<List<DeductionGood>> GetDefectGoodsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();

            var rawData = await wbService.GetGoods<DefectGoodsRoot>(configs.defectGoodsConfig, pickpoint);
            Debug.WriteLine($"Запрос браков на {pickpoint.Name} выполнен.");
            if (rawData.Goods == null)
                return result;
            foreach (var item in rawData.Goods)
            {
                if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                    result.Add(item.ToDomain(pickpoint.Name));
            }

            return result;
        }


        public async Task<List<DeductionGood>> GetReplaceGoodsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();

            var rawData = await wbService.GetGoods<ReplaceGoodsRoot>(configs.replaceGoodsConfig, pickpoint);
            Debug.WriteLine($"Запрос подмен на {pickpoint.Name} выполнен.");
            if (rawData.Goods == null) return result;
            foreach (var item in rawData.Goods)
            {
                if (item.TicketId == 0)
                    result.Add(item.ToDomain(pickpoint.Name));
            }

            return result;
        }
        public async Task<List<DeductionGood>> GetHandingGoodsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();

            var rawData = await wbService.GetGoods<HandingGoodsRoot>(configs.handingGoodsConfig, pickpoint);
            Debug.WriteLine($"Запрос зависших шк на {pickpoint.Name} выполнен.");
            if (rawData.data.Goods == null) return result;
            foreach (var item in rawData.data.Goods)
            {
                if (item.Type == "expired" && item.TicketId == 0)
                    result.Add(item.ToDomain(pickpoint.Name));
            }

            return result;
        }

        public async Task<List<DeductionGood>> GetTicketsAsync(Pickpoint pickpoint)
        {
            List<DeductionGood> result = new();

            var rawData = await wbService.GetGoods<WbTicketsResponse>(configs.ticketsConfig, pickpoint);
            Debug.WriteLine($"Запрос тикетов на {pickpoint.Name} выполнен.");

            foreach (var item in rawData.Data)
            {
                var mappedItem = item.ToDomain(pickpoint.Name);

                if (item.State != 3 && item.TicketId != 0 && mappedItem.OnHoldDate != null)
                    result.Add(mappedItem);
            }

            return result;
        }


        public async Task<WbGoodSearchEntry> FindByShkAsync(string shk, Pickpoint pickpoint)
        {
            var result = await wbService.FindByShk(configs.findByShkConfig, pickpoint, shk);
            var details = result?.Data?.FirstOrDefault();
            Debug.WriteLine($"Запрос поиска шк на {pickpoint.Name} выполнен.");

            return details;
        }
    }
}