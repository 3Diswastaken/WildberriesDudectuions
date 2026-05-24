using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesDeductions.IPEye
{
    public class IPEyeAuthService(TokenStorage tokenStorage, HttpClientHandler httpHandler)
    {
        private string _cookieFileName = "IPEyeSessionCookie.txt";

        public void SaveSessionCookie()
        {
            var uri = new Uri("https://www.ipeye.ru/");
            var cookies = httpHandler.CookieContainer.GetCookies(uri);

            List<string> cookiePairs = new List<string>();
            foreach (Cookie cookie in cookies)
            {
                cookiePairs.Add($"{cookie.Name}={cookie.Value}");
            }

            if (cookiePairs.Count > 0)
            {
                string fullCookieString = string.Join("; ", cookiePairs);
                tokenStorage.Save(fullCookieString, _cookieFileName);
                Debug.WriteLine($"[IPEye] Сессия сохранена: {fullCookieString}");
            }
        }

        public void TryLoadSessionCookie()
        {
            string cookieHeader = tokenStorage.LoadAccessToken(_cookieFileName);
            if (string.IsNullOrEmpty(cookieHeader))
                Debug.WriteLine("Куки не найдены"); // куки не найдены
            var uri = new Uri("https://www.ipeye.ru");

            httpHandler.CookieContainer.SetCookies(uri, cookieHeader);
            Debug.WriteLine("[IPEye] Все куки успешно восстановлены из файла.");
        }
    }
}
