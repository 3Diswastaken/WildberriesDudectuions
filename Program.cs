using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using WildberriesDeductions.Wildberries;
using WildberriesDeductions.Wildberries.Exceptions;
using WildberriesDeductions.Wildberries.Models;
using WildberriesDeductions.Wildberries.Models.Dto;

namespace WildberriesDeductions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string baseDir = AppContext.BaseDirectory;

            // Создаем лоадеры
            var wbLoader = new ConfigLoader(Path.Combine(baseDir, "Wildberries", "Configs"));
            var ipEyeLoader = new ConfigLoader(Path.Combine(baseDir, "IPEye", "Configs"));

            // Получаем полные наборы конфигов
            Dictionary<string, RequestConfig> wbApi = wbLoader.LoadAll();
            Dictionary<string, RequestConfig> ipEyeApi = ipEyeLoader.LoadAll();

            HttpClient httpClient = new();
            WbRequestService wbService = new(httpClient);
            TokenStorage storage = new(); // для хранения токена в файле, чтобы не логиниться при каждом запуске
            AuthService authService = new(storage, wbService, wbApi["login"], wbApi["validate"], wbApi["switch"]);
            Client client = new(authService, wbService, wbApi);

            List<string> errorLog = new();

            bool isAuthorized = authService.IsTokenValid();

            if (!isAuthorized)
            {
                string step = "phone";
                long phone = 0;
                string tokenForValidate = string.Empty;


                while (!isAuthorized)
                {
                    try
                    {
                        switch (step)
                        {
                            case "phone":
                                phone = SpectreConsole.AskForPhone();
                                tokenForValidate = await authService.LoginAsync(phone);
                                step = "code";
                                break;

                            case "code":
                                string code = SpectreConsole.AskForCode();
                                await authService.ValidateCodeAndSaveTokenAsync(code, tokenForValidate);
                                isAuthorized = true;
                                break;
                        }
                    }

                    catch (ApiException ex)
                    {
                        AnsiConsole.MarkupLine($"[yellow]Ошибка WB:[/] {ex.Message}. Попробуйте еще раз.");
                        if (ex.Message.Contains("5 минут"))
                        {
                            AnsiConsole.MarkupLine("[grey]Приложение будет закрыто. Попробуйте зайти позже.[/]");
                            Console.ReadKey();
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
                        Console.ReadKey();
                        throw;
                    }
                }
            }

            var allPickpoints = new List<Pickpoint>();

            try
            {
                allPickpoints = await client.GetPickpointsListAsync();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при получении списка пунктов выдачи! Описание ошибки: {ex.Message}[/]");
            }

            if (!allPickpoints.Any())
            {
                AnsiConsole.MarkupLine("[grey]Приложение будет закрыто, так как без списка пунктов выдачи дальнейшая работа невозможна.[/]");
                Console.ReadKey();
                return;
            }

            // заранее собираем токены (живут 24 часа) для всех пунктов выдачи, чтобы не делать это при каждом запросе данных
            foreach (var pickpoint in allPickpoints)
            {
                try
                {
                    pickpoint.Token = await authService.UpdatePickpointToken(pickpoint);
                }
                catch (Exception ex)
                {
                    errorLog.Add($"Ошибка при создании токена для ПВЗ {pickpoint.Name}! Описание ошибки: {ex.Message}");
                };
            }

            while (true)
            {
                var selectedPickpoints = SpectreConsole.SelectPickpoints(allPickpoints);

                List<DeductionGood> deductions = new();
                List<Ticket> tickets = new();
                await AnsiConsole.Status().StartAsync("Получение данных...", async ctx =>
                {
                    foreach (var pickpoint in selectedPickpoints)
                    {
                        try
                        {
                            ctx.Status($"Получение зависших шк для {pickpoint.Name}...");
                            deductions.AddRange(await client.GetHandingGoodsAsync(pickpoint));

                            ctx.Status($"Получение брака для {pickpoint.Name}...");
                            deductions.AddRange(await client.GetDefectGoodsAsync(pickpoint));

                            ctx.Status($"Получение подмен для {pickpoint.Name}...");
                            deductions.AddRange(await client.GetReplaceGoodsAsync(pickpoint));

                            ctx.Status($"Получение тикетов для {pickpoint.Name}...");
                            tickets.AddRange(await client.GetTicketsAsync(pickpoint));
                        }
                        catch (Exception ex)
                        {
                            errorLog.Add($"Ошибка получения данных для ПВЗ {pickpoint.Name}! Описание ошибки: {ex.Message}");
                        }
                    }
                });


                while (true)
                {
                    var selectedItem = SpectreConsole.SelectDeduction(deductions, tickets, errorLog);
                    if (selectedItem == null) // проверка на назад
                        break;

                    if (selectedItem is DeductionGood deduction)
                    {
                        if (deduction.Details != null) // если детали уже были загружены, то не грузим их снова
                        {
                            try
                            {
                                SpectreConsole.ShowDeductionScreen(deduction);
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[red]Ошибка при загрузке деталей удержания! Описание ошибки: {ex.Message}[/]");
                                Console.ReadKey();
                            }
                        }
                        else 
                        {
                            try
                            {
                                var deductionDetails = await AnsiConsole.Status()
                                .Spinner(Spinner.Known.Dots)
                                .StartAsync("Загрузка...", async ctx =>
                                {
                                    var deductionPickpoint = selectedPickpoints.FirstOrDefault(p => p.Name == deduction.PickpointName);

                                    ctx.Status($"Получение данных по ШК {deduction.Shk}...");
                                    var details = await client.FindByShkAsync(deduction.Shk.ToString(), deductionPickpoint);

                                    return deduction.ToEnrich(details); // дополняем удержание данными по ШК
                                });

                                SpectreConsole.ShowDeductionScreen(deduction);
                            } catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[red]Ошибка при загрузке деталей удержания! Описание ошибки: {ex.Message}[/]");
                                Console.ReadKey();
                            }
                        }
                    }
                    if (selectedItem is Ticket ticket)
                    {
                        SpectreConsole.ShowTicketDetails(ticket);
                    }
                }
            }
        }
    }
}