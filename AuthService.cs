using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildberriesDeductions;
using System.IdentityModel.Tokens.Jwt;

namespace WildberriesDeductions
{
    public class AuthService(TokenStorage tokenStorage)
    {
        private string _switchToken = tokenStorage.Load();

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
    }
}
