using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            var storage = new TokenStorage();
            AuthService authService = new(storage);

            Client client = new(httpClient, authService);

            List<GoodsInterface> allGoods = new();

            if (!client.IsTokenValid())
            {
                Console.WriteLine("Токен недействителен или отсутствует. Необходимо выполнить вход.");

                await client.TryLoginAsync(79605174813);
                string? input = null;

                Console.WriteLine("Введите код подтверждения, отправленный на ваш номер:");
                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Ошибка: код подтверждения не может быть пустым.");
                    return;
                }
                int code = int.Parse(input);

                await client.TryValidateAsync(code);

            }

            List<string> pickpoints = new() { "p105", "p10" };
            foreach (var pickpoint in pickpoints)
            {
                await client.TrySwitchAsync(pickpoint);

                var handingGoods = await client.GetHandingGoodsAsync(pickpoint);
                allGoods.AddRange(handingGoods);
                var defectGoods = await client.GetDefectGoodsAsync(pickpoint);
                allGoods.AddRange(defectGoods);
                var replaceGoods = await client.GetReplaceGoodsAsync(pickpoint);
                allGoods.AddRange(replaceGoods);
            }

            SpectreConsole.PrintDeductionGoods(allGoods);

            Console.ReadLine();
        }
    }
}
