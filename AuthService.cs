using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public class AuthService(TokenStorage tokenStorage, WbRequestService wbService, RequestConfig loginConfig, RequestConfig validateConfig)
    {
        private string _switchToken = tokenStorage.Load();
        private string _accessToken = string.Empty;

        public bool IsTokenValid()
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(_switchToken);

            // Достаем дату истечения
            DateTime expiration = jwtToken.ValidTo.ToLocalTime();

            if (DateTime.Now < expiration)
                return true;
            else
                return false;
        }

        public void Save(string token)
        {
            tokenStorage.Save(token);
        }

        public string Load()
        {
            return tokenStorage.Load();
        }

        public async Task<bool> TryLoginAsync(long phoneNumber)
        {
            try
            {
                string loginJson = await wbService.Login(loginConfig, phoneNumber);
                var loginResponse = ResultsConverter.ConvertLoginResponse(loginJson);
                _accessToken = loginResponse.LoginToken;
                Debug.WriteLine("Запрос login выполнен");
                return true;
            }
            catch
            {
                Console.WriteLine("Ошибка при попытке входа. Проверьте номер телефона и повторите попытку.");
                return false;
            }
        }

        public async Task<bool> TryValidateAsync(int code)
        {
            try
            {
                string validateJson = await wbService.Validate(validateConfig, _accessToken, code);
                var validateResponse = ResultsConverter.ConvertValidateResponse(validateJson);
                _switchToken = validateResponse.Access.Token;
                Debug.WriteLine("Запрос validate выполнен");

                Save(_switchToken);
                return true;
            }
            catch
            {
                Console.WriteLine("Ошибка при валидации кода. Проверьте код и повторите попытку.");
                return false;
            }
        }
    }
}
