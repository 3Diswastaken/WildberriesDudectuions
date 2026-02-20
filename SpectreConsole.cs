using Spectre.Console;
using System.Diagnostics;
using System.Globalization;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public static class SpectreConsole
    {
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
