using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public class Client(HttpClient httpClient, AuthService authService)
    {
        // конфиг на каждый тип удержаний со своими url и headers
        RequestConfig defectGoodsConfig = ConfigLoader.LoadDefectGoods();
        RequestConfig ticketsConfig = ConfigLoader.LoadTickets(); // использует post и payload
        RequestConfig replaceGoodsConfig = ConfigLoader.LoadReplaceGoods();
        RequestConfig handingGoodsConfig = ConfigLoader.LoadHandingGoods();
        RequestConfig loginConfig = ConfigLoader.LoadLogin();
        RequestConfig validateConfig = ConfigLoader.LoadValidate();
        RequestConfig switchConfig = ConfigLoader.LoadSwitch();
        UserConfig userConfig = ConfigLoader.LoadUserConfig(); // хранит токен и список пвз с их 

        WbRequestService service = new WbRequestService(httpClient);

        private string _accessToken = string.Empty;
        private string _switchToken = authService.Load();
        public async Task TryLoginAsync(long phoneNumber)
        {
            try
            {
                string loginJson = await service.Login(loginConfig, phoneNumber);
                var loginResponse = ResultsConverter.ConvertLoginResponse(loginJson);
                _accessToken = loginResponse.LoginToken;
                Debug.WriteLine("Запрос login выполнен");
            }
            catch
            {
                Console.WriteLine("Ошибка при попытке входа. Проверьте номер телефона и повторите попытку.");
            }
        }

        public async Task TryValidateAsync(int code)
        {
            try
            {
                string validateJson = await service.Validate(validateConfig, _accessToken, code);
                var validateResponse = ResultsConverter.ConvertValidateResponse(validateJson);
                _switchToken = validateResponse.Access.Token;
                Debug.WriteLine("Запрос validate выполнен");

                authService.Save(_switchToken);
            }
            catch
            {
                Console.WriteLine("Ошибка при валидации кода. Проверьте код и повторите попытку.");
            }
        }

        public bool IsTokenValid()
        {
            return authService.IsTokenValid();
        }

        public async Task TrySwitchAsync(string pickpointName)
        {
            string switchJson = await service.Switch(userConfig, switchConfig, _switchToken, pickpointName);
            var switchResponse = ResultsConverter.ConvertSwitchResponse(switchJson);
            Debug.WriteLine($"Запрос switch на {pickpointName} выполнен.");
            string requestToken = switchResponse.Access.Token;
            userConfig.Token = requestToken;
        }

        public async Task<List<DefectGood>> GetDefectGoodsAsync(string pickpointName)
        {
            List<DefectGood> defectGoods = new();
            try
            {
                string resultJson = await service.GetDefectGoods(userConfig, defectGoodsConfig, pickpointName);
                var resultDefectGoods = ResultsConverter.ConvertDefectGoods(resultJson);
                Debug.WriteLine($"Запрос defectGoods на {pickpointName} выполнен.");
                foreach (var item in resultDefectGoods.Goods)
                {
                    if (item.TicketId == 0 && item.BalanceStatus == "to_hold")
                    {
                        item.Pickpoint = pickpointName;
                        defectGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по бракованным товарам на пвз {pickpointName}.");
            }
            
            return defectGoods;
        }

        public async Task<List<ReplaceGood>> GetReplaceGoodsAsync(string pickpointName)
        {
            List<ReplaceGood> replaceGoods = new();
            try
            {
                string resultJson = await service.GetReplaceGoods(userConfig, replaceGoodsConfig, pickpointName);
                var resultReplaceGoods = ResultsConverter.ConvertReplaceGoods(resultJson);
                Debug.WriteLine($"Запрос replaceGoods на {pickpointName} выполнен.");
                foreach (var item in resultReplaceGoods.Goods)
                {
                    if (item.TicketId == 0)
                    {
                        item.Pickpoint = pickpointName;
                        replaceGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по товарам на замену на пвз {pickpointName}.");
            }
            return replaceGoods;
        }
        public async Task<List<HandingGood>> GetHandingGoodsAsync(string pickpointName)
        {
            List<HandingGood> handingGoods = new();
            try
            {
                string resultJson = await service.GetHandingGoods(userConfig, handingGoodsConfig, pickpointName);
                var resultHandingGoods = ResultsConverter.ConvertHandingGoods(resultJson);
                Debug.WriteLine($"Запрос handingGoods на {pickpointName} выполнен.");
                foreach (var item in resultHandingGoods.data.Goods)
                {
                    if (item.Type == "expired" && item.TicketId == 0)
                    {
                        item.Pickpoint = pickpointName;
                        handingGoods.Add(item);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Ошибка при получении данных об удержаниях по зависшим ШК на пвз {pickpointName}.");
            }
            return handingGoods;
        }
    }
}