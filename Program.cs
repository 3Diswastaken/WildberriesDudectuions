using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WildberriesDudectuions.Models;

namespace WildberriesDudectuions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            // конфиг на каждый тип удержаний со своими url и headers
            RequestConfig defectGoodsConfig = ConfigLoader.LoadDefectGoods();
            RequestConfig ticketsConfig = ConfigLoader.LoadTickets(); // использует post и payload
            RequestConfig replaceGoodsConfig = ConfigLoader.LoadReplaceGoods();
            RequestConfig loginConfig = ConfigLoader.LoadLogin();
            RequestConfig validateConfig = ConfigLoader.LoadValidate();
            RequestConfig switchConfig = ConfigLoader.LoadSwitch();
            UserConfig userConfig = ConfigLoader.LoadUserConfig(); // хранит токен и список пвз с их 

            WbRequestService service = new(httpClient);

            List<GoodsInterface> allGoods = new();

            // тест логина
            Console.WriteLine("Введите номер телефона для логина (только цифры, начиная с семи):");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Ошибка: номер телефона не может быть пустым.");
                return;
            }
            long phoneNumber = long.Parse(input);

            string loginJson = await service.Login(loginConfig, phoneNumber);

            var loginResponse = ResultsConverter.ConvertLoginResponse(loginJson);

            Console.WriteLine("Введите код подтверждения, отправленный на ваш номер:");
            input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Ошибка: код подтверждения не может быть пустым.");
                return;
            }
            int code = int.Parse(input);

            string validateJson = await service.Validate(validateConfig, loginResponse.LoginToken, code);
            var validateResponse = ResultsConverter.ConvertValidateResponse(validateJson);

            string accessToken = validateResponse.Access.Token;
            Console.WriteLine($"Access Token: {accessToken}");

            string switchJson = await service.Switch(userConfig, switchConfig, accessToken, "p105");
            var switchResponse = ResultsConverter.ConvertSwitchResponse(switchJson);
            Console.WriteLine($"Switch Token: {switchResponse.Access.Token}");




            return;

            // конец теста логина


            /*foreach (var pickpoint in userConfig.PickpointId)
            {
                try
                {
                    // получаем подмены
                    // сначала получаем полный ответ в виде json
                    string jsonData = await service.GetDataForPickpoint(userConfig, replaceGoodsConfig, pickpoint.Key);
                    // конвертируем json в объект
                    var resultReplaceGoods = ResultsConverter.ConvertReplaceGoods(jsonData);
                    // если у товара нет тикета, значит он не обработан и его нужно учитывать
                    foreach (var item in resultReplaceGoods.ReplaceGoodsList)
                    {
                        if (item.TicketId == 0)
                        {
                            item.Pickpoint = pickpoint.Key;
                            allGoods.Add(item);
                        }
                    }

                    // получаем брак
                    jsonData = await service.GetDataForPickpoint(userConfig, defectGoodsConfig, pickpoint.Key);
                    var resultDefectGoods = ResultsConverter.ConvertDefectGoods(jsonData);
                    foreach (var item in resultDefectGoods.DefectGoodsList)
                    {
                        if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                        {
                            item.Pickpoint = pickpoint.Key;
                            allGoods.Add(item);
                        }
                    }

                    // получаем тикеты
                    jsonData = await service.GetDataForPickpoint(userConfig, ticketsConfig, pickpoint.Key);
                    var reslutTickets = ResultsConverter.ConvertTickets(jsonData);
                    foreach (var item in reslutTickets.Tickets)
                    {
                        if (item.OnHoldDate != null)
                        {
                            item.Pickpoint = pickpoint.Key;
                            allGoods.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при запросе {ex.Message}");
                }
            }

            // сортировка по дате удержания полученных удержаний
            var sortedGoods = allGoods.OrderBy(g => g.OnHoldDate);
            foreach (var item in sortedGoods)
            {
                Console.WriteLine($"{item.OnHoldDate?.ToString("dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture)} {item.Reason} {item.Pickpoint}");
            }*/

            // задержка перед закрытием консоли
            Console.ReadLine();
        }
    }
}
