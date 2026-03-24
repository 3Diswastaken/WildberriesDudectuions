using Spectre.Console;
using WildberriesDeductions.Wildberries.Models;

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

        public static DeductionGood? SelectDeduction(List<DeductionGood> deductions)
        {
            AnsiConsole.Clear();

            var prompt = new SelectionPrompt<object>()
                .Title("[yellow]Выберите товар для деталей или вернитесь назад:[/]")
                .PageSize(30)
                .MoreChoicesText("[grey](Листайте вниз, чтобы увидеть все товары)[/]")
                .AddChoices("<- Назад (К выбору ПВЗ)")
                .AddChoices(deductions.OrderBy(g => g.OnHoldDate).Cast<object>());

            prompt.UseConverter(item =>
            {
                if (item is DeductionGood d)
                {
                    string date = d.OnHoldDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "Нет данных";
                    string pp = d.Pickpoint.Length > 40 ? d.Pickpoint[..37] + "..." : d.Pickpoint;
                    string price = d.DeductionPrice.ToString() + " руб.";
                    return $" {date}  {d.Reason, -12}  {pp, -40}  {price, -12}  {d.Shk, -15} ";
                }

                return item.ToString() ?? "Неизвестно";
            });

            var selected = AnsiConsole.Prompt(prompt);
            if (selected is string)
                return null; 

            return selected as DeductionGood;
        }

        public static void PrintDeductionDetails(DeductionGood deduction)
        {
            AnsiConsole.Clear();

            var table = new Table();
            table.Title("Детали удержания");
            table.AddColumns("Время отметки о браке", "Дата удержания", "Вид удержания", "Стоимость товара", "ШК", "Ячейка");
            table.Width = AnsiConsole.Console.Profile.Width;

            string price = deduction.DeductionPrice.ToString() + " руб.";
            string shk = deduction.Shk.ToString();
            string date = deduction.OnHoldDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            string defectMarkDate = deduction.Details.DefectMark.FirstOrDefault()?.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss") ?? "-";
            string cell = deduction.Details.Cell.ToString();


            table.AddRow(
                defectMarkDate,
                date,
                deduction.Reason,
                price,
                shk,
                cell
            );

            var statusHistoryTable = new Table();
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
            AnsiConsole.Write(table);
            AnsiConsole.Write(statusHistoryTable);
        }

        public enum DetailsAction {Back}

        public static DetailsAction ShowDetailsMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<DetailsAction>()
                    .Title("[grey]Выберите действие:[/]")
                    .UseConverter(a => a switch {
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
                var action = ShowDetailsMenu();

                if (action == DetailsAction.Back)
                {
                    stayOnScreen = false; // Выходим из цикла, метод завершится
                }
            }
        }
    }
}
