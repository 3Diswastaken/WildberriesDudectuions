using Spectre.Console;
using WildberriesDeductions.Wildberries;
using WildberriesDeductions.Wildberries.Exceptions;
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

            List<string> errorLog = new();

            if (!authService.IsTokenValid())
            {
                long phone = SpectreConsole.AskForPhone();
                await authService.TryLoginAsync(phone);

                while (true)
                {
                    int code = SpectreConsole.AskForCode();
                    try
                    {
                        await authService.TryValidateAsync(code);
                        break;
                    }
                    catch (ApiException ex)
                    {
                        AnsiConsole.MarkupLine($"[yellow]Ошибка WB:[/] {ex.Message}. Попробуйте еще раз.");
                        if (ex.Message.Contains("5 минут"))
                        {
                            AnsiConsole.MarkupLine("[grey]Приложение будет закрыто. Попробуйте зайти позже.[/]");
                            return;
                        }
                    }
                    catch (NetworkException) 
                    {
                        AnsiConsole.MarkupLine("[red]Проблема с соединением.[/] Проверьте сеть и нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Непредвиденная ошибка:[/] {ex.Message}");
                        throw; 
                    }
                }
            }

            var allPickpoints = new List<Pickpoint>();

            try
            {
                allPickpoints = await client.TryGetPickpointsListAsync();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при получении списка пунктов выдачи! Описание ошибки: {ex.Message}[/]");
                return;
            }

            // заранее собираем токены (живут 24 часа) для всех пунктов выдачи, чтобы не делать это при каждом запросе данных
            foreach (var pickpoint in allPickpoints)
            {
                if (allPickpoints == null)
                    break;

                try
                {
                    await client.TrySwitchAsync(pickpoint);
                }
                catch (Exception ex)
                {
                    errorLog.Add($"Ошибка при переключении на ПВЗ {pickpoint.Name}! Описание ошибки: {ex.Message}");
                }
                ;
            }

            while (true)
            {
                var selectedPickpoints = SpectreConsole.SelectPickpoints(allPickpoints);

                List<DeductionGood> deductionGoods = new();
                await AnsiConsole.Status().StartAsync("Получение данных...", async ctx =>
                {
                    foreach (var pickpoint in selectedPickpoints)
                    {
                        try
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
                        catch (Exception ex)
                        {
                            errorLog.Add($"Ошибка получения данных для ПВЗ {pickpoint.Name}! Описание ошибки: {ex.Message}");
                        }
                    }
                });


                while (true)
                {
                    var deduction = SpectreConsole.SelectDeduction(deductionGoods, errorLog);
                    if (deduction == null) // проверка на назад
                        break;
                    else if (deduction.Details == null) // если детали уже были загружены, то не грузим их снова
                    {
                        var deductionDetails = await AnsiConsole.Status()
                            .Spinner(Spinner.Known.Dots)
                            .StartAsync("Загрузка...", async ctx =>
                            {
                                var deductionPickpoint = selectedPickpoints.FirstOrDefault(p => p.Name == deduction.PickpointName);

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