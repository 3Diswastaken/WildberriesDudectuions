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
            table.Title("Удержания");
            allGoods = allGoods.OrderBy(g => g.OnHoldDate).ToList();
            foreach (var goods in allGoods)
            {
                string price = goods.Price.ToString();
                price += " руб.";
                string Shk = goods.Shk.ToString();
                if (goods.GetType() == typeof(Ticket))
                {
                    Shk = goods.TicketId.ToString();
                    price = "-";
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
