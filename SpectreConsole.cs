using Microsoft.Win32.SafeHandles;
using Spectre.Console;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using WildberriesDeductions.Models;

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
            var prompt = new SelectionPrompt<object>()
                .Title("[yellow]Выберите товар для деталей или вернитесь назад:[/]")
                .PageSize(15)
                .MoreChoicesText("[grey](Листайте вниз, чтобы увидеть все товары)[/]")
                .AddChoices("<- НАЗАД (К выбору ПВЗ)")
                .AddChoices(deductions.OrderBy(g => g.OnHoldDate).Cast<object>());

            prompt.UseConverter(item =>
            {
                if (item is string str)
                    return $"[red]{str}[/]";
                if (item is DeductionGood d)
                {
                    string date = d.OnHoldDate?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "Нет данных";
                    string pp = d.Pickpoint.Length > 25 ? d.Pickpoint[..22] + "..." : d.Pickpoint;
                    return $"| {date} | {d.Reason, -12} | {pp, -25} | {d.DeductionPrice, -6} руб. | {d.Shk, -15} |";
                }

                return item.ToString() ?? "Неизвестно";
            });

            var selected = AnsiConsole.Prompt(prompt);
            if (selected is string)
                return null; 

            return selected as DeductionGood;
        }


        public static void PrintDeductionGoods(List<GoodsInterface> allGoods)
        {
            Table table = new Table();
            table.AddColumns("Дата удержания", "Вид удержания", "ПВЗ", "Стоимость", "ШК");
            table.RoundedBorder();
            table.Width = AnsiConsole.Console.Profile.Width;
            table.Title("Удержания");
            allGoods = allGoods.OrderBy(g => g.OnHoldDate).ToList();
            foreach (var goods in allGoods)
            {
                string price = goods.Price.ToString();
                price += " руб.";
                string Shk = goods.Shk.ToString();
                if (goods is Ticket)
                {
                    Shk = goods.TicketId.ToString();
                    price = "-";
                    table.Columns[4].Header = new Text("ШК | Номер тикета");
                }

                table.AddRow(
                    goods.OnHoldDate?.ToLocalTime().ToString(),
                    goods.Reason,
                    goods.Pickpoint,
                    price,
                    Shk
                );
            }
            AnsiConsole.Write(table);
        }
    }
}
