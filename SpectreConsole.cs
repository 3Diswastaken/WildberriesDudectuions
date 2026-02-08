using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                table.AddRow(
                    goods.OnHoldDate?.ToString("dd.MM.yy HH:mm:ss", CultureInfo.InvariantCulture),
                    goods.Reason,
                    goods.Pickpoint,
                    price,
                    goods.Shk.ToString()
                );
            }
            AnsiConsole.Write(table);
        }
    }
}
