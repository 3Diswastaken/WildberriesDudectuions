using Spectre.Console;
using WildberriesDeductions.Wildberries.Models;
using WildberriesDeductions.Wildberries.Models.Dto;

namespace WildberriesDeductions
{
    public static class SpectreConsole
    {
        public static long AskForPhone()
        {
            string phoneNumberInput = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("Введите номер телефона: [green]+7[/]")
                        .PromptStyle("green")
                        .Validate(input =>
                        {
                            if (input.Length != 10)
                                return ValidationResult.Error("[red]Нужно ровно 10 цифр![/]");
                            if (!input.All(char.IsDigit))
                                return ValidationResult.Error("[red]Только цифры, пожалуйста[/]");
                            return ValidationResult.Success();
                        })
                    )
                    .Insert(0, "7"); // Добавляем '7' в начало после успешного ввода

            AnsiConsole.Clear();
            return long.Parse(phoneNumberInput);
        }

        public static int AskForCode()
        {
            string codeInput = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>($"Введите код подтверждения, отправленный на ваш номер:")
                        .PromptStyle("green")
                        .Validate(input =>
                        {
                            if (input.Length != 6)
                                return ValidationResult.Error("[red]Код должен состоять из 6 цифр![/]");
                            if (!input.All(char.IsDigit))
                                return ValidationResult.Error("[red]Только цифры, пожалуйста[/]");

                            return ValidationResult.Success();
                        })
                    );
            AnsiConsole.Clear();
            return int.Parse(codeInput);
        }

        public static List<Pickpoint> SelectPickpoints(List<Pickpoint> allPickpoints)
        {
            AnsiConsole.Clear();

            var selectedPickpoints = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Pickpoint>()
                    .Title("Выберите ПВЗ для получения данных:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](нажмите пробел для выбора)[/]")
                    .InstructionsText("[grey](используйте стрелки для навигации, пробел для выбора, Enter для подтверждения)[/]")
                    .UseConverter(pickpoint => pickpoint.Name)
                    .AddChoices(allPickpoints));

            return selectedPickpoints;
        }

        public static object? SelectDeduction(List<DeductionGood> deductions, List<Ticket> tickets, List<string> errorLog)
        {
            AnsiConsole.Clear();

            List<object> combinedList = new List<object>();
            combinedList.AddRange(deductions);
            combinedList.AddRange(tickets);
            
            if (errorLog.Any())
            {
                foreach (var error in errorLog)
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]{error}[/]");
                }   
                AnsiConsole.WriteLine("");
            }

            var prompt = new SelectionPrompt<object>()
                .Title("[yellow]Выберите товар для деталей или вернитесь назад:[/]")
                .PageSize(30)
                .MoreChoicesText("[grey](Листайте вниз, чтобы увидеть все товары)[/]")
                .AddChoices("<- Назад (К выбору ПВЗ)")
                .AddChoices(combinedList.OrderBy(x => (x as dynamic).OnHoldDate));

            prompt.UseConverter(item =>
            {
                if (item is DeductionGood d)
                {
                    string date = d.OnHoldDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "Нет данных";
                    string pp = d.ShortPickpointName.Length > 20 ? d.ShortPickpointName[..17] + "..." : d.ShortPickpointName;
                    string price = d.DeductionPrice.ToString() + " руб.";
                    return $" {date}  {d.Reason,-12}  {pp,-20}  {price,-12}  {d.Shk,-15} ";
                }
                if (item is Ticket t)
                {
                    string date = t.OnHoldDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "Нет данных";
                    string id = $"Тикет #{t.Id.ToString()}";
                    string pp = t.ShortPickpointName.Length > 20 ? t.ShortPickpointName[..17] + "..." : t.ShortPickpointName;
                    string price = "0 руб.";
                    return $" {date}  {t.Reason,-12}  {pp,-20}  {price, -12}  {id, -8}";
                }

                return item.ToString() ?? "Неизвестно";
            });

            var selected = AnsiConsole.Prompt(prompt);

            if (selected is DeductionGood)
                return selected as DeductionGood;
            else if (selected is Ticket)
                return selected as Ticket;
            else  return null;
        }

        public static void PrintDeductionDetails(DeductionGood deduction)
        {
            AnsiConsole.Clear();

            var table = new Table();
            table.Title("Детали удержания");
            table.AddColumns("ПВЗ", "Дата удержания", "Вид удержания", "Стоимость товара", "ШК", "Ячейка");
            table.Width = AnsiConsole.Console.Profile.Width;

            string price = deduction.DeductionPrice.ToString() + " руб.";
            string shk = deduction.Shk.ToString();
            string date = deduction.OnHoldDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            string pickpoint = deduction.PickpointName;
            string cell = deduction.Details.Cell.ToString();


            table.AddRow(
                pickpoint,
                date,
                deduction.Reason,
                price,
                shk,
                cell
            );

            var statusHistoryTable = new Table();
            statusHistoryTable.Title("История статусов товара");
            statusHistoryTable.Width = AnsiConsole.Console.Profile.Width;
            var statusHistory = deduction.Details.StatusHistory.ToList();
            statusHistoryTable.AddColumns("Статус", "Время", "Сотрудник");
            foreach (var status in statusHistory)
            {
                statusHistoryTable.AddRow(
                    status.StatusName,
                    status.Date.ToString("dd.MM.yyyy HH:mm:ss"),
                    status.EmployeeFullName
                    );
            }

            var defectMarkTable = new Table();
            defectMarkTable.Title("Данные об отметке о браке");
            defectMarkTable.Width = AnsiConsole.Console.Profile.Width;
            var defectMark = deduction.Details.DefectMark.FirstOrDefault();
            defectMarkTable.AddColumns("Дата отметки о браке", "Комментарий", "Фотографии");
            var defectMarkPhotos = defectMark?.Photos ?? new List<string>();
            string? photosUrl = string.Join("\n", defectMarkPhotos.Select((url, i) => $"{url}"));

            defectMarkTable.AddRow(defectMark?.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss") ?? "-", defectMark?.Comment ?? "-", photosUrl ?? "-");


            AnsiConsole.Write(table);
            AnsiConsole.Write(defectMarkTable);
            AnsiConsole.Write(statusHistoryTable);
        }

        public enum DetailsAction { Back }

        public static DetailsAction ShowNavigationMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<DetailsAction>()
                    .Title("[grey]Выберите действие:[/]")
                    .UseConverter(a => a switch
                    {
                        DetailsAction.Back => "<- Вернуться к списку"
                    })
                    .AddChoices(DetailsAction.Back)
            );
        }

        public static void ShowDeductionScreen(DeductionGood deduction)
        {
            bool stayOnScreen = true;
            while (stayOnScreen)
            {
                PrintDeductionDetails(deduction);
                var action = ShowNavigationMenu();

                if (action == DetailsAction.Back)
                {
                    stayOnScreen = false; // Выходим из цикла, метод завершится
                }
            }
        }


        private static void PrintTicketDetails(Ticket ticket)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[yellow]Тикет #{ticket.Id}[/]");
            AnsiConsole.MarkupLine($"[green]Тип:[/] {ticket.TypeName}");
            AnsiConsole.MarkupLine($"[green]ПВЗ:[/] {ticket.PickpointName}");
            AnsiConsole.MarkupLine($"[green]Дата создания:[/] {ticket.CreateDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss") ?? "Нет данных"}");
            AnsiConsole.MarkupLine($"[green]Дата завершения:[/] {ticket.FinishDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss") ?? "Нет данных"}");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[green]Описание:[/] {ticket.Description}");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[green]История изменений:[/]");
            foreach (var log in ticket.Logs)
            {
                if (log.UserId == ticket.UserId)
                {
                    AnsiConsole.MarkupLine($"  [blue]{log.CreateDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss")} - {log.FullName} - [/][white]{log.Message}[/]");
                    AnsiConsole.MarkupLine("");
                }
                 else
                {
                    AnsiConsole.MarkupLine($"  [purple]{log.CreateDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss")} - {log.FullName} - [/][white]{log.Message}[/]");
                    AnsiConsole.MarkupLine("");
                }
            }
        }

        public static void ShowTicketDetails(Ticket ticket)
        {
            bool stayOnScreen = true;
            while (stayOnScreen)
            {
                PrintTicketDetails(ticket);

                var action = ShowNavigationMenu();
                if (action == DetailsAction.Back)
                {
                    stayOnScreen = false; // Выходим из цикла, метод завершится
                }
            }
        }
    }
}