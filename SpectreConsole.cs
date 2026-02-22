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
        public static async Task AskToLoginAndValidate(AuthService authService)
        {
            if (!authService.IsTokenValid())
            {
                await TryLogin(authService);
                await TryValidate(authService);
            }
        }

        private static async Task TryLogin(AuthService authService)
        {
            int attempts = 5;
            while (true)
            {
                if (attempts == 0)
                    return;
                attempts--;

                string phoneNumberInput = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("Введите номер телефона: [green]+7[/]")
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
                long phone = long.Parse(phoneNumberInput);

                if (await authService.TryLoginAsync(phone))
                {
                    AnsiConsole.Clear();
                    break;
                }
                AnsiConsole.MarkupLine($"[red]Ошибка при отправке кода подтверждения. Проверьте номер телефона и попробуйте снова. Осталось попыток {attempts}[/]");
            }
        }

        private static async Task TryValidate(AuthService authService)
        {
            int attempts = 5;
            while (true)
            {
                if (attempts == 0)
                    return;
                attempts--;

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
                int code = int.Parse(codeInput);

                if (await authService.TryValidateAsync(code))
                {
                    AnsiConsole.Clear();
                    break;
                }

                AnsiConsole.MarkupLine("[red]Ошибка при проверке кода подтверждения. Проверьте код и попробуйте снова.[/]");
            }
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
