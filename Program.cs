using Spectre.Console;
using WildberriesDeductions.Wildberries;
using WildberriesDeductions.Wildberries.Models;

namespace WildberriesDeductions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configs = Factory.LoadConfigs(); // конфиги для всех запросов (url, payload, method, headers), хранятся в json файлах
            HttpClient httpClient = new();
            WbRequestService wbService = new(httpClient);
            TokenStorage storage = new(); // для хранения токена в файле, чтобы не логиниться при каждом запуске
            AuthService authService = new(storage, wbService, configs.loginConfig, configs.validateConfig);
            Client client = new(authService, wbService, configs);


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

            var allPickpoints = await client.TryGetPickpointsListAsync();

            // заранее собираем токены (живут 24 часа) для всех пунктов выдачи, чтобы не делать это при каждом запросе данных
            foreach (var pickpoint in allPickpoints)
                await client.TrySwitchAsync(pickpoint);

            while (true)
            {
                var selectedPickpoints = SpectreConsole.SelectPickpoints(allPickpoints);

                List<DeductionGood> deductionGoods = new();
                await AnsiConsole.Status().StartAsync("Получение данных...", async ctx =>
                {
                    foreach (var pickpoint in selectedPickpoints)
                    {
                        ctx.Status($"Получение зависших шк для {pickpoint.Name}...");
                        deductionGoods.AddRange(await client.TryGetHandingGoodsAsync(pickpoint));

                        ctx.Status($"Получение брака для {pickpoint.Name}...");
                        deductionGoods.AddRange(await client.TryGetDefectGoodsAsync(pickpoint));

                        ctx.Status($"Получение подмен для {pickpoint.Name}...");
                        deductionGoods.AddRange(await client.TryGetReplaceGoodsAsync(pickpoint));

                        ctx.Status($"Получение тикетов для {pickpoint.Name}...");
                        deductionGoods.AddRange(await client.TryGetTicketsAsync(pickpoint));
                    }
                });


                while (true)
                {
                    var deduction = SpectreConsole.SelectDeduction(deductionGoods);
                    if (deduction == null) // проверка на назад
                        break;
                    else if (deduction.Details == null) // если детали уже были загружены, то не грузим их снова
                    {
                        var deductionDetails = await AnsiConsole.Status()
                            .Spinner(Spinner.Known.Dots)
                            .StartAsync("Загрузка...", async ctx =>
                            {
                                var deductionPickpoint = selectedPickpoints.FirstOrDefault(p => p.Name == deduction.Pickpoint);

                                ctx.Status($"Получение данных по ШК {deduction.Shk}...");
                                var details = await client.TryFindByShkAsync(deduction.Shk.ToString(), deductionPickpoint);

                                return deduction.ToEnrich(details); // дополняем удержание данными по ШК
                            });
                    }
                    SpectreConsole.ShowDeductionScreen(deduction);
                }
            }
        }
    }
}