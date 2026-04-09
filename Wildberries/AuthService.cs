using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using WildberriesDeductions.Wildberries.Models;
using WildberriesDeductions.Wildberries.Models.Dto;

namespace WildberriesDeductions.Wildberries
{
    public class AuthService(TokenStorage tokenStorage, WbRequestService wbService, RequestConfig loginConfig, RequestConfig validateConfig, RequestConfig switchConfig)
    {
        public bool IsTokenValid()
        {
            string tokenForSwitch = tokenStorage.LoadAccessToken();
            if (tokenForSwitch == string.Empty)
                return false;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenForSwitch);

            // Достаем дату истечения
            DateTime expiration = jwtToken.ValidTo.ToLocalTime();

            if (DateTime.Now < expiration)
                return true;
            else
                return false;
        }

        private void Save(string accessToken)
        {
            tokenStorage.Save(accessToken);
        }

        public string Load()
        {
            return tokenStorage.LoadAccessToken();
        }

        public async Task<string> LoginAsync(long phoneNumber)
        {
            string tokenForValidate = string.Empty;

            var result = await wbService.Login(loginConfig, phoneNumber);
            tokenForValidate = result.LoginToken;
            Debug.WriteLine("Запрос login выполнен");

            return tokenForValidate;
        }

        public async Task<bool> ValidateCodeAndSaveTokenAsync(int code, string tokenForValidate)
        {
            string tokenForSwitch = string.Empty;

            var result = await wbService.Validate(validateConfig, tokenForValidate, code);
            tokenForSwitch = result.Access.Token;
            Debug.WriteLine("Запрос validate выполнен");

            Save(tokenForSwitch);
            return true;
        }

        public async Task<string>  UpdatePickpointToken(Pickpoint pickpoint)
        {
            string tokenForSwitch = Load();

            var result = await wbService.Switch(switchConfig, tokenForSwitch, pickpoint);
            Debug.WriteLine($"Запрос switch на {pickpoint.Name} выполнен.");

            return result.Access.Token;
        }
    }
}
