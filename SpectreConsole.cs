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
                    .NotRequired()
                    .PageSize(10)
                    .MoreChoicesText("[grey](нажмите пробел для выбора)[/]")
                    .InstructionsText("[grey](используйте стрелки для навигации, пробел для выбора, Enter для подтверждения)[/]")
                    .UseConverter(pickpoint => pickpoint.Name)
                    .AddChoices(allPickpoints));

            return selectedPickpoints;
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
