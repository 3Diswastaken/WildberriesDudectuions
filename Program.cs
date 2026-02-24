using Spectre.Console;
using System.Diagnostics;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configs = Factory.LoadConfigs();

            HttpClient httpClient = new();

            WbRequestService wbService = new(httpClient);

            TokenStorage storage = new();

            AuthService authService = new(storage, wbService, configs.loginConfig, configs.validateConfig);

            Client client = new(authService, wbService, configs);

            List<GoodsInterface> allGoods = new();

            if (!authService.IsTokenValid())
            {
                long phone = SpectreConsole.AskForPhone();
                await authService.TryLoginAsync(phone);

                int code = SpectreConsole.AskForCode();
                await authService.TryValidateAsync(code);
            }

            List<string> pickpoints = new() { "p105", "p10" };

            await AnsiConsole.Status().StartAsync("Получение данных...", async ctx =>
                {
                    foreach (var pickpoint in pickpoints)
                    {
                        ctx.Status($"Переключение на {pickpoint}...");
                        await client.TrySwitchAsync(pickpoint);

                        ctx.Status($"Получение зависших шк для {pickpoint}...");
                        var handingGoods = await client.TryGetHandingGoodsAsync(pickpoint);
                        allGoods.AddRange(handingGoods);

                        foreach (var good in handingGoods)
                        {
                            Debug.WriteLine($"Зависший шк: {good.Shk}, дата: {good.OnHoldDate}, пвз: {pickpoint}");
                        }

                        ctx.Status($"Получение брака для {pickpoint}...");
                        var defectGoods = await client.TryGetDefectGoodsAsync(pickpoint);
                        allGoods.AddRange(defectGoods);

                        ctx.Status($"Получение подмен для {pickpoint}...");
                        var replaceGoods = await client.TryGetReplaceGoodsAsync(pickpoint);
                        allGoods.AddRange(replaceGoods);

                        ctx.Status($"Получение тикетов для {pickpoint}...");
                        var tickets = await client.TryGetTicketsAsync(pickpoint);
                        allGoods.AddRange(tickets);
                    }
                });
            SpectreConsole.PrintDeductionGoods(allGoods);

            Console.ReadLine();
        }
    }
}
