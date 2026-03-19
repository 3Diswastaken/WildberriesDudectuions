using Spectre.Console;
using System.Diagnostics;
using System.Text;
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

            //List<GoodsInterface> allGoods = new();

            

            if (!authService.IsTokenValid())
            {
                long phone = SpectreConsole.AskForPhone();
                await authService.TryLoginAsync(phone);

                while (true)
                {
                    int code = SpectreConsole.AskForCode();
                    if (await authService.TryValidateAsync(code))
                        break;
                    else
                        AnsiConsole.MarkupLine("[red]Неверный код. Попробуйте снова.[/]");
                }
            }

            while (true)
            {
                var allPickpoints = await client.TryGetPickpointsListAsync();
                var selectedPickpoints = SpectreConsole.SelectPickpoints(allPickpoints);

                List<DeductionGood> deductionGoods = new();
                await AnsiConsole.Status().StartAsync("Получение данных...", async ctx =>
                {
                    foreach (var pickpoint in selectedPickpoints)
                    {
                        ctx.Status($"Переключение на {pickpoint.Name}...");
                        await client.TrySwitchAsync(pickpoint);

                        ctx.Status($"Получение зависших шк для {pickpoint.Name}...");
                        var handingGoods = await client.TryGetHandingGoodsAsync(pickpoint);
                        deductionGoods.AddRange(handingGoods.Select(g => g.ToDomain()));

                        ctx.Status($"Получение брака для {pickpoint.Name}...");
                        var defectGoods = await client.TryGetDefectGoodsAsync(pickpoint);
                        deductionGoods.AddRange(defectGoods.Select(g => g.ToDomain()));

                        ctx.Status($"Получение подмен для {pickpoint.Name}...");
                        var replaceGoods = await client.TryGetReplaceGoodsAsync(pickpoint);
                        deductionGoods.AddRange(replaceGoods.Select(g => g.ToDomain()));

                        /* ctx.Status($"Получение тикетов для {pickpoint.Name}...");
                         var tickets = await client.TryGetTicketsAsync(pickpoint);
                         allGoods.AddRange(tickets);*/
                    }
                });
                //SpectreConsole.PrintDeductionGoods(allGoods);

                while (true)
                {
                    var deduction = SpectreConsole.SelectDeduction(deductionGoods);
                    if (deduction == null)
                    {
                        break;
                    } else
                    {
                        var deductionPickpoint = selectedPickpoints.FirstOrDefault(p => p.Name == deduction.Pickpoint);

                        await client.TrySwitchAsync(deductionPickpoint);
                        var rawDeductionDetails = await client.TryFindByShkAsync(deduction.Shk.ToString(), deductionPickpoint);
                        var deductionDetails = rawDeductionDetails.ToEnrich(deduction);


                        Debug.WriteLine($"Размер: {deductionDetails.Details.Size}");
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
